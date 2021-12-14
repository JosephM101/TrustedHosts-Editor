using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Automation;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;
using System.Net;

namespace TrustedHosts_Editor
{
    public partial class Form1 : Form
    {
        string[] backupList = { };
        ContextMenuStrip listboxContextMenu;

        bool closeWhenFinished = false;
        bool closeVerified = false;

        public Form1()
        {
            InitializeComponent();
            listboxContextMenu = new ContextMenuStrip();
            listboxContextMenu.Opening += new CancelEventHandler(listboxContextMenu_Opening);
            Hostnames_ListBox.ContextMenuStrip = listboxContextMenu;
        }

        private void listboxContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (Hostnames_ListBox.SelectedItems.Count > 0)
            {
                listboxContextMenu.Items.Clear();
                ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy");
                copyItem.Click += (o, r) =>
                {
                    Clipboard.SetText(Hostnames_ListBox.SelectedItem.ToString());
                };

                ToolStripMenuItem editItem = new ToolStripMenuItem("Edit");
                editItem.Click += (o, r) =>
                {
                    int index = Hostnames_ListBox.SelectedIndex;
                    AddEntry addEntry = new AddEntry(Hostnames_ListBox.SelectedItem.ToString());
                    if (addEntry.ShowDialog() == DialogResult.OK)
                    {
                        Hostnames_ListBox.Items[index] = addEntry.Hostname;
                    }
                };

                ToolStripMenuItem testHost = new ToolStripMenuItem("Test Host");
                editItem.Click += (o, r) =>
                {
                    // Insert code to connect to the specified host and verify it exists
                };

                listboxContextMenu.Items.Add(copyItem);
                listboxContextMenu.Items.Add(editItem);
                listboxContextMenu.Items.Add(new ToolStripSeparator());
                listboxContextMenu.Items.Add(testHost);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddEntry addEntry = new AddEntry();
            if (addEntry.ShowDialog() == DialogResult.OK)
            {
                Hostnames_ListBox.Items.Add(addEntry.Hostname);
            }
        }

        void RefreshList(string[] array)
        {
            Hostnames_ListBox.Items.Clear();
            foreach (string s in array)
            {
                Hostnames_ListBox.Items.Add(s);
            }
            backupList = array;
        }

        String[] getTrustedHosts()
        {
            String[] temp = { };
            using (PowerShell powerShell = PowerShell.Create().AddScript(@"Get-Item WSMan:\localhost\Client\TrustedHosts"))
            {
                Collection<PSObject> output = powerShell.Invoke();
                string val = "";
                //foreach (PSPropertyInfo info in output.ElementAt(0).Properties)
                //{
                //    if(info.Name.Contains("TrustedHosts"))
                //    {
                //        val = (string)info.Value;
                //    }
                //}
                val = (string)output.ElementAt(0).Properties.ElementAt(7).Value;
                Debug.WriteLine(val);
                temp = val.Split(',');
            }
            return temp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Remove selected item from list
            if (!IsSelectedItemNull())
            {
                Hostnames_ListBox.Items.RemoveAt(Hostnames_ListBox.SelectedIndex);
            }
        }

        bool saveTrustedHosts(string[] entries)
        {
            String temp = String.Join(",", entries).Replace("\\", "").Replace("//", "");
            using (PowerShell powerShell = PowerShell.Create().AddScript(String.Format(@"Set-Item WSMan:\localhost\Client\TrustedHosts -Value {1}{0}{1} -Force", temp, "\"")))
            {
                powerShell.Invoke();
                // Check if something went wrong
                if (powerShell.Streams.Error.Count() > 0)
                {
                    return false;
                }
                else
                {
                    backupList = entries;
                    return true;
                }
            }
        }

        void HideToolStripItems()
        {
            toolStripStatusLabel1.Visible = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            toolStripStatusLabel1.Text = "";
        }

        void ToolStrip_ShowStatus(string status)
        {
            toolStripStatusLabel1.Visible = true;
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripStatusLabel1.Text = status;
        }

        void Finished()
        {
            this.Invoke((MethodInvoker)delegate
            {
                panel1.Enabled = true;
                HideToolStripItems();
            });
        }

        private void backgroundWorker_readTrustedHosts_doWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(100);
            this.Invoke((MethodInvoker)delegate
            {
                panel1.Enabled = false;
                ToolStrip_ShowStatus("Getting TrustedHosts...");
            });
            string[] hosts = getTrustedHosts();
            this.Invoke((MethodInvoker)delegate
            {
                RefreshList(hosts);
            });
        }

        private void backgroundWorker_setTrustedHosts_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] entries = { };
            this.Invoke((MethodInvoker)delegate
            {
                panel1.Enabled = false;
                ToolStrip_ShowStatus("Saving TrustedHosts...");
                entries = Hostnames_ListBox.Items.Cast<string>().ToArray();
            });
            Thread.Sleep(1000);
            saveTrustedHosts(entries);
        }

        private void backgroundWorker_readTrustedHosts_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Finished();
        }

        private void backgroundWorker_setTrustedHosts_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (closeWhenFinished)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Close();
                });
            }
            else
            {
                Finished();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove all entries from TrustedHosts? This cannot be undone!", "Clear TrustedHosts", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Hostnames_ListBox.Items.Clear();
                backgroundWorker_setTrustedHosts.RunWorkerAsync();
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (Hostnames_ListBox.SelectedItems.Count > 0)
                {
                    Clipboard.SetText(Hostnames_ListBox.SelectedItem.ToString());
                }
            }
        }

        bool IsSelectedItemNull()
        {
            bool isNull = false;
            try
            {
                isNull = (Hostnames_ListBox.SelectedItem == null);
                isNull = (Hostnames_ListBox.SelectedIndex < 0);
                //isNull = false;
            }
            catch (Exception ex)
            {
                isNull = true;
            }
            return isNull;
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //select the item under the mouse pointer
                Hostnames_ListBox.SelectedIndex = Hostnames_ListBox.IndexFromPoint(e.Location);
                if (!IsSelectedItemNull())
                {
                    if (Hostnames_ListBox.ContextMenuStrip == null)
                    {
                        Hostnames_ListBox.ContextMenuStrip = listboxContextMenu;
                    }
                    //Hostnames_ListBox.ContextMenuStrip.Close();
                    //listboxContextMenu.Show();
                }
                else
                {
                    Hostnames_ListBox.ContextMenuStrip = null;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker_readTrustedHosts.IsBusy)
            {
                e.Cancel = true;
            }
            else if (!closeVerified)
            {
                string[] current = Hostnames_ListBox.Items.Cast<string>().ToArray();

                if (!current.SequenceEqual(backupList))
                {
                    // Changes were made
                    e.Cancel = true;

                    DialogResult dialogResult = MessageBox.Show("Save changes?", "Save?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        closeWhenFinished = true;
                        backgroundWorker_setTrustedHosts.RunWorkerAsync();
                    }
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker_readTrustedHosts.RunWorkerAsync();
        }
    }
}