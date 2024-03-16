using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

bool isLoggedIn = false;
string userNameFromInput;
string masterPasswordFromInput;

string vaultKey = "";
string sameSalt = "QiGjS6jCbraa9ztGLmDRtw==";


string conString = "Data Source=DESKTOP-U5VBMMH; Initial Catalog=PasswordManager; Integrated Security = True;";

PasswordManager.PasswordManager pwm = new PasswordManager.PasswordManager();
//pwm.createNewMasterPassword("strongpassword12345");
//Console.WriteLine(pwm.ValidatePassword("strongpassword12345"));
pwm.createNewWebsitePassword("Temu", 16);
pwm.createNewWebsitePassword("Twitch", 16);
foreach (string passwords in pwm.getAllPasswordsInDatabase().Values)
{
    Console.WriteLine(passwords);
}
/*




















var returned = EvaluateVaultkeyFromPasswordAndUsername("123", "username");
//InitiateStartup();
Console.WriteLine(returned.Item1);
Console.WriteLine(returned.Item2);



void InitiateStartup()
{
    while (!isLoggedIn)
    {
        Console.WriteLine("Type LOGIN to login, type CREATE to create account");
        string userLoginAction = Console.ReadLine();
        if (userLoginAction == "LOGIN")
        {
            startLogin();
        }
        else
        {
            startCreateAccount();
        }
    }


}

void startLogin()
{
    Console.WriteLine("Insert Credentials, please ");
    Console.Write("Username: ");
    userNameFromInput = Console.ReadLine();
    Console.Write("MasterPassword: ");
    masterPasswordFromInput = Console.ReadLine();
    if (masterPasswordFromInput != null)
    {
        //if masterPassword.hash == database.user.passwordhash
        isLoggedIn = true;
        Console.WriteLine("Logged in as" + userNameFromInput);
    }
}

void startCreateAccount()
{
    Console.WriteLine("Insert desired Username, please");
    Console.Write("Username: ");
    string newUserName = Console.ReadLine();
    Console.Write("password: ");
    string newUserPassword = Console.ReadLine();
    CreateNewAccount(newUserName, newUserPassword);
}

void CreateNewAccount(string userName, string userPassword)
{
    using (SqlConnection con = new SqlConnection(conString))
    {
        try
        {
            con.Open();
            (string vaultKey, string salt) = EvaluateVaultkeyFromPasswordAndUsername(userPassword, userName);
            string InsertQuery = "INSERT INTO Users (Username, vaultKey) VALUES (@NameValue, @KeyValue)"; //husk salt

            using (SqlCommand cmd = new SqlCommand(InsertQuery, con))
            {
                cmd.Parameters.AddWithValue("@Namevalue", userName);
                cmd.Parameters.AddWithValue("@KeyValue", vaultKey);

                Console.WriteLine(cmd.ExecuteNonQuery());
            }
            con.Close();
        } catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

//returns the vaultKey + salt
(string,string) EvaluateVaultkeyFromPasswordAndUsername(string passWord,string userName)
{
    byte[] salt = new byte[128 / 8];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(salt);
    }

    byte[] trySalt = Convert.FromBase64String(sameSalt);
    string hashedVaultKey = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: passWord + userName,
        salt: trySalt,
        prf: KeyDerivationPrf.HMACSHA512,
        iterationCount: 10,
        numBytesRequested: 256 / 8)); //iterationCount should be way higher, but for testing purposes we start at 10

    return (hashedVaultKey, Convert.ToBase64String(salt));
}

bool authenticateUserLogin(string userName, string userPassword)
{
    using (SqlConnection con = new SqlConnection(conString))
    {
        try
        {
            con.Open();
            string selectQuery = "select AuthKey from Users where Username = @nameValue"; //husk salt
            string authKeyResult = "";
            using (SqlCommand cmd = new SqlCommand(selectQuery, con))
            {
                cmd.Parameters.AddWithValue("@namevalue", userName);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            authKeyResult = reader.GetString(0);
                        }
                    }
                }
            }
            con.Close();

            //and derive a vaultkey
            string hashedVaultKey = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userPassword + userName,
                salt: Convert.FromBase64String(sameSalt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10,
                numBytesRequested: 256 / 8));

            //now make an auth key from vaultkey
            string authKey = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: hashedVaultKey + userPassword,
        salt: Convert.FromBase64String(sameSalt),
        prf: KeyDerivationPrf.HMACSHA512,
        iterationCount: 10,
        numBytesRequested: 256 / 8)); //iterationCount should be way higher, but for testing purposes we start at 10

            if (authKey == authKeyResult){
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}*/

