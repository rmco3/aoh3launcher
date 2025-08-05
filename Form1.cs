using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;

namespace AOH3Launcher
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private static readonly HttpClient httpClient = new HttpClient();
        private string gameFilesPath;
        private string gameExecutablePath;
        private string tempZipPath;
        private bool isDownloadingOrExtracting = false; // Flag to track download/extraction/deletion state
        public bool isDownloadStarted = false; // Track if download has started
        private CancellationTokenSource _downloadCancellationTokenSource; // New: For download cancellation

        // Forms
        private UpdateNote updateNoteWindow;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private bool disableUpdateCheck = false;
        public const string CurrentVersion = "1.5"; // AOH3 Launcher Version

        public static class SystemCursors
        {
            private const int IDC_HAND = 32649;

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

            public static Cursor HandCursor => new Cursor(LoadCursor(IntPtr.Zero, IDC_HAND));
        }

        private void ApplySystemHandCursor(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c.Cursor == Cursors.Hand)
                {
                    c.Cursor = SystemCursors.HandCursor;
                }
                ApplySystemHandCursor(c);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }
        private void Form1_MouseDown_TabPage1(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }
        private void Form1_MouseDown_groupBox1(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }
        private void Form1_MouseDown_panel1(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }
        private void Form1_MouseDown_panel2(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }
        private void Form1_MouseDown_TabPage2(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }
        private void Form1_MouseDown_TabPage3(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }

        public Form1()
        {
            InitializeComponent();

            MouseDown += Form1_MouseDown;
            if (groupBox1 != null) groupBox1.MouseDown += Form1_MouseDown_groupBox1;
            if (panel1 != null) panel1.MouseDown += Form1_MouseDown_panel1;
            if (panel2 != null) panel2.MouseDown += Form1_MouseDown_panel2;
            if (tabPage1 != null) tabPage1.MouseDown += Form1_MouseDown_TabPage1;
            if (tabPage2 != null) tabPage2.MouseDown += Form1_MouseDown_TabPage2;
            if (tabPage3 != null) tabPage3.MouseDown += Form1_MouseDown_TabPage3;

            string currentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            gameFilesPath = Path.Combine(currentDirectory, "Game Files");
            gameExecutablePath = Path.Combine(gameFilesPath, "game.jar");
            tempZipPath = Path.Combine(gameFilesPath, "gameFilesTmp.zip");

            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            // Apply system hand cursor to all controls that use Cursors.Hand
            ApplySystemHandCursor(this);

            LoadSettings();
            CheckGameInstallation();

            // Automatic update check
            this.Invoke((Action)(() => { _ = UpdateCheckAsync(false); }));
        }

        private void LoadSettings()
        {
            if (checkBox1 == null || checkBox2 == null || checkBox3 == null) return;

            // Load existing settings
            checkBox1.Checked = Properties.Settings.Default.CloseLauncherAfterGameStart;
            checkBox2.Checked = Properties.Settings.Default.BoostGamePerformance;
            checkBox3.Checked = Properties.Settings.Default.DarkMode;

            // Apply theme based on saved setting
            if (checkBox3.Checked)
            {
                ApplyDarkMode();
            }
            else
            {
                ApplyLightTheme();
            }

            UpdateCheckBoxStates();
        }

        private void SaveSettings()
        {
            if (checkBox1 == null || checkBox2 == null || checkBox3 == null) return;

            Properties.Settings.Default.CloseLauncherAfterGameStart = checkBox1.Checked;
            Properties.Settings.Default.BoostGamePerformance = checkBox2.Checked;
            Properties.Settings.Default.DarkMode = checkBox3.Checked;
            Properties.Settings.Default.Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent closing if download or extraction is in progress
            if (isDownloadingOrExtracting)
            {
                MessageBox.Show("Game download/extraction/deletion process is in progress. Please do not close the application until this process is finished.", "Process in Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true; // Cancel the closing event
                return;
            }
            SaveSettings();
        }

        private void CheckGameInstallation()
        {
            if (Directory.Exists(gameFilesPath) && File.Exists(gameExecutablePath))
            {
                SetButtonStates(false, true, true);
                labelDownload.Text = "Game Ready";
                progressBar1.Value = 100;
            }
            else
            {
                SetButtonStates(true, false, false);
                labelDownload.Text = "Ready to Download";
                progressBar1.Value = 0;
            }
        }

        private async void BtnDownloadGame_Click(object sender, EventArgs e)
        {
            checkForUpdates.Enabled = false; // Disable update check button during download
            isDownloadStarted = true; // Set flag to indicate download has started

            // If already in a process (download/extract/delete), check if it's a cancellation request
            if (isDownloadingOrExtracting)
            {
                if (BtnDownloadGame.Text == "CANCEL DOWNLOAD") // User clicked to cancel
                {
                    if (MessageBox.Show("Are you sure you want to cancel the download?", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        _downloadCancellationTokenSource?.Cancel(); // Request cancellation
                        SetButtonStates(false, false, false); // Disable all buttons temporarily
                        labelDownload.Text = "Cancelling download...";
                        progressBar1.Value = 0;
                    }
                    return; // Exit, let the async operation's catch block handle the rest
                }
                return; // Prevent multiple clicks during extraction/deletion if not in CANCEL state
            }

            // --- Start Download Logic ---
            isDownloadingOrExtracting = true;
            _downloadCancellationTokenSource = new CancellationTokenSource(); // Create a new token source for this download

            // Disable all buttons but re-enable BtnDownloadGame specifically for cancellation
            SetButtonStates(false, false, false);
            BtnDownloadGame.Enabled = true;
            BtnDownloadGame.ForeColor = Color.FromArgb(192, 0, 0); // Change button color to red 
            BtnDownloadGame.Text = "CANCEL DOWNLOAD"; // Change button text to indicate cancellable state

            try
            {
                if (Directory.Exists(gameFilesPath) && File.Exists(gameExecutablePath))
                {
                    MessageBox.Show("The game is already installed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // This scenario should ideally be caught by CheckGameInstallation before this method is called,
                    // but as a safeguard, reset state and exit.
                    CheckGameInstallation();
                    return;
                }

                labelDownload.Text = "Getting download link...";
                progressBar1.Value = 0;

                string downloadUrl = await GetDownloadUrlFromGitHub();
                if (string.IsNullOrEmpty(downloadUrl))
                {
                    MessageBox.Show("Download URL could not be retrieved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Will go to finally block for cleanup
                }

                if (!Directory.Exists(gameFilesPath)) Directory.CreateDirectory(gameFilesPath);

                // Pass the cancellation token to the download method
                await DownloadGameFile(downloadUrl, _downloadCancellationTokenSource.Token);

                // If cancellation was requested during download, OperationCanceledException would have been thrown.
                // If it wasn't, confirm success.
                _downloadCancellationTokenSource.Token.ThrowIfCancellationRequested(); // Final check before extraction

                await ExtractGameFile(); // Extraction is local, less critical for cancellation mid-way
                if (File.Exists(tempZipPath)) File.Delete(tempZipPath);

                SetButtonStates(false, true, true);
                labelDownload.Text = "Download Completed!";
                progressBar1.Value = 100;

                checkForUpdates.Enabled = true; // Re-enable update check button after download 

                MessageBox.Show("Game downloaded successfully!", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Game download was cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Cleanup will be handled in the finally block
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while downloading the game. Please check your internet connection, or try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show($"Error occurred while downloading the game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // For debugging
            }
            finally
            {
                // Always reset states and clean up regardless of success or failure
                isDownloadingOrExtracting = false; // Reset flag
                BtnDownloadGame.ForeColor = Color.DodgerBlue; // Reset button color to default
                BtnDownloadGame.Text = "DOWNLOAD"; // Reset button text

                CheckGameInstallation(); // Update UI based on current installation status
                _downloadCancellationTokenSource?.Dispose(); // Dispose the token source
                _downloadCancellationTokenSource = null;
            }

            checkForUpdates.Enabled = true; // Reset update check button
        }

        private async Task<string> GetDownloadUrlFromGitHub()
        {
            try
            {
                string gitHubRawUrl = "https://raw.githubusercontent.com/rmco3/aoh3launcher/main/game.txt";
                httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true }; // Prevent caching of game.txt
                string jsonContent = await httpClient.GetStringAsync(gitHubRawUrl);
                dynamic jsonObject = JsonConvert.DeserializeObject(jsonContent);
                return jsonObject?.download_url?.ToString()?.Trim();
            }
            catch (Exception ex) { throw new Exception($"Failed to get download URL: {ex.Message}"); }
        }

        // Modified: Now accepts a CancellationToken
        private async Task DownloadGameFile(string downloadUrl, CancellationToken cancellationToken)
        {
            try
            {
                labelDownload.Text = "The game is downloading...";
                using (var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken)) // Pass token here
                {
                    response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    var downloadedBytes = 0L;
                    using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken)) // Pass token here
                    using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0) // Pass token here
                        {
                            cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation frequently during stream reading

                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken); // Pass token here
                            downloadedBytes += bytesRead;
                            if (totalBytes > 0)
                            {
                                var progressPercentage = (int)((downloadedBytes * 100) / totalBytes);
                                progressBar1.Value = Math.Min(progressPercentage, 90);
                                labelDownload.Text = $"Downloading... {progressPercentage}%";
                                Application.DoEvents(); // Ensure UI updates are processed
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw; // Re-throw the cancellation exception to be caught by the calling method (BtnDownloadGame_Click)
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to download game file: {ex.Message}", ex); // Wrap other exceptions
            }
        }

        private async Task ExtractGameFile()
        {
            try
            {
                labelDownload.Text = "Extracting game files...";
                progressBar1.Value = 90;

                BtnDownloadGame.Enabled = false; // Disable download button during extraction

                await Task.Run(() =>
                {
                    using (var fileStream = new FileStream(tempZipPath, FileMode.Open, FileAccess.Read))
                    using (var zipStream = new ZipInputStream(fileStream))
                    {
                        ZipEntry entry;
                        while ((entry = zipStream.GetNextEntry()) != null)
                        {
                            var entryPath = Path.Combine(gameFilesPath, entry.Name);
                            var entryDir = Path.GetDirectoryName(entryPath); // Safe for files and dirs
                            if (entry.IsFile)
                            {
                                if (!string.IsNullOrEmpty(entryDir) && !Directory.Exists(entryDir))
                                {
                                    Directory.CreateDirectory(entryDir);
                                }
                                using (var outputStream = File.Create(entryPath)) zipStream.CopyTo(outputStream);
                            }
                            else if (entry.IsDirectory)
                            {
                                if (!Directory.Exists(entryPath)) Directory.CreateDirectory(entryPath);
                            }
                        }
                    }
                });

                progressBar1.Value = 100;
                labelDownload.Text = "Extraction complete!";

                BtnDownloadGame.ForeColor = Color.DodgerBlue; // Reset button color to default
                BtnDownloadGame.Text = "DOWNLOAD"; // Reset button text
                BtnDownloadGame.Enabled = true; // Re-enable download button
                checkForUpdates.Enabled = true; // Reset update check button
            }
            catch (Exception ex) { throw new Exception($"Failed to extract game files: {ex.Message}"); }
        }

        /// <summary>
        /// Cleans up partially downloaded or extracted game files and the temporary zip.
        /// This is called on download cancellation or error.
        /// </summary>
        private void CleanupDownloadFiles()
        {
            labelDownload.Text = "Cleaning up...";
            progressBar1.Value = 0;
            try
            {
                // Delete the temporary zip file
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }

                // Delete the entire game files directory if it exists and is empty or contains only partial data
                // Be cautious with this if gameFilesPath might contain unrelated user data.
                // For this launcher, it appears it's exclusively for game data.
                if (Directory.Exists(gameFilesPath))
                {
                    // Attempt to delete files within, then the directory itself
                    try
                    {
                        Directory.Delete(gameFilesPath, true); // true for recursive deletion
                    }
                    catch (IOException ioEx)
                    {
                        // Handle cases where files might be locked, e.g., by antivirus
                        Debug.WriteLine($"Cleanup error: Could not delete directory {gameFilesPath}: {ioEx.Message}");
                        MessageBox.Show($"Failed to fully clean up game files: {ioEx.Message}\nSome files might be locked. Please try deleting the 'Game Files' folder manually if issues persist.", "Cleanup Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during cleanup: {ex.Message}", "Cleanup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progressBar1.Value = 0; // Reset progress bar after cleanup
            labelDownload.Text = "Ready to Download"; // Reset status label
        }

        private void SetButtonStates(bool downloadEnabled, bool playEnabled, bool deleteEnabled)
        {
            BtnDownloadGame.Enabled = downloadEnabled;
            BtnPlay.Enabled = playEnabled;
            BtnDltGame.Enabled = deleteEnabled;
        }

        private async void BtnPlay_Click(object sender, EventArgs e)
        {
            if (!File.Exists(gameExecutablePath))
            {
                MessageBox.Show("The game's executable file was not found. Please download the game first, or download again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process gameProcess = null;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = $"-jar \"{gameExecutablePath}\"",
                    WorkingDirectory = gameFilesPath,
                    UseShellExecute = false
                };
                gameProcess = Process.Start(startInfo);

                if (gameProcess != null && !gameProcess.HasExited)
                {
                    if (checkBox2.Checked)
                    {
                        await Task.Delay(2000); // Allow time for processes to initialize

                        // Elevate priority for ALL java.exe processes
                        Debug.WriteLine("BoostGamePerformance is checked. Attempting to elevate Java process priorities.");
                        try
                        {
                            // GetProcessesByName("java") should find "java.exe" and "javaw.exe"
                            Process[] javaProcesses = Process.GetProcessesByName("java");
                            if (javaProcesses.Length > 0)
                            {
                                Debug.WriteLine($"Found {javaProcesses.Length} 'java' processes. Attempting to set priority to High.");
                                foreach (Process proc in javaProcesses)
                                {
                                    try
                                    {
                                        if (!proc.HasExited) // Check if the process is still running
                                        {
                                            proc.PriorityClass = ProcessPriorityClass.High;
                                            Debug.WriteLine($"Set priority for 'java' process (PID: {proc.Id}, Name: {proc.ProcessName}) to High.");
                                        }
                                    }
                                    // Catch specific exceptions for better diagnostics
                                    catch (Win32Exception wEx) // e.g., Access Denied
                                    {
                                        Debug.WriteLine($"Could not set priority for 'java' process (PID: {proc.Id}, Name: {proc.ProcessName}): {wEx.Message} (NativeErrorCode: {wEx.NativeErrorCode})");
                                    }
                                    catch (InvalidOperationException ioEx) // e.g., Process has exited
                                    {
                                        Debug.WriteLine($"Could not set priority for 'java' process (PID: {proc.Id}, Name: {proc.ProcessName}) as it may have already exited: {ioEx.Message}");
                                    }
                                    catch (Exception ex) // Catch-all for other errors during priority set
                                    {
                                        Debug.WriteLine($"An unexpected error occurred while setting priority for 'java' process (PID: {proc.Id}, Name: {proc.ProcessName}): {ex.Message}");
                                    }
                                    finally
                                    {
                                        proc.Dispose(); // Dispose the Process object for this iteration
                                    }
                                }
                            }
                            else
                            {
                                Debug.WriteLine("No 'java' processes found to elevate priority (after game start). This might be unexpected if the game is running.");
                            }
                        }
                        catch (Exception ex) // Catch errors from GetProcessesByName itself
                        {
                            Debug.WriteLine($"Error retrieving 'java' processes: {ex.Message}");
                        }

                        TerminateUnnecessaryProcesses();
                    }

                    if (checkBox1.Checked)
                    {
                        await Task.Delay(500);
                        this.Close();
                    }
                }
                else if (gameProcess == null)
                {
                    MessageBox.Show("The game process could not be started. It may have been closed immediately or could not be started.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // If gameProcess is not null but has exited, it means it started and closed quickly.
                // No specific message for that here, as it might be normal or an error within the Java app.
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 2)
            {
                MessageBox.Show($"Failed to start game: No Java executable found. Please make sure Java is installed and added to your system's PATH.\n\nDetails: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start the game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Dispose the gameProcess object if it was started.
                // This releases the handle, doesn't kill the external process.
                gameProcess?.Dispose();
            }
        }

        private void TerminateUnnecessaryProcesses()
        {
            string[] processNamesToKill = {
                "XboxPcAppFt", "XboxGameBar", "GameBarPresenceWriter", "GamingServices", "Gaming Services", "XboxAppServices",
                "wmplayer", "Media Player", "Medya Oynatýcý",
                "OneDrive",
                "CTF Loader", "CTF Yükleyici", // ctfmon.exe - often related to input/language, be cautious
                "Settings", "Ayarlar",
                "Runtime Broker", // System process, usually leave alone unless problematic
                "Copilot", // If Microsoft Copilot is running
                "PhoneLink",
                "YourPhone",
                "Teams" // Microsoft Teams
            };

            StringBuilder killLog = new StringBuilder();
            killLog.AppendLine("Performance Boost: Attempting to close background applications...");
            Debug.WriteLine("Performance Boost: Attempting to close background applications...");

            foreach (string procName in processNamesToKill)
            {
                try
                {
                    Process[] processes = Process.GetProcessesByName(procName);
                    if (processes.Length == 0) continue;

                    foreach (Process p in processes)
                    {
                        try
                        {
                            // Check if we can even try to kill (e.g. System Idle Process has Id 0)
                            if (p.Id == 0) continue;

                            // Optional: Add more checks here, e.g., don't kill if it's explorer.exe or critical system processes by name
                            // if (p.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase)) continue;

                            p.Kill();
                            // p.WaitForExit(1000); // Giving a chance to exit. Can hang if process doesn't respond.
                            // For aggressive termination, Kill() is often enough.
                            killLog.AppendLine($"- Killed {p.ProcessName} (PID: {p.Id})");
                            Debug.WriteLine($"- Killed {p.ProcessName} (PID: {p.Id})");
                        }
                        catch (Win32Exception ex)
                        {
                            killLog.AppendLine($"- Could not kill {p.ProcessName} (PID: {p.Id}): {ex.Message} (Often access denied)");
                            Debug.WriteLine($"- Could not kill {p.ProcessName} (PID: {p.Id}): {ex.Message}");
                        }
                        catch (InvalidOperationException ex) // Process already exited or no access
                        {
                            killLog.AppendLine($"- Could not kill {p.ProcessName} (PID: {p.Id}): {ex.Message} (Process may have exited or no access)");
                            Debug.WriteLine($"- Could not kill {p.ProcessName} (PID: {p.Id}): {ex.Message}");
                        }
                        finally
                        {
                            p.Dispose();
                        }
                    }
                }
                catch (Exception ex) // Error getting processes by name
                {
                    killLog.AppendLine($"Error accessing processes for '{procName}': {ex.Message}");
                    Debug.WriteLine($"Error accessing processes for '{procName}': {ex.Message}");
                }
            }

            // Log the results of the kill attempts
            Debug.WriteLine(killLog.ToString()); // Log is now inline
        }

        private async void BtnDltGame_Click(object sender, EventArgs e)
        {
            if (isDownloadingOrExtracting) return; // Prevent deletion if another major operation is ongoing

            if (!Directory.Exists(gameFilesPath))
            {
                MessageBox.Show("The game is not installed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CheckGameInstallation();
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete the game? This cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                isDownloadingOrExtracting = true; // Set flag to true for deletion process
                SetButtonStates(false, false, false); // Disable buttons
                labelDownload.Text = "Deleting game files...";
                progressBar1.Value = 0;

                checkForUpdates.Enabled = false; // Disable update check button during deletion

                try
                {
                    // Simulate progress as Directory.Delete doesn't offer native progress
                    progressBar1.Value = 10;
                    labelDownload.Text = "Checking game folder...";
                    await Task.Delay(500); // Small delay for effect

                    progressBar1.Value = 30;
                    labelDownload.Text = "Deleting game files (1/3)...";
                    await Task.Run(() =>
                    {
                        // Attempt to delete game.jar first if it exists, to avoid issues if it's locked by another process.
                        // Though usually, if the game isn't running, it shouldn't be locked.
                        if (File.Exists(gameExecutablePath))
                        {
                            File.Delete(gameExecutablePath);
                        }
                    });
                    await Task.Delay(200);

                    progressBar1.Value = 60;
                    labelDownload.Text = "Deleting game files (2/3)...";
                    await Task.Run(() =>
                    {
                        // Delete the entire game files directory
                        if (Directory.Exists(gameFilesPath))
                        {
                            Directory.Delete(gameFilesPath, true); // true for recursive deletion
                        }
                    });
                    await Task.Delay(200);

                    progressBar1.Value = 90;
                    labelDownload.Text = "Cleaning up temporary files...";
                    await Task.Run(() =>
                    {
                        if (File.Exists(tempZipPath))
                        {
                            File.Delete(tempZipPath);
                        }
                    });
                    await Task.Delay(200);

                    labelDownload.Text = "Deletion completed!";
                    progressBar1.Value = 100;

                    checkForUpdates.Enabled = true; // Enable update check button after deletion

                    MessageBox.Show("The game was deleted successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    isDownloadingOrExtracting = false; // Reset flag
                    CheckGameInstallation(); // Update UI based on installation status
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            httpClient?.Dispose();
            _downloadCancellationTokenSource?.Dispose(); // Dispose if still active
            base.OnFormClosed(e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Logic is handled by UpdateCheckBoxStates and BtnPlay_Click
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxStates();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            // Save setting immediately when changed
            Properties.Settings.Default.DarkMode = checkBox3.Checked;
            Properties.Settings.Default.Save();

            if (checkBox3.Checked)
            {
                this.Invoke((Action)(() =>
                {
                    ApplyDarkMode();
                }));
            }
            else
            {
                this.Invoke((Action)(() =>
                {
                    ApplyLightTheme();
                }));
            }
        }

        private void UpdateCheckBoxStates()
        {
            if (checkBox2 == null || checkBox1 == null) return;

            if (checkBox2.Checked)
            {
                if (!checkBox1.Checked) checkBox1.Checked = true;
                checkBox1.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
            }
        }

        private async Task UpdateCheckAsync(bool isManualCheck = false)
        {
            isDownloadStarted = false; // Reset the download started flag

            if (disableUpdateCheck && !isManualCheck) // Allow manual check even if auto-check is disabled for some reason
            {
                if (isManualCheck)
                {
                    // If you want a message for manual check when disabled, add it here.
                    // For now, just returns.
                }
                return;
            }

            // Ensure UI updates are on the UI thread
            Action<Action> uiUpdate = (action) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(action);
                }
                else
                {
                    action();
                }
            };

            uiUpdate(() =>
            {
                if (checkForUpdates != null) checkForUpdates.Enabled = false;
                if (isManualCheck)
                {
                    Cursor = Cursors.WaitCursor;
                }
            });

            await Task.Delay(9000);

            try
            {
                using (WebClient client = new WebClient())
                {
                    // Ensure no caching for version.txt
                    string versionUrl = $"https://raw.githubusercontent.com/rmco3/aoh3launcher/main/version.txt?t={DateTime.Now.Ticks}";
                    client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                    client.Headers.Add(HttpRequestHeader.CacheControl, "no-cache"); // Extra measure for cache control


                    Debug.WriteLine($"Fetching version info from: {versionUrl}");
                    string versionContent = await client.DownloadStringTaskAsync(versionUrl);
                    Debug.WriteLine($"Version info content received: {versionContent}");

                    if (string.IsNullOrWhiteSpace(versionContent))
                    {
                        uiUpdate(() =>
                        {
                            MessageBox.Show("Could not get version information. Version file may be empty or may not exist. Please try again later.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                        return;
                    }

                    JObject versionJson;
                    try
                    {
                        versionJson = JObject.Parse(versionContent);
                    }
                    catch (JsonReaderException jsonEx)
                    {
                        Debug.WriteLine($"UpdateCheckAsync - JsonReaderException: {jsonEx.ToString()}");
                        uiUpdate(() =>
                        {
                            MessageBox.Show($"Error parsing update version information. The version file might be malformed.\nDetails: {jsonEx.Message}", "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        });
                        return;
                    }

                    string latestVersion = versionJson["version"]?.ToString();
                    if (string.IsNullOrEmpty(latestVersion))
                    {
                        uiUpdate(() =>
                        {
                            MessageBox.Show("The version information in the file is missing or incorrect.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        });
                        return;
                    }

                    if (latestVersion != CurrentVersion)
                    {
                        string[] updateNoteUrls = {
                        "https://raw.githubusercontent.com/rmco3/aoh3launcher/main/UpdateNote.txt",
                        "https://raw.githubusercontent.com/rmco3/aoh3launcher/main/UpdateNote1.txt"
                    };
                        string updateNoteContent = null;

                        foreach (string url in updateNoteUrls)
                        {
                            string currentUrlForNote = null;
                            try
                            {
                                currentUrlForNote = $"{url}?t={DateTime.Now.Ticks}"; // Cache bust
                                client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                                client.Headers[HttpRequestHeader.CacheControl] = "no-cache";


                                Debug.WriteLine($"Attempting to download update note from: {currentUrlForNote}");
                                byte[] fileBytes = await client.DownloadDataTaskAsync(currentUrlForNote);
                                // Assume UTF-8 encoding for text files from GitHub, which is a common standard.
                                string content = Encoding.UTF8.GetString(fileBytes);
                                Debug.WriteLine($"Successfully downloaded and decoded update note from: {currentUrlForNote}. Content length: {content?.Length ?? 0}");

                                if (!string.IsNullOrWhiteSpace(content))
                                {
                                    updateNoteContent = content;
                                    break; // Found content, no need to check other URLs
                                }
                                else
                                {
                                    Debug.WriteLine($"Update note from {currentUrlForNote} is empty or whitespace.");
                                }
                            }
                            catch (WebException webEx)
                            {
                                Debug.WriteLine($"UpdateCheckAsync - WebException downloading update note from {currentUrlForNote ?? url}: {webEx.ToString()}");
                                if (webEx.Response is HttpWebResponse errorResponse)
                                {
                                    Debug.WriteLine($"HTTP Status Code: {errorResponse.StatusCode}");
                                }
                                // Silently continue to the next URL or fail if this is the last one.
                            }
                            catch (Exception noteProcEx) // Catch other errors during note download/processing
                            {
                                Debug.WriteLine($"UpdateCheckAsync - Exception processing update note from {currentUrlForNote ?? url}: {noteProcEx.ToString()}");
                                // Silently continue to the next URL
                            }
                        }

                        if (!string.IsNullOrEmpty(updateNoteContent))
                        {
                            uiUpdate(() =>
                            {
                                if (!isDownloadStarted)
                                {
                                    DialogResult result = MessageBox.Show(
                                    $"{updateNoteContent}\n\nDo you want to download the new update?",
                                    "New Update Available",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question
                                );

                                    if (result == DialogResult.Yes)
                                    {
                                        string updaterProcessName = "AOH3Launcher Updater";
                                        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                        string updaterPath = Path.Combine(currentDirectory, "AOH3Launcher Updater.exe");

                                        Process[] processes = Process.GetProcessesByName(updaterProcessName);
                                        if (processes.Length > 0)
                                        {
                                            IntPtr handle = processes[0].MainWindowHandle;
                                            if (handle != IntPtr.Zero)
                                            {
                                                ShowWindow(handle, SW_SHOW); // SW_SHOW = 5
                                                SetForegroundWindow(handle);
                                            }
                                            Array.ForEach(processes, p => p.Dispose()); // Dispose process objects
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Process.Start(updaterPath);
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine($"Failed to start updater: {ex.ToString()}");
                                                MessageBox.Show($"The updater application failed to start. Please try again or check app installation.\nDetails: {ex.Message}", "Updater Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        Application.Exit();
                                    }
                                }
                            });
                        }
                        else
                        {
                            uiUpdate(() =>
                            {
                                MessageBox.Show("A new version is available, but update notes could not be retrieved. Manually check \"https://github.com/rmco3/aoh3launcher/releases\"", "Update Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            });
                        }
                    }
                    else
                    {
                        if (isManualCheck)
                        {
                            uiUpdate(() =>
                            {
                                MessageBox.Show("AOH3 Launcher is up to date. There is no new update available. Relax.", "Up to date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            });
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.WriteLine($"UpdateCheckAsync - Outer WebException: {ex.ToString()}");
                uiUpdate(() =>
                {
                    string message = "The update check failed. Please check your internet connection and try again.";
                    if (ex.Response is HttpWebResponse response)
                    {
                        message += $"\n(Status: {response.StatusCode})";
                    }
                    else if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                    {
                        message += $"\n(Could not resolve server name.)";
                    }
                    MessageBox.Show(message, "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
            catch (Exception ex) // Catch-all for any other unexpected errors
            {
                Debug.WriteLine($"UpdateCheckAsync - General Exception: {ex.ToString()}");
                uiUpdate(() =>
                {
                    // This is the generic message the user was likely seeing.
                    MessageBox.Show($"An unexpected error occurred while checking for updates. Please try again later.\nDetails: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
            finally
            {
                uiUpdate(() =>
                {
                    if (!isDownloadStarted)
                    {
                        if (checkForUpdates != null) checkForUpdates.Enabled = true;
                        if (isManualCheck)
                        {
                            Cursor = Cursors.Default;
                        }

                        isDownloadStarted = false; // Reset the flag if no download was initiated   
                    }
                    else
                    {
                        if (checkForUpdates != null) checkForUpdates.Enabled = false;
                        if (isManualCheck)
                        {
                            Cursor = Cursors.Default;
                        }

                        isDownloadStarted = true; // Keep the flag true if a download was initiated
                    }
                });
            }
        }

        private async void checkForUpdates_Click(object sender, EventArgs e)
        {
            this.Invoke((Action)(() =>
            {
                checkForUpdates.Enabled = false;
            }));

            await UpdateCheckAsync(true); // Pass 'true' to indicate a manual check
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOW = 5;

        private async void updateNotes_Click(object sender, EventArgs e)
        {
            if (updateNoteWindow == null || updateNoteWindow.IsDisposed)
            {
                updateNoteWindow = new UpdateNote();
                updateNoteWindow.FormClosed += (s, args) => updateNoteWindow = null;
                updateNoteWindow.Show();
            }
            else
            {
                updateNoteWindow.Activate();
                if (updateNoteWindow.WindowState == FormWindowState.Minimized)
                {
                    updateNoteWindow.WindowState = FormWindowState.Normal;
                }
            }

            updateNotes.ForeColor = Color.FromArgb(192, 0, 0); // Change button color to red 

            await Task.Delay(120);

            updateNotes.ForeColor = Color.CornflowerBlue; // Reset button color to default
        }

        private void ApplyDarkMode()
        {
            // Set the form background color
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Update TabPages' appearance
            foreach (TabPage tab in tabPage.TabPages)
            {
                tab.BackColor = Color.FromArgb(45, 45, 48);
            }

            // Update all controls in the form
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = Color.FromArgb(45, 45, 48);
                    button.ForeColor = Color.White;
                }
                else if (control is Label label)
                {
                    label.ForeColor = Color.White;
                }
                else if (control is TabControl tabControl)
                {
                    foreach (TabPage tab in tabControl.TabPages)
                    {
                        tab.BackColor = Color.FromArgb(45, 45, 48);
                        tab.ForeColor = Color.White;
                    }
                }
                else if (control is System.Windows.Forms.CheckBox checkBox)
                {
                    checkBox.ForeColor = Color.White;
                }
                else if (control is System.Windows.Forms.RadioButton radioButton)
                {
                    radioButton.ForeColor = Color.White;
                }
            }
        }

        private void ApplyLightTheme()
        {
            // Set the form background color
            this.BackColor = SystemColors.Control;

            // Update TabPages' appearance
            foreach (TabPage tab in tabPage.TabPages)
            {
                tab.BackColor = SystemColors.Control;
                tab.ForeColor = SystemColors.ControlText;
            }

            // Update all controls in the form
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                }
                else if (control is Label label)
                {
                    label.ForeColor = SystemColors.ControlText;
                }
                else if (control is TabControl tabControl)
                {
                    foreach (TabPage tab in tabControl.TabPages)
                    {
                        tab.BackColor = SystemColors.Control;
                        tab.ForeColor = SystemColors.ControlText;
                    }
                }
                else if (control is System.Windows.Forms.CheckBox checkBox)
                {
                    checkBox.ForeColor = SystemColors.ControlText;
                }
                else if (control is System.Windows.Forms.RadioButton radioButton)
                {
                    radioButton.ForeColor = SystemColors.ControlText;
                }
            }//
        }
    }
}//