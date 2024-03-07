using System.Security.Cryptography;

bool isLoggedIn = false;
string userNameFromInput;
string masterPasswordFromInput;
List<String> adminPasswords = new List<string>() { "yes", "this", "is", "a", "password"};
while (!isLoggedIn)
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
    }
}

if (isLoggedIn)
{
    foreach (string passwords in adminPasswords)
    {
        Console.WriteLine(createNewPassword(passwords));
    }
}

else
{
    Console.WriteLine("Not logged in");
}


string createVaultKeyFromMasterPassword(string masterPassword)
{
    //create a vaultkey using some kind of function (hvad med auth med server når det er lokalt? :/)
   
    return masterPassword.GetHashCode() + "";
}
string createNewPassword(string masterPassword)
{
    return masterPassword.GetHashCode() + "";
}