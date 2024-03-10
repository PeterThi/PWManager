# PWManager



First the manager is going to require the user to sign up. I have chosen to do this using a Username and a MasterPassword, which the user has to input. This will create a user using the following flow:

![pwManagerCreate](https://github.com/PeterThi/PWManager/assets/60512162/1917a917-bd81-4ce1-83dd-11fcf8d4ad6a)

from this we get two keys: an Authentication Key and a Vault Key. The Authentication key is later used to authenticate the user, which is why it will be stored in a local database. The Vault Key will not be stored in the database, as that is a secret used to encrypt and decrypt passwords later.

After this, the user will have to log in to the application using their credentials again. The flow looks like this: 

![pwManagerLogin](https://github.com/PeterThi/PWManager/assets/60512162/8fa2caa5-126e-4765-8ed9-f21baa9ee101)

We go through the same steps as in the create function, so that we can derive a new AuthenticationKey. We will then compare the two authentication keys, and if they're a match, the user will be validated and logged in to the manager.

From here, the user will be able to manage their passwords. First of all, they will need to be able to create a new password in the system. This flow should look like this:

![pwManagerNewPassword](https://github.com/PeterThi/PWManager/assets/60512162/b74b3da4-9b8b-4af3-a4bc-0ccb5aea5a24)

In this flow we will generate a string of a set length using random letters and numbers - preferably mixing upper and lower case. This string is then encrypted using the derived VaultKey from the credentials, and stored encrypted in the database. When creating a password, the user can input the name of the website that the password is gonna correspond with for a better user experience. I have chosen not to include usernames or e-mails in this as a last resort if this manager should be compromised in some way. 

Finally, the manager must show the user their passwords with their corresponding websites in a list-like display (when logged in of course). Flow:

![pwManagerViewPasswords](https://github.com/PeterThi/PWManager/assets/60512162/c170198e-bc89-4bce-98a9-6c62b224f3f9)

if the user is logged in, we will gather all encrypted passwords from the database. Here the manager will decrypt them all using the derived vault key from earlier. The application will then list all passwords in plain text together with their corresponding website-name for easy access. 
