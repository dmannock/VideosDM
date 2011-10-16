using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.XPath;

using NUtility;

using VideosDM.Core;

namespace VideosDM
{
    public partial class SettingsForm : SettingsPage
    {
        private readonly VideosSettings Settings;

        public SettingsForm()
        {
            InitializeComponent();
            Settings = Config.ReadConfig();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            TextBoxFolderPath.Text = Settings.Path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                TextBoxFolderPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void TextBoxFolderPath_TextChanged(object sender, EventArgs e)
        {
            if (TextBoxFolderPath.Text.Length == 0 || !System.IO.Directory.Exists(TextBoxFolderPath.Text)) {
                TextBoxFolderPath.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
                return;
            }

            TextBoxFolderPath.BackColor = System.Drawing.SystemColors.Window;
            Config.SaveConfig(new VideosSettings { 
                Path = TextBoxFolderPath.Text 
            });
        }

        public override void  SaveSettings()
        {
 	        base.SaveSettings();
        }
    }
}
