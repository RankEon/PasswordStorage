# PasswordStorage
An application for storing password for various websites and services. Typically the users have multiple usernames and passwords for several or even tens of websites or services. This application has been developed for my personal use to store the passwords into on place.

The main characteristics of the application are:
 - Upon starting the application for the first time, the user is asked to create a master password.
   The password is then encrypted with Rijndael –algorithm (RijndaelManaged –class) and the hash + salt is stored to disk (same directory with the application) hash + salt –file is encrypted again with ProtectedData –class which bounds the file to the current username.
- In the main view, the user can add several Site/Service, Username and password combinations.
- The password list is stored to the disk (same directory with the application) and encrypted with Rijndael 
  –algorithm (included in .NET).