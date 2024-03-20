using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PasswordManager.PasswordManager passwordManager;
        public MainWindow()
        {
            InitializeComponent();
            passwordManager = new PasswordManager.PasswordManager();
            initializeMasterPasswordBox();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string inputPassword = passwordBox.Text;
            if (passwordManager.ValidatePassword(inputPassword))
            {
                setEverythingActive();
                passwordManager.InMemoryEncryptionKey = passwordManager.deriveEncryptionKeyFromPassword(inputPassword);
                Dictionary<string, string> result = passwordManager.getAllPasswordsInDatabase();
                List<string> websites = new List<string>();
                List<string> passwords = new List<string>();
                if (result!= null)
                {
                    foreach (string website in result.Keys)
                    {
                        websites.Add(website);
                    }
                    foreach (string password in result.Values)
                    {
                        passwords.Add(password);
                    }

                    websiteList.ItemsSource = websites;
                    passwordList.ItemsSource = passwords;
                }
                AlertLabel.Content = "User Authenticated";



            }
            else
            {
                AlertLabel.Content = "Wrong password";
            }

        }

        private void updatePWList()
        {
            try
            {
                Dictionary<string, string> result = passwordManager.getAllPasswordsInDatabase();
                if (result != null)
                {
                    List<string> websites = new List<string>();
                    List<string> passwords = new List<string>();
                    foreach (string website in result.Keys)
                    {
                        websites.Add(website);
                    }
                    foreach (string password in result.Values)
                    {
                        passwords.Add(password);
                    }

                    websiteList.ItemsSource = websites;
                    passwordList.ItemsSource = passwords;
                }
               
            }
            catch(Exception ex)
            {
                AlertLabel.Content = "Couldn't fetch passwords";
            }

        }

        private void ClickCreatePassword(object sender, RoutedEventArgs e)
        {
            string inputWebsiteName = websiteNameInput.Text;
            int inputLength = int.TryParse(passwordLengthInput.Text, out int result) ? result : 16; //default to 16 if bad input


            passwordManager.createNewWebsitePassword(inputWebsiteName, inputLength);
            updatePWList();
            AlertLabel.Content = "New Website Password Created!";
        }

        private void setEverythingActive()
        {
            websiteNameInput.IsEnabled = true;
            passwordLengthInput.IsEnabled = true;
            CreatePasswordButton.IsEnabled = true;
        }

        private void ClickCreateMasterPassword(object sender, RoutedEventArgs e)
        {
            if (newMasterPasswordBox.Text != "")
            {
                passwordManager.createNewMasterPassword(newMasterPasswordBox.Text);
                initializeMasterPasswordBox();
                newMasterPasswordBox.Text = "";
                AlertLabel.Content = "New User created";
            }
            
        }

        private void initializeMasterPasswordBox()
        {
            if (passwordManager.checkIfUserCreated())
            {
                newMasterPasswordBox.IsEnabled = false;
            }
            
        }
    }
}