using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public class PasswordManager
    {
        string conString = "Data Source=DESKTOP-U5VBMMH; Initial Catalog=PasswordManager; Integrated Security = True;";
        public string InMemoryEncryptionKey = "";
        public PasswordManager() { }

        public bool ValidatePassword(string inputPassword)
        {
            var result = getEncryptedVaultKeyAndSaltFromDatabase();
            string KeyInDatabase = result.Item1;
            string IVFromDatabase = result.Item3;

            Console.WriteLine("Database Key (vault):" + KeyInDatabase);
            Console.WriteLine("IV from database:" + IVFromDatabase);

            string EncryptionKey = deriveEncryptionKeyFromPassword(inputPassword);

            Console.WriteLine("Encryption KEy:" + EncryptionKey);
            byte[] encryptedVaultKey;
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = aes.CreateEncryptor(Convert.FromBase64String(EncryptionKey), Convert.FromBase64String(IVFromDatabase)); //IV has to somehow be the same

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(csEncrypt))
                        {
                            sw.Write(inputPassword);
                        }
                        encryptedVaultKey = msEncrypt.ToArray();
                    }
                }
            }
            Console.WriteLine("Derived VaultKey:" + Convert.ToBase64String(encryptedVaultKey));
            return Convert.ToBase64String(encryptedVaultKey) == KeyInDatabase;
        }

        public string deriveEncryptionKeyFromPassword(string inputPassword)
        {
            var result = getEncryptedVaultKeyAndSaltFromDatabase();
            string KeyInDatabase = result.Item1;
            string SaltInDatabase = result.Item2;

            //Get encryptionKey with Pbkdf2
            byte[] EncryptionKey = (KeyDerivation.Pbkdf2(
                password: inputPassword.Trim(),
                salt: Convert.FromBase64String(SaltInDatabase),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10,
                numBytesRequested: 256 / 8));

            return Convert.ToBase64String(EncryptionKey);
        }
        public (string, string, string) getEncryptedVaultKeyAndSaltFromDatabase()
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                try
                {
                    con.Open();
                    string selectQuery = "select * from Users";
                    string vaultKeyResult = "";
                    string saltResult = "";
                    string IVResult = "";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    vaultKeyResult = reader.GetString(0);
                                    saltResult = reader.GetString(1);
                                    IVResult = reader.GetString(2);
                                }
                            }
                            reader.Close();
                        }
                    }

                    con.Close();

                    return (vaultKeyResult, saltResult, IVResult);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return ("nothing", "", "");
                }

            }
        }

        public void createNewMasterPassword(string inputPassword)
        {
            //derive encryptionKey from password
            var result = createNewEncryptionKeyAndSaltFromPassword(inputPassword);
            string encryptionKey = result.Item1;
            string salt = result.Item2;

            Console.WriteLine("enc key" + encryptionKey);
            //create custom IV for length
            byte[] ivToInject = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(ivToInject);
            }

            // encrypt encryptionKey as Vaultkey
            byte[] encryptedVaultKey;
            using (Aes aes = Aes.Create())
            {
                aes.IV = ivToInject;
                Console.WriteLine("Injected IV: " + ivToInject);
                aes.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = aes.CreateEncryptor(Convert.FromBase64String(encryptionKey), aes.IV); //IV has to somehow be the same

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(csEncrypt))
                        {
                            sw.Write(inputPassword);
                        }
                        encryptedVaultKey = msEncrypt.ToArray();
                    }
                }

                //store Vaultkey + salt + IV in database for later use.

                using (SqlConnection con = new SqlConnection(conString))
                {
                    try
                    {
                        con.Open();
                        string InsertQuery = "INSERT INTO Users (vaultKey, salt, IV) VALUES (@KeyValue, @salt, @IV)"; //husk salt

                        using (SqlCommand cmd = new SqlCommand(InsertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@KeyValue", Convert.ToBase64String(encryptedVaultKey));
                            cmd.Parameters.AddWithValue("@salt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(aes.IV));

                            Console.WriteLine(cmd.ExecuteNonQuery());
                        }

                        con.Close();


                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }
                }

            }
        }

        public (string, string) createNewEncryptionKeyAndSaltFromPassword(string inputPassword)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            //Get encryptionKey with Pbkdf2
            byte[] EncryptionKey = (KeyDerivation.Pbkdf2(
                password: inputPassword.Trim(),
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10,
                numBytesRequested: 256 / 8));

            return (Convert.ToBase64String(EncryptionKey), Convert.ToBase64String(salt));
        }

        public Dictionary<string, string> getAllPasswordsInDatabase()
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                try
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();

                    con.Open();
                    string selectQuery = "select * from passwords"; //husk salt
                    string vaultKeyResult = "";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dict.Add(reader.GetString(0), reader.GetString(1));
                                }
                            }
                            reader.Close();
                        }
                    }

                    con.Close();
                    Dictionary<string, string> decryptedDict = new Dictionary<string, string>();
                    //foreach dict-Value: decrypt and add into new array.
                    foreach (KeyValuePair<string, string> kvp in dict)
                    {
                        if (kvp.Value != null)
                        {
                            decryptedDict.Add(kvp.Key, decryptWebsitePassword(kvp.Value));
                        }
                    }
                    return decryptedDict;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }

            }
        }

        public string decryptWebsitePassword(string encryptedPassword)
        {
            string ivFromDatabase = getEncryptedVaultKeyAndSaltFromDatabase().Item3;
            string result = "";
            //InMemoryEncryptionKey = deriveEncryptionKeyFromPassword("strongpassword12345"); //remove after
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(Convert.FromBase64String(InMemoryEncryptionKey), Convert.FromBase64String(ivFromDatabase));

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedPassword)))
                {
                    using(CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return result;
        }
        public void createNewWebsitePassword(string websiteName, int length)
        {
            var result = getEncryptedVaultKeyAndSaltFromDatabase();
            string IVFromDatabase = result.Item3;

            byte[] randomPassword = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomPassword);
            }

            //InMemoryEncryptionKey = deriveEncryptionKeyFromPassword("strongpassword12345"); //remove later
            byte[] encryptedPasswordBytes;
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = aes.CreateEncryptor(Convert.FromBase64String(InMemoryEncryptionKey), Convert.FromBase64String(IVFromDatabase)); //IV has to somehow be the same

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(csEncrypt))
                        {
                            sw.Write(Convert.ToBase64String(randomPassword));
                        }
                        encryptedPasswordBytes = msEncrypt.ToArray();
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(conString))
            {
                try
                {
                    con.Open();
                    string InsertQuery = "INSERT INTO passwords (website, EncryptedPasswords) VALUES (@WebsiteNameValue, @EncryptedPasswordValue)"; //husk salt

                    using (SqlCommand cmd = new SqlCommand(InsertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@WebsiteNameValue", websiteName);
                        cmd.Parameters.AddWithValue("@EncryptedPasswordValue", Convert.ToBase64String(encryptedPasswordBytes));

                        Console.WriteLine(cmd.ExecuteNonQuery());
                    }

                    con.Close();


                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }

            }
        }
    }
}
