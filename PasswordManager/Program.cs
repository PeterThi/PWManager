bool isLoggedIn = false;

while (!isLoggedIn)
{
    Console.WriteLine("Insert Credentials, please ");
    Console.Write("Username: ");
    string userNameFromInput = Console.ReadLine();
    Console.Write("Password: ");
    string passwordFromInput = Console.ReadLine();
    if (passwordFromInput != null)
    {
        isLoggedIn = true;
    }


}