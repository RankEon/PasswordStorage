using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
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
    /// Interaction logic for ChangePwd.xaml
    /// </summary>
    public partial class ChangePwd : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ChangePwd()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for password change.
        /// </summary>
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentPw = new SecureString();
                var newPwOne = new SecureString();
                var newPwTwo = new SecureString();

                foreach (char ch in pwBoxOldPassword.Password.ToCharArray())
                {
                    currentPw.AppendChar(ch);
                }

                byte[] pwHash = ProgramData.CreatePasswordHash(currentPw);

                if (ProgramData.ComparePwd(pwHash))
                {
                    foreach (char ch in pwBoxNewPassword.Password.ToCharArray())
                    {
                        newPwOne.AppendChar(ch);
                    }

                    foreach (char ch in pwBoxReTypeNewPassword.Password.ToCharArray())
                    {
                        newPwTwo.AppendChar(ch);
                    }

                    if (pwBoxNewPassword.Password.Equals(pwBoxReTypeNewPassword.Password))
                    {
                        if (ProgramData.ReHashFiles(newPwOne))
                        {
                            MessageBox.Show("Password changed", "Information");
                            this.Close();
                        }
                    }
                    else
                    {
                        lblMessage.Content = "Passwords do not match!";
                    }
                }
                else
                {
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception:\n" + ex.Message, "Error");
            }
        }
    }
}
