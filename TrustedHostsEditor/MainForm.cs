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
using System.ServiceProcess;
using System.Net.Sockets;
using System.IO;
using TrustedHostsEditor;

namespace TrustedHosts_Editor
{
    public partial class MainForm : Form
    {
        string[] backupList = { };
        ContextMenuStrip listboxContextMenu;

        bool closeWhenFinished = false;
        bool closeVerified = false;

        bool justStarted = true;

        public MainForm()
        {
            InitializeComponent();
            listboxContextMenu = new ContextMenuStrip();
            listboxContextMenu.Opening += new CancelEventHandler(listboxContextMenu_Opening);
            Hostnames_ListBox.ContextMenuStrip = listboxContextMenu;
        }

        string StringToBool(bool value, string falseValue, string trueValue)
        {
            if (value)
            {
                return trueValue;
            }
            else return falseValue;
        }

        void RemoveSelected()
        {
            // Remove selected item from list
            if (!IsSelectedItemNull())
            {
                Hostnames_ListBox.Items.RemoveAt(Hostnames_ListBox.SelectedIndex);
            }
        }

        private void listboxContextMenu_Opening(object sender, CancelEventArgs e)
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
                AddHostname addEntry = new AddHostname(Hostnames_ListBox.SelectedItem.ToString());
                if (addEntry.ShowDialog() == DialogResult.OK)
                {
                    // Check if entry already exists 
                    // case insensitive
                    if (Hostnames_ListBox.Items.Contains(addEntry.Hostname.ToLower()) || Hostnames_ListBox.Items.Contains(addEntry.Hostname.ToUpper()))
                    {
                        MessageBox.Show(String.Format("Hostname {0} already exists.", addEntry.Hostname), "Hostname already exists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        Hostnames_ListBox.Items[index] = addEntry.Hostname;
                    }
                }
            };

            ToolStripMenuItem removeItem = new ToolStripMenuItem("Remove");
            removeItem.Click += (o, r) =>
            {
                RemoveSelected();
            };

