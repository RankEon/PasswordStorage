using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PwdStorage
{
    /// <summary>
    /// Interaction logic for CreateNewPwd.xaml
    /// </summary>
    public partial class CreateNewPwd : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CreateNewPwd()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for creating a new password.
        /// </summary>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            lblMessage.Content = "";

            if (String.Equals(tbEnterPassword.Password.ToString(), tbReEnterPassword.Password.ToString()))
            {
                byte[] salt = new byte[16];

                RNGCryptoServiceProvider RngProvider = new RNGCryptoServiceProvider();
                RngProvider.GetBytes(salt);

                var rfc2898 = new Rfc2898DeriveBytes(tbEnterPassword.Password.ToString(), salt, 10000);
                byte[] pwHash = rfc2898.GetBytes(16);

                // Combine salt + hash
                byte[] hashBytes = new byte[32];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(pwHash, 0, hashBytes, 16, 16);

                // Protect (current user)
                ProgramData.protectedPwdHash = ProtectedData.Protect(hashBytes, null, DataProtectionScope.CurrentUser);

                // Save
                FileStream fs = new FileStream(ProgramData.userDataFile, FileMode.Create);

                foreach(byte data in ProgramData.protectedPwdHash)
                {
                    fs.WriteByte(data);
                }

                fs.Close();

                this.Close();
            }
            else
            {
                lblMessage.Content = "ERROR: Passwords must match!!! Please re-type.";
            }
        }
    }
}
