﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Octokit;

namespace Centrifuge.Installer
{
    public partial class MainForm : Form
    {
        private readonly GitHubClient _githubClient;
        private Release _latestRelease;
        private string _gameDataDir;
        private GslFinder _gslFinder;

        public MainForm()
        {
            InitializeComponent();

            _githubClient = new GitHubClient(new ProductHeaderValue("Centrifuge.Installer", "1.0"));
            _gslFinder = new GslFinder();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            _toolStripLabel.Text = "Fetching GSL definitions...";

            _toolStripLabel.Text = "Latest release: retrieving...";
            
            SetBusyState(true);
            _latestRelease = await _githubClient.Repository.Release.GetLatest("Ciastex", "Centrifuge");
            SetBusyState(false);

            _toolStripLabel.Text = $"Latest release: {_latestRelease.Name}";
        }

        private void BrowseGameButton_Click(object sender, EventArgs e)
        {
            _folderBrowserDialog.ShowNewFolderButton = false;
            _folderBrowserDialog.Description = "Browse for the game directory (the one containing _Data directory)...";

            var result = _folderBrowserDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                _pathTextBox.Text = _folderBrowserDialog.SelectedPath;
            }
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            SetBusyState(true);

            try
            {
                var targetPath = Path.Combine(Path.GetTempPath(), "centrifuge.zip");

                ReleaseAsset asset;
                if (dotnetStandardCheckBox.Checked)
                {
                    asset = _latestRelease.Assets.First(x => x.Name.Contains("netstandard"));
                }
                else
                {
                    asset = _latestRelease.Assets.First(x => x.Name.Contains("net35"));
                }

                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(asset.BrowserDownloadUrl, targetPath);
                }

                using (var zipArchive = ZipFile.OpenRead(targetPath))
                {
                    foreach (var entry in zipArchive.Entries)
                    {
                        var filePath = Path.Combine(_gameDataDir, entry.FullName);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }

                    zipArchive.ExtractToDirectory(Path.Combine(_pathTextBox.Text, _gameDataDir));
                }

                await _gslFinder.TryFetchGSL(
                    Path.Combine(
                        _gameDataDir,
                        "Managed",
                        "Assembly-CSharp.dll"
                    ), _gameDataDir
                );

                var spindleProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        Arguments = "-t Assembly-CSharp.dll -s Centrifuge.dll",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = Path.Combine(_gameDataDir, "Managed"),
                        FileName = Path.Combine(_gameDataDir, "Managed", "Spindle.exe")
                    }
                };

                await Task.Run(() =>
                {
                    spindleProcess.Start();
                    spindleProcess.WaitForExit();
                });

                if (spindleProcess.ExitCode != 0)
                {
                    MessageBox.Show(this, $"Spindle terminated with exit code {spindleProcess.ExitCode}: ({(TerminationReason)spindleProcess.ExitCode})", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(this, "Looks like Centrifuge is now installed, and nothing exploded yet!\nLaunch the game now, see if it works.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetBusyState(false);
        }

        private void SetBusyState(bool busy)
        {
            if (busy)
            {
                _mainPanel.Enabled = false;
                _toolStripProgressBar.Visible = true;
            }
            else
            {
                _mainPanel.Enabled = true;
                _toolStripProgressBar.Visible = false;
            }
        }

        private void PathTextBox_TextChanged(object sender, EventArgs e)
        {
            _installButton.Enabled = ValidateGameDirectory(_pathTextBox.Text, out _gameDataDir);
        }

        private bool ValidateGameDirectory(string path, out string dirOfInterest)
        {
            try
            {
                var dirs = Directory.GetDirectories(path);
                dirOfInterest = dirs.FirstOrDefault(x => x.EndsWith("_Data"));

                return !string.IsNullOrWhiteSpace(path) && dirOfInterest != null;
            }
            catch
            {
                dirOfInterest = string.Empty;
                return false;
            }
        }

        private void CheckBoxIknowWhatImDoing_CheckedChanged(object sender, EventArgs e)
        {
            _pathTextBox.ReadOnly = !_checkBoxEnableFreeTextInput.Checked;
        }
    }
}