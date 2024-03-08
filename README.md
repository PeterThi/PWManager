# PWManager

First of all, we will need the ability to create a new user. I have chosen to do so with a username and password to unlock. The flow looks like this:

![CreateUserPwManager](https://github.com/PeterThi/PWManager/assets/60512162/9fceefd9-99d9-4798-a709-4a9ac2a081e8)

Username, vaultkey and salt i stored in the database for later authentication.

Then we will require a login screen for the user. Inputting a valid user will return a list of that user's passwords, in the shape of a dictionary, with the key being whatever website that password is associated with.
Input will be authenticated by hashing the input using the same salt and iterations as the stored hash of the password in the database.

![LoginPwManager](https://github.com/PeterThi/PWManager/assets/60512162/6d4a1085-a2e8-4f78-8d97-0baea0c2c4b8)


Next up, the user must be able to create a new random password. The program will generate this password using three random words from a dictionary-like object, which combines to form the password. A website (inputted string, arbitrary) is inputted as the key.

