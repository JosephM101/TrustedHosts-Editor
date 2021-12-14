using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace TrustedHosts_Editor
{
    public partial class AddHostname : Form
    {
        public AddHostname()
        {
            InitializeComponent();
            okButton.Enabled = false;
            //textBox1.Focus();
            UpdateV();
        }

        public AddHostname(string entry)
        {
            InitializeComponent();
            Hostname_textBox.Text = entry;
            okButton.Enabled = false;
            this.Text = "Edit Entry";
            UpdateV();
            //textBox1.Focus();
        }

        public string Hostname;

        bool Hostname_HasInvalidCharacters(string hostname)
        {
            // Regex (Windows hostnames)
            // string filter = @"^[a-zA-Z0-9][a-zA-Z0-9-]{0,61}[a-zA-Z0-9]$";
            // if (System.Text.RegularExpressions.Regex.IsMatch(hostname, filter))
            // {
            //     return false;
            // }
            // else
            // {
            //     return true;
            // }

            return (Uri.CheckHostName(hostname) == UriHostNameType.Unknown);
        }

        void UpdateV()
        {
            Hostname = Hostname_textBox.Text;
            if (Hostname_textBox.Text.Length < 1)
            {
                okButton.Enabled = false;
                testHostnameButton.Enabled = false;
            }
            else
            {
                if (Hostname_HasInvalidCharacters(Hostname))
                {
                    okButton.Enabled = false;
                    testHostnameButton.Enabled = false;
                }
                else
                {
                    okButton.Enabled = true;
                    testHostnameButton.Enabled = true;
                }
            }
            label_IpAddress.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateV();
        }

        private void AddEntry_Load(object sender, EventArgs e)
        {

        }

        private void testHostnameButton_Click(object sender, EventArgs e)
        {
            try
            {
                Hostname = Hostname_textBox.Text;
                if (!Hostname_HasInvalidCharacters(Hostname))
                {
                    IPHostEntry entry = Dns.GetHostEntry(Hostname);
                    Hostname_textBox.Text = entry.HostName;
                    label_IpAddress.Visible = true;
                    //List<string> ipAddresses = new List<string>();
                    //label_IpAddress.Text = String.Format("IP Address: {0}", String.Join(", ", ipAddresses));

                    //string ipv4 = entry.AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork));
                    List<IPAddress> ipAddresses = new List<IPAddress>(entry.AddressList);
                    string ipv4 = String.Join(", ", ipAddresses.FindAll(ip => ip.AddressFamily == AddressFamily.InterNetwork));
                    string ipv6 = entry.AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetworkV6).ToString();
                    label_IpAddress.Text = String.Format("IP Address: {0}", ipv4);
                    toolTip.SetToolTip(this.label_IpAddress, String.Format("IPv6 Address: {0}", ipv6));
                }
            }
            catch
            {
                MessageBox.Show("The host could not be contacted.", "Host not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}