using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace PwdStorage
{
    /// <summary>
    /// This class contains the program data, such as the hash and utility
    /// methods for updating and reading the hash.
    /// </summary>
    public class ProgramData
    {
        public static string userDataFile = @".\Userid.dat";
        public static byte[] protectedPwdHash;

        public static void ReadPwHash()
        {
            if (userDataFile == null)
            {
                throw new FileNotFoundException("User data file not found");
            }

            if (protectedPwdHash != null && protectedPwdHash.Length > 0)
            {
                Array.Clear(protectedPwdHash, 0, protectedPwdHash.Length);
            }

            protectedPwdHash = File.ReadAllBytes(ProgramData.userDataFile);
        }

        /// <summary>
        /// Gets protected (with ProtectedData-class) password hash (including salt). 
        /// Needs to be unprotected before use.
        /// </summary>
        /// <returns>Protected password hash</returns>
        public static byte[] GetProtectedPwHash()
        {
            return protectedPwdHash;
        }

        /// <summary>
        /// Compares given hashed password (including salt) to the password hash.
        /// </summary>
        /// <param name="pwHash">Hashed password</param>
        /// <returns>True on succcess, false on non-matching password.</returns>
        public static bool ComparePwd(byte[] pwHash)
        {
            byte[] storedHash;

            storedHash = ProtectedData.Unprotect(protectedPwdHash, null, DataProtectionScope.CurrentUser);

            for (int i = 0; i < 16; i++)
            {
                if(pwHash[i+16] != storedHash[i+16])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets password salt.
        /// </summary>
        /// <returns>Salt</returns>
        public static byte[] GetPwSalt()
        {
            // Get salt+key
            byte[] storedHash = ProtectedData.Unprotect(ProgramData.protectedPwdHash, null, DataProtectionScope.CurrentUser);
            byte[] salt = new byte[16];
            Array.Copy(storedHash, 0, salt, 0, 16);
            return salt;
        }

        /// <summary>
        /// Gets password hash.
        /// </summary>
        /// <returns>Password hash</returns>
        public static byte[] GetPwHash()
        {
            // Get salt+key
            byte[] storedHash = ProtectedData.Unprotect(ProgramData.protectedPwdHash, null, DataProtectionScope.CurrentUser);
            byte[] key = new byte[16];
            Array.Copy(storedHash, 16, key, 0, 16);
            return key;
        }

        /// <summary>
        /// Creates a new password hash, with the current salt, based on the given SecureString.
        /// </summary>
        /// <param name="passwd">Password (as securestring)</param>
        /// <returns>Hash, including current salt.</returns>
        public static byte[] CreatePasswordHash(SecureString passwd)
        {
            // Securestring to string marshaling (example from: http://www.csharpdeveloping.net/Snippet/how_to_convert_securestring_to_string)
            var rfc2898 = new Rfc2898DeriveBytes(Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passwd)), GetPwSalt(), 10000);
            byte[] pwHash = rfc2898.GetBytes(16);
            byte[] fullHash = new byte[32];

            Array.Copy(GetPwSalt(), 0, fullHash, 0, 16);
            Array.Copy(pwHash, 0, fullHash, 16, 16);

            return fullHash;
        }

        /// <summary>
        /// Creates a new password hash with new salt
        /// </summary>
        /// <param name="passwd">Password (as securestring)</param>
        /// <returns>Hash, including new salt</returns>
        public static byte[] CreateNewPasswordHash(SecureString passwd)
        {
            // Create new salt
            byte[] salt = new byte[16];

            RNGCryptoServiceProvider RngProvider = new RNGCryptoServiceProvider();
            RngProvider.GetBytes(salt);

            // Securestring to string marshaling (example from: http://www.csharpdeveloping.net/Snippet/how_to_convert_securestring_to_string)
            var rfc2898 = new Rfc2898DeriveBytes(Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passwd)), salt, 10000);
            byte[] pwHash = rfc2898.GetBytes(16);
            byte[] fullHash = new byte[32];

            Array.Copy(salt, 0, fullHash, 0, 16);
            Array.Copy(pwHash, 0, fullHash, 16, 16);

            return fullHash;
        }

        /// <summary>
        /// Re-hashes and encrypts the password files.
        /// </summary>
        /// <param name="passwd">Password (as securestring)</param>
        /// <returns>True on success, throws error on failure.</returns>
        public static bool ReHashFiles(SecureString passwd)
        {
            try
            { 
                byte[] newPwHash = ProgramData.CreateNewPasswordHash(passwd);
                // Protect (current user)
                ProgramData.protectedPwdHash = ProtectedData.Protect(newPwHash, null, DataProtectionScope.CurrentUser);

                // Save
                FileStream fs = new FileStream(ProgramData.userDataFile, FileMode.Create);

                foreach (byte data in ProgramData.protectedPwdHash)
                {
                    fs.WriteByte(data);
                }

                fs.Close();

                // Save the grid with the new hash
                var mw = Application.Current.MainWindow as MainWindow;
                mw.StorePwData();
            }
            catch(Exception ex)
            {
                throw new Exception("Re-Hashing failed: " + ex.Message);
            }

            return true;
        }

    }

    /// <summary>
    /// Credential info property.
    /// </summary>
    public class CredentialInfo
    {
        public string Site { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