            ToolStripMenuItem testHost = new ToolStripMenuItem("Test Host");
            testHost.Click += (o, r) =>
            {
                String Hostname = Hostnames_ListBox.SelectedItem.ToString();
                if (!(Uri.CheckHostName(Hostname) == UriHostNameType.Unknown))
                {
                    try
                    {
                        IPHostEntry entry = Dns.GetHostEntry(Hostname);
                        StringBuilder @string = new StringBuilder();
                        @string.AppendLine(String.Format("Hostname: {0}", entry.HostName));
                        @string.AppendLine();
                        List<IPAddress> ipAddresses = new List<IPAddress>(entry.AddressList);

                        List<IPAddress> ipv4_list = ipAddresses.FindAll(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                        List<IPAddress> ipv6_list = ipAddresses.FindAll(ip => ip.AddressFamily == AddressFamily.InterNetworkV6);

                        string ipv4_prefix = "";
                        string ipv6_prefix = "";

                        if (ipv4_list.Count > 1)
                        {
                            ipv4_prefix = "\n";
                        }

                        if (ipv6_list.Count > 1)
                        {
                            ipv6_prefix = "\n";
                        }
                        //string ipv4 = String.Join(", ", ipv4_list);
                        //string ipv6 = String.Join(", ", ipv6_list);

                        string ipv4 = String.Join("\n", ipv4_list);
                        string ipv6 = String.Join("\n", ipv6_list);

                        @string.AppendLine(String.Format("IPv4 Address{1}: {0}", ipv4, StringToBool((ipv4_list.Count > 1), "", "es"), ipv4_prefix));
                        if (ipv4_list.Count > 1) { @string.AppendLine(); }
                        @string.AppendLine(String.Format("IPv6 Address{1}: {0}", ipv6, StringToBool((ipv6_list.Count > 1), "", "es"), ipv6_prefix));

                        MessageBox.Show(@string.ToString(), String.Format("{0} details", Hostname), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show(String.Format("Could not communicate with host {0}.", Hostname), "Host not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(String.Format("Could not look up host because hostname {0} is invalid.", Hostname), "Hostname invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            ToolStripMenuItem saveItem = new ToolStripMenuItem("Update TrustedHosts");
            saveItem.Click += (o, r) =>
            {
                backgroundWorker_setTrustedHosts.RunWorkerAsync();
            };


            if (!IsSelectedItemNull())
            {
                listboxContextMenu.Items.Add(copyItem);
                listboxContextMenu.Items.Add(editItem);
                listboxContextMenu.Items.Add(removeItem);
                listboxContextMenu.Items.Add(new ToolStripSeparator());
                listboxContextMenu.Items.Add(testHost);
            }
            else
            {
                listboxContextMenu.Items.Add(saveItem);
            }
        }

        bool EntryExists(string entry)
        {
            if (Hostnames_ListBox.Items.Contains(entry.ToLower()) || Hostnames_ListBox.Items.Contains(entry.ToUpper()))
            {
                return true;
            }
            else return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddHostname addEntry = new AddHostname();
            if (addEntry.ShowDialog() == DialogResult.OK)
            {
                //Check if entry already exists (not case sensitive)
                if (EntryExists(addEntry.Hostname))
                {
                    MessageBox.Show(String.Format("Hostname {0} already exists.", addEntry.Hostname), "Hostname already exists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Hostnames_ListBox.Items.Add(addEntry.Hostname);
                }
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

        List<String> getTrustedHosts()
        {
            List<String> trustedHosts = new List<String>();
            using (PowerShell powerShell = PowerShell.Create().AddScript(@"Get-Item WSMan:\localhost\Client\TrustedHosts"))
            {
                Collection<PSObject> output = powerShell.Invoke();
                string val = "";

                //foreach (PSObject root in output)
                //{
                //    foreach (PSPropertyInfo info in root.Properties)
                //    {
                //        if (info.Name.Contains("TrustedHosts"))
                //        {
                //            val = (string)info.Value;
                //        }
                //    }
                //}

                val = (string)output.ElementAt(0).Properties.ElementAt(7).Value;
                // Debug.WriteLine(val);
                foreach (string item in val.Split(','))
                {
                    // If string is not empty
                    if (item.Length > 0)
                    {
                        trustedHosts.Add(item);
                    }
                }
            }
            return trustedHosts;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RemoveSelected();
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
            ServiceController service = new ServiceController("WinRM");
            if (service.Status != ServiceControllerStatus.Running)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ToolStrip_ShowStatus("Starting WinRM service...");
                });
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
            this.Invoke((MethodInvoker)delegate
            {
                panel1.Enabled = false;
                ToolStrip_ShowStatus("Getting TrustedHosts...");
            });
            List<String> hosts = getTrustedHosts();
            this.Invoke((MethodInvoker)delegate
            {
                RefreshList(hosts.ToArray());
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
            Thread.Sleep(100);
            saveTrustedHosts(entries);
        }

        private void backgroundWorker_readTrustedHosts_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (justStarted)
            {
                List<ServerInfo> servers;
                if (ServerListParser.GetServers(out servers))
                {
                    List<string> UntrustedHosts = new List<string>();
                    foreach (ServerInfo server in servers)
                    {
                        //if (!Hostnames_ListBox.Items.Contains(server.Name) && server.Status == false)
                        if (!Hostnames_ListBox.Items.Contains(server.Name))
                        {
                            UntrustedHosts.Add(server.Name);
                        }
                    }
                    if (UntrustedHosts.Count > 0)
                    {
                        StringBuilder @string = new StringBuilder();
                        @string.AppendLine(String.Format("{0} found registered with Server Manager that {1} not exist in TrustedHosts.", StringToBool(UntrustedHosts.Count > 1, "A server was", "Servers were"), StringToBool(UntrustedHosts.Count > 1, "does", "do")));
                        @string.AppendLine();
                        @string.AppendLine("Server(s):");
                        foreach (string host in UntrustedHosts)
                        {
                            @string.AppendLine(host);
                        }
                        @string.AppendLine();
                        @string.AppendLine(String.Format("Would you like to add {0}?", StringToBool(UntrustedHosts.Count > 1, "it", "them")));

                        DialogResult result = MessageBox.Show(@string.ToString(), "Servers detected from Server Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            foreach (string host in UntrustedHosts)
                            {
                                Hostnames_ListBox.Items.Add(host);
                            }
                            backgroundWorker_setTrustedHosts.RunWorkerAsync();
                        }
                    }
                }
            }
            justStarted = false;
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
            catch
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
                    // if (Hostnames_ListBox.ContextMenuStrip == null)
                    // {
                    //     Hostnames_ListBox.ContextMenuStrip = listboxContextMenu;
                    // }
                    // Hostnames_ListBox.ContextMenuStrip.Close();
                    listboxContextMenu.Show();
                }
                else
                {
                    //Hostnames_ListBox.ContextMenuStrip = null;
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
                        backupList = current;
                        closeWhenFinished = true;
                        backgroundWorker_setTrustedHosts.RunWorkerAsync();
                    }
                    else
                    {
                        e.Cancel = false;
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