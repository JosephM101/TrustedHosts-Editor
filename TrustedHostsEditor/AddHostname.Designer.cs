﻿namespace TrustedHosts_Editor
{
    partial class AddHostname
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Hostname_textBox = new System.Windows.Forms.TextBox();
            this.testHostnameButton = new System.Windows.Forms.Button();
            this.label_IpAddress = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(257, 88);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(63, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(324, 88);
            this.okButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(64, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Hostname";
            // 
            // Hostname_textBox
            // 
            this.Hostname_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Hostname_textBox.Location = new System.Drawing.Point(12, 27);
            this.Hostname_textBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Hostname_textBox.MaxLength = 63;
            this.Hostname_textBox.Name = "Hostname_textBox";
            this.Hostname_textBox.Size = new System.Drawing.Size(376, 20);
            this.Hostname_textBox.TabIndex = 0;
            this.Hostname_textBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.Hostname_textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Hostname_textBox_KeyDown);
            // 
            // testHostnameButton
            // 
            this.testHostnameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.testHostnameButton.Location = new System.Drawing.Point(10, 88);
            this.testHostnameButton.Name = "testHostnameButton";
            this.testHostnameButton.Size = new System.Drawing.Size(93, 23);
            this.testHostnameButton.TabIndex = 4;
            this.testHostnameButton.Text = "Test Hostname";
            this.testHostnameButton.UseVisualStyleBackColor = true;
            this.testHostnameButton.Click += new System.EventHandler(this.testHostnameButton_Click);
            // 
            // label_IpAddress
            // 
            this.label_IpAddress.AutoSize = true;
            this.label_IpAddress.Location = new System.Drawing.Point(12, 53);
            this.label_IpAddress.Name = "label_IpAddress";
            this.label_IpAddress.Size = new System.Drawing.Size(61, 13);
            this.label_IpAddress.TabIndex = 5;
            this.label_IpAddress.Text = "IP Address:";
            this.label_IpAddress.Visible = false;
            // 
            // AddHostname
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(399, 121);
            this.ControlBox = false;
            this.Controls.Add(this.label_IpAddress);
            this.Controls.Add(this.testHostnameButton);
            this.Controls.Add(this.Hostname_textBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddHostname";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Hostname";
            this.Load += new System.EventHandler(this.AddEntry_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Hostname_textBox;
        private System.Windows.Forms.Button testHostnameButton;
        private System.Windows.Forms.Label label_IpAddress;
        private System.Windows.Forms.ToolTip toolTip;
    }
}