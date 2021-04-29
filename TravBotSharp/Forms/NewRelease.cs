using System;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Forms
{
    public partial class NewRelease : Form
    {
        public NewRelease()
        {
            InitializeComponent();
        }

        private readonly string[] message = {
            "You're up to date",
            "New version is available"
        };

        private readonly string discordUrl = "https://discord.gg/mBa4f2K";

        private Version currentVersion;

        public Version CurrentVersion

        {
            get { return currentVersion; }
            set
            {
                currentVersion = value;
                textBox2.Text = $"Current: {currentVersion}";
            }
        }

        private Version latestVersion;

        public Version LatestVersion
        {
            get { return latestVersion; }
            set
            {
                latestVersion = value;
                textBox3.Text = $"Latest version: {latestVersion}";
            }
        }

        private Version latestBuild;

        public Version LatestBuild
        {
            get { return latestBuild; }
            set
            {
                latestBuild = value;
                textBox4.Text = $"Latest build: {latestBuild}";

                if (latestBuild == latestVersion || currentVersion.CompareTo(latestBuild) >= 0)
                {
                    button3.Enabled = false;
                }
            }
        }

        private bool isNewVersion;

        public bool IsNewVersion

        {
            get { return isNewVersion; }
            set
            {
                isNewVersion = value;
                textBox1.Text = message[isNewVersion ? 1 : 0];

                if (!isNewVersion)
                {
                    button2.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Discord
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(discordUrl);
        }

        /// <summary>
        /// Download latest version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://github.com/{GithubHelper.username}/{GithubHelper.repo}/releases/tag/{latestVersion}");
            this.Close();
        }

        /// <summary>
        /// Download latest build
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://github.com/{GithubHelper.username}/{GithubHelper.repo}/releases/tag/{latestBuild}");
            this.Close();
        }
    }
}