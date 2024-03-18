﻿using System.Text;
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
                foreach (string website in result.Keys)
                {
                     websites.Add( website);
                }
                foreach (string password in result.Values)
                {
                    passwords.Add( password );
                }

                websiteList.ItemsSource = websites;
                passwordList.ItemsSource = passwords;


            }
            else
            {
                //show that we failed somehow
            }

        }

        private void updatePWList()
        {
            try
            {
                Dictionary<string, string> result = passwordManager.getAllPasswordsInDatabase();
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void ClickCreatePassword(object sender, RoutedEventArgs e)
        {
            string inputWebsiteName = websiteNameInput.Text;
            int inputLength = int.TryParse(passwordLengthInput.Text, out int result) ? result : 16; //default to 16 if bad input


            passwordManager.createNewWebsitePassword(inputWebsiteName, inputLength);
            updatePWList();
        }

        private void setEverythingActive()
        {
            websiteNameInput.IsEnabled = true;
            passwordLengthInput.IsEnabled = true;
            CreatePasswordButton.IsEnabled = true;
        }
    }
}