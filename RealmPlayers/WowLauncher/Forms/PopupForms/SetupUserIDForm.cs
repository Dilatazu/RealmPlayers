using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace VF_WoWLauncher
{
    public partial class SetupUserIDForm : Form
    {
        public SetupUserIDForm()
        {
            InitializeComponent();
            c_txtUserID.GotFocus += c_txtUserID_GotFocus;
            c_txtUserID.LostFocus += c_txtUserID_LostFocus;
        }

        void c_txtUserID_GotFocus(object sender, EventArgs e)
        {
            if (c_txtUserID.Text == "Unknown.123456")
                c_txtUserID.Text = "";
        }

        void c_txtUserID_LostFocus(object sender, EventArgs e)
        {
            if (c_txtUserID.Text == "")
                c_txtUserID.Text = "Unknown.123456";
        }

        private void c_txtUserID_TextChanged(object sender, EventArgs e)
        {

        }

        public static bool ConvertBool(string _String)
        {
            string toLower = _String.ToLower();
            if (toLower == "false" || toLower == "nil" || toLower == "0")
                return false;
            return true;
        }
        bool ValidateUserID(string _UserID)
        {
            if (_UserID.Contains("Unknown.123456") || _UserID == "")
            {
                Utility.MessageBoxShow("You must fill in the correct UserID");
                return false;
            }
            if (RealmPlayersUploader.IsValidUserID(c_txtUserID.Text) == false)
            {
                Utility.MessageBoxShow("UserID is not valid format, must be <name(a-z)>.<number> example of valid UserID: Unknown.123456");
                return false;
            }

            Socket tcpSocket = null;
            try
            {
                DateTime startConnectionTime = DateTime.Now;
                tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ipAddress = System.Net.Dns.GetHostEntry(ServerComm.g_Host).AddressList[0];
                IAsyncResult ar = tcpSocket.BeginConnect(ipAddress, 18374, null, null);
                System.Threading.WaitHandle waitHandle = ar.AsyncWaitHandle;
                try
                {
                    for (int t = 0; t < 5; ++t)
                    {
                        if (waitHandle.WaitOne(TimeSpan.FromSeconds(1), false))
                            break;
                        Application.DoEvents();
                        //Console.Write(".");
                    }
                    Console.Write("\r\n");
                    if (!waitHandle.WaitOne(TimeSpan.FromSeconds(1), false))
                    {
                        tcpSocket.Close();
                        Utility.SoftThreadSleep(1000);
                        waitHandle.Close();
                        Utility.MessageBoxShow("Could not validate UserID because connection to server failed, try again later.");
                        return false;
                    }

                    tcpSocket.EndConnect(ar);
                }
                finally
                {
                    waitHandle.Close();
                }
                tcpSocket.ReceiveTimeout = 5000;
                tcpSocket.SendTimeout = 5000;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("NullNullNullNullNullNullNullNull");
                byte[] header = System.Text.Encoding.UTF8.GetBytes("Command=UserCheck;UserID=" + _UserID + ";FileSize=" + bytes.Length + "%");
                tcpSocket.Send(header);
                tcpSocket.Send(bytes);
                tcpSocket.Shutdown(SocketShutdown.Send);
                Byte[] readBuffer = new Byte[1024];
                int i = 0;
                string data = "";
                while ((i = tcpSocket.Receive(readBuffer, readBuffer.Length, SocketFlags.None)) != 0)
                {
                    data += System.Text.Encoding.UTF8.GetString(readBuffer, 0, i);
                    if ((DateTime.Now - startConnectionTime).TotalSeconds > 10)
                    {
                        throw new Exception("Transfer took longer than 10 seconds, should not be allowed, canceling");
                    }
                }
                bool wasSuccess = false;
                if (data.Contains(";") && data.Contains("="))
                {
                    string[] dataSplit = data.Split(';');
                    foreach (string currDataSplit in dataSplit)
                    {
                        if (currDataSplit.Contains("="))
                        {
                            string[] currValue = currDataSplit.Split('=');
                            if (currValue[0] == "VF_RPP_Success")
                                wasSuccess = ConvertBool(currValue[1]);
                            else if (currValue[0] == "Message")
                                Utility.MessageBoxShow(currValue[1]);
                        }
                    }
                }
                if (wasSuccess == true)
                {
                    Utility.MessageBoxShow("The UserID \"" + _UserID + "\" was valid and is now in use! The program will now start upload your inspected database automatically everytime you close wow(if started using the WowLauncher).");
                    return true;
                }
                else
                {
                    Utility.MessageBoxShow("The UserID \"" + _UserID + "\" is not valid. Please enter the correct UserID that was given to you.");
                }
            }
            catch (Exception ex)
            {
                Utility.MessageBoxShow("Could not validate UserID because of a connection error. Printscreen this message and PM to Dilatazu @ realmplayers forums or try again later.\r\nException:\r\n" + ex.ToString());
            }
            return false;
        }
        private void c_btnValidate_Click(object sender, EventArgs e)
        {
            c_btnValidate.Enabled = false;
            if(ValidateUserID(c_txtUserID.Text) == true)
            {
                Settings.Instance._UserID = c_txtUserID.Text;
                Settings.Save();
                Close();
            }
            c_btnValidate.Enabled = true;
        }

        private void c_btnCancel_Click(object sender, EventArgs e)
        {
            c_btnSkip_Click(sender, e);
        }

        private void SetupUserIDForm_Load(object sender, EventArgs e)
        {
            Utility.SetPositionToMouse(this);
            c_txtUserID.Text = Settings.UserID;
            if(RealmPlayersUploader.IsValidUserID(c_txtUserID.Text) == false)
            {
                if (System.IO.File.Exists(Settings.GetWowDirectory(WowVersion.Vanilla) + "VF_RealmPlayersUploader\\Settings.cfg") == true)
                {
                    var oldSettings = System.IO.File.ReadAllLines(Settings.GetWowDirectory(WowVersion.Vanilla) + "VF_RealmPlayersUploader\\Settings.cfg");
                    foreach (var settingsLine in oldSettings)
                    {
                        if (settingsLine.StartsWith("UserID="))
                        {
                            c_txtUserID.Text = settingsLine.Substring("UserID=".Length);
                            Utility.MessageBoxShow("Found old installation of VF_RealmPlayersUploader, copied the UserID");
                        }
                    }
                }
            }
            //this.TopMost = true;
        }

        private void c_btnSkip_Click(object sender, EventArgs e)
        {
            if(RealmPlayersUploader.IsValidUserID(Settings.UserID) == false)
            {
                var dialogResult = Utility.MessageBoxShow("Skipping this step means contributions to realmplayers and raidstats wont work. However you can always setup UserID later in the menu File->Settings.\r\n\r\nAre you sure you want to skip configuring UserID?", "Skip configuring UserID?", MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.No)
                    return;
            }
            Close();
        }
    }

    public class SetupUserID
    {
        public static bool ShowSetupUserID()
        {
            string previousUserID = Settings.UserID;
            SetupUserIDForm setupUserIDForm = new SetupUserIDForm();
            setupUserIDForm.ShowDialog();
            return (previousUserID != Settings.UserID); //True om UserID förändrats
        }
    }
}
