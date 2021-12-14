using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrustedHosts_Editor
{
    public partial class AddEntry : Form
    {
        public AddEntry()
        {
            InitializeComponent();
            okButton.Enabled = false;
            //textBox1.Focus();
        }

        public AddEntry(string entry)
        {
            InitializeComponent();
            textBox1.Text = entry;
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
            Hostname = textBox1.Text;
            if (textBox1.Text.Length < 1)
            {
                okButton.Enabled = false;
            }
            else
            {
                if (Hostname_HasInvalidCharacters(Hostname))
                {
                    okButton.Enabled = false;
                }
                else
                {
                    okButton.Enabled = true;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateV();
        }

        private void AddEntry_Load(object sender, EventArgs e)
        {

        }
    }
}