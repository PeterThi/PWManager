# PWManager

For this to work, we will require a login screen for the user. Inputting a valid user will return a list of that user's passwords, in the shape of a dictionary, with the key being whatever website that password is associated with.
Input will be authenticated by hashing the input using the same parameters as the stored hash of the password in the database.


Next up, the user must be able to create a new random password. The program will generate this password using three random words from a dictionary-like object, which combines to form the password. A website is inputted as the key.
