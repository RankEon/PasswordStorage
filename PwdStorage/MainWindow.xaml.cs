using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.IO;
using System.Security;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;

namespace PwdStorage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Collection of credential information, which is bind to datagrid.
        public ObservableCollection<CredentialInfo> CredentialList = new ObservableCollection<CredentialInfo>();

        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            dataGridPasswords.ItemsSource = CredentialList;
        }

        /// <summary>
        /// Stores the password data to a file.
        /// </summary>
        public void StorePwData()
        {
            try
            {
                List<string> passdata = new List<string>();

                foreach (CredentialInfo item in CredentialList)
                {
                    string siteName = item.Site;

                    // Remove unwanted characters from the names
                    if (siteName.Contains("ä"))
                        siteName = siteName.Replace('ä', 'a');
                    if (siteName.Contains("ö"))
                        siteName = siteName.Replace('ö', 'o');
                    if (siteName.Contains("å"))
                        siteName = siteName.Replace('å', 'a');

                    passdata.Add(siteName + "\t" + item.Username + "\t" + item.Password + "\n");
                }

                List<byte[]> passBytes = new List<byte[]>();

                foreach (string text in passdata)
                {
                    byte[] byteArr;
                    UTF8Encoding enc = new UTF8Encoding();
                    byteArr = enc.GetBytes(text);
                    passBytes.Add(byteArr);
                }

                // Get salt+key
                byte[] salt = ProgramData.GetPwSalt();
                byte[] key = ProgramData.GetPwHash();

                string cryptedFile = @".\passdata.dat";

                // Write and encrypt file.
                FileStream fsCryptedfile = new FileStream(cryptedFile, FileMode.Create);
                RijndaelManaged RMCrypto = new RijndaelManaged();
                CryptoStream cryptoStream = new CryptoStream(fsCryptedfile, RMCrypto.CreateEncryptor(key, salt), CryptoStreamMode.Write);

                foreach (byte[] data in passBytes)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        cryptoStream.WriteByte(data[i]);
                    }
                }

                cryptoStream.Close();
                fsCryptedfile.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error:\n" + e.Message, "Error");
            }
        }

        /// <summary>
        /// Loads stored password storage file from the disk.
        /// </summary>
        private void LoadPwData()
        {
            BackgroundWorker loadDataWorker = new BackgroundWorker();
            loadDataWorker.WorkerReportsProgress = true;
            loadDataWorker.DoWork += bgWorkerLoadPwData_DoWork;
            loadDataWorker.ProgressChanged += bgWorker_ProgressChanged;
            loadDataWorker.RunWorkerAsync();
        }

        private void bgWorkerLoadPwData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int progressPct = 1;

                this.Dispatcher.BeginInvoke((Action)delegate { spProgressIndicator.Visibility = Visibility.Visible; });
                ((BackgroundWorker)sender).ReportProgress(progressPct);

                // Get salt+key
                byte[] salt = ProgramData.GetPwSalt();
                byte[] key = ProgramData.GetPwHash();

                FileStream fsDecrypt = new FileStream(@".\passdata.dat", FileMode.Open);
                RijndaelManaged RMDeCrypt = new RijndaelManaged();

                CryptoStream deCryptStream = new CryptoStream(fsDecrypt, RMDeCrypt.CreateDecryptor(key, salt), CryptoStreamMode.Read);

                int character;

                List<string> pwList = new List<string>();
                StringBuilder sbLine = new StringBuilder();

                // Read from decrypt stream
                while ((character = deCryptStream.ReadByte()) != -1)
                {
                    Console.Write(char.ConvertFromUtf32(character));

                    if (char.ConvertFromUtf32(character).Equals("\n"))
                    {
                        pwList.Add(sbLine.ToString());
                        sbLine.Clear();
                        ((BackgroundWorker)sender).ReportProgress((progressPct < 100) ? progressPct++ : progressPct);
                    }
                    else
                    {
                        sbLine.Append(char.ConvertFromUtf32(character));
                    }
                }


                deCryptStream.Close();

                // Update Credential list
                this.Dispatcher.BeginInvoke((Action)delegate { CredentialList.Clear(); });

                foreach (string entry in pwList)
                {
                    string[] content = entry.Split(new char[] { '\t' });
                    this.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CredentialList.Add(new CredentialInfo { Site = content[0], Username = content[1], Password = content[2] });
                    });
                    ((BackgroundWorker)sender).ReportProgress((progressPct < 100) ? progressPct++ : progressPct);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Error");
            }
            finally
            {
                this.Dispatcher.BeginInvoke((Action)delegate { spProgressIndicator.Visibility = Visibility.Collapsed; });
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Event handler for add new password -button click
        /// </summary>
        private void btnAddNewPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CredentialList.Add(new CredentialInfo { Username = "", Password = "", Site = "" });
                dataGridPasswords.UpdateLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Error");
            }
        }

        /// <summary>
        /// Event handler for removing item from the grid.
        /// </summary>
        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedRow = dataGridPasswords.SelectedIndex;
                CredentialList.RemoveAt(selectedRow);
                dataGridPasswords.UpdateLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Error");
            }
        }

        /// <summary>
        /// Event handler for File -> Save menu option click.
        /// </summary>
        private void OnMenuItemFileSaveClick(object sender, RoutedEventArgs e)
        {
            this.StorePwData();
        }

        /// <summary>
        /// Shows the loaded password hash.
        /// </summary>
        private void OnMenuItemExportHashClick(object sender, RoutedEventArgs e)
        {
            byte[] hashBytes = new byte[32];

            hashBytes = ProtectedData.Unprotect(ProgramData.GetProtectedPwHash(), null, DataProtectionScope.CurrentUser);

            MessageBox.Show("Hash:\n\n" + String.Join(",", BitConverter.ToString(hashBytes)));
        }

        /// <summary>
        /// Event handler for chaging the password
        /// </summary>
        private void menuItemChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePwd FormChangePwd = new ChangePwd();
            FormChangePwd.Owner = this;
            FormChangePwd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            FormChangePwd.ShowDialog();
            FormChangePwd.Focus();
            FormChangePwd.Topmost = true;
        }

        /// <summary>
        /// Event hander for a search button click.
        /// </summary>
        private void btnSearchClick(object sender, RoutedEventArgs e)
        {
            // Creating a temporery observable collection for the search.
            ObservableCollection<CredentialInfo> temp_CredentialList = new ObservableCollection<CredentialInfo>();

            // Looping through the credential list and copying the items
            // that match to the search terms.
            for (int i = 0; i < CredentialList.Count; i++)
            {
                string username = CredentialList[i].Username.ToLower();
                string site = CredentialList[i].Site.ToLower();

                if (username.Contains(tbSearch.Text.ToString().ToLower()) ||
                    site.Contains(tbSearch.Text.ToString().ToLower()))
                {
                    temp_CredentialList.Add(CredentialList[i]);
                }
            }

            // Show the search results in the grid.
            dataGridPasswords.ItemsSource = temp_CredentialList;
        }

        /// <summary>
        /// Clears the search and restores original view to the grid.
        /// </summary>
        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Clear();
            dataGridPasswords.ItemsSource = CredentialList;
        }

        /// <summary>
        /// Event handler for search box. Initiates search when enter -key is pressed.
        /// </summary>
        private void tbSearchOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            // Check for user identification file, which contains password hash and salt.
            if (!File.Exists(@".\Userid.dat"))
            {
                CreateNewPwd FormNewPwd = new CreateNewPwd();

                // Center the dialog on top of the parent.
                FormNewPwd.Owner = this;
                FormNewPwd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                FormNewPwd.Activate();
                FormNewPwd.ShowDialog();
                CredentialList.Add(new CredentialInfo { Site = "", Username = "", Password = "" });
            }
            else
            {
                Login FormLogin = new Login();
                FormLogin.WindowStyle = WindowStyle.None;
                FormLogin.ShowDialog();
                FormLogin.Focus();
                FormLogin.Topmost = true;

                //CredentialList.Add(new CredentialInfo { Username = "x", Password = "y", Site = "z" });
                if (File.Exists(@".\passdata.dat"))
                {
                    this.LoadPwData();
                }
            }
        }
    }
}
