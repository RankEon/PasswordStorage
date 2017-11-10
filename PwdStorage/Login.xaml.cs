using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Security.Cryptography;
using System.Security;
using System.Runtime.InteropServices;

namespace PwdStorage
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private int loginCount = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Login()
        {
            InitializeComponent();
            loginCount = 0;
        }

        /// <summary>
        /// Event handler for closing the dialog.
        /// </summary>
        private void OnLoginDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // <no functionality>
        }

        /// <summary>
        /// Handles login.
        /// </summary>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (loginCount >= 3)
                {
                    lblLoginMessage.Content = "Logins exceeded, exit!";
                    Application.Current.Shutdown();
                }

                loginCount++;

                if (tbPassword.Password.Length > 0)
                {
                    ProgramData.ReadPwHash();

                    byte[] storedHash = ProgramData.GetPwHash();
                    byte[] salt = ProgramData.GetPwSalt();


                    var password = new SecureString();

                    foreach (char ch in tbPassword.Password.ToCharArray())
                    {
                        password.AppendChar(ch);
                    }

                    // Securestring to string marshaling (example from: http://www.csharpdeveloping.net/Snippet/how_to_convert_securestring_to_string)
                    var rfc2898 = new Rfc2898DeriveBytes(Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password)), salt, 10000);
                    byte[] pwHash = rfc2898.GetBytes(16);

                    byte[] hashBytes = new byte[32];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(pwHash, 0, hashBytes, 16, 16);

                    if (ProgramData.ComparePwd(hashBytes))
                    {
                        lblLoginMessage.Content = "Access OK";
                        this.Close();
                    }
                    else
                    {
                        lblLoginMessage.Content = "Please enter a valid password!";
                    }
                }
                else
                {
                    lblLoginMessage.Content = "Please enter a valid password!";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Handles Exit-button click.
        /// </summary>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Handles left mouse button to enable moving of frameless dialog.
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Handles down key.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
