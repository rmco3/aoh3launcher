namespace AOH3Launcher
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tabPage = new TabControl();
            tabPage1 = new TabPage();
            labelDownload = new Label();
            BtnPlay = new Button();
            BtnDltGame = new Button();
            BtnDownloadGame = new Button();
            progressBar1 = new ProgressBar();
            tabPage2 = new TabPage();
            panel1 = new Panel();
            groupBox1 = new GroupBox();
            checkBox3 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            label1 = new Label();
            tabPage3 = new TabPage();
            panel2 = new Panel();
            updateNotes = new Label();
            checkForUpdates = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            pictureBox2 = new PictureBox();
            tabPage.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage3.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // tabPage
            // 
            tabPage.Controls.Add(tabPage1);
            tabPage.Controls.Add(tabPage2);
            tabPage.Controls.Add(tabPage3);
            tabPage.Location = new Point(0, -2);
            tabPage.Name = "tabPage";
            tabPage.SelectedIndex = 0;
            tabPage.Size = new Size(452, 209);
            tabPage.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(labelDownload);
            tabPage1.Controls.Add(BtnPlay);
            tabPage1.Controls.Add(BtnDltGame);
            tabPage1.Controls.Add(BtnDownloadGame);
            tabPage1.Controls.Add(progressBar1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(444, 181);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Game";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // labelDownload
            // 
            labelDownload.AutoSize = true;
            labelDownload.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 162);
            labelDownload.Location = new Point(15, 122);
            labelDownload.Name = "labelDownload";
            labelDownload.Size = new Size(212, 20);
            labelDownload.TabIndex = 9;
            labelDownload.Text = "Status: Waiting for Download...";
            // 
            // BtnPlay
            // 
            BtnPlay.Cursor = Cursors.Hand;
            BtnPlay.FlatStyle = FlatStyle.Popup;
            BtnPlay.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            BtnPlay.ForeColor = Color.FromArgb(0, 192, 0);
            BtnPlay.Location = new Point(19, 53);
            BtnPlay.Name = "BtnPlay";
            BtnPlay.Size = new Size(395, 23);
            BtnPlay.TabIndex = 8;
            BtnPlay.Text = "PLAY";
            BtnPlay.UseVisualStyleBackColor = true;
            BtnPlay.Click += BtnPlay_Click;
            // 
            // BtnDltGame
            // 
            BtnDltGame.Cursor = Cursors.Hand;
            BtnDltGame.FlatStyle = FlatStyle.Popup;
            BtnDltGame.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            BtnDltGame.ForeColor = Color.FromArgb(192, 0, 0);
            BtnDltGame.Location = new Point(19, 82);
            BtnDltGame.Name = "BtnDltGame";
            BtnDltGame.Size = new Size(395, 23);
            BtnDltGame.TabIndex = 7;
            BtnDltGame.Text = "DELETE";
            BtnDltGame.UseVisualStyleBackColor = true;
            BtnDltGame.Click += BtnDltGame_Click;
            // 
            // BtnDownloadGame
            // 
            BtnDownloadGame.Cursor = Cursors.Hand;
            BtnDownloadGame.FlatStyle = FlatStyle.Popup;
            BtnDownloadGame.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            BtnDownloadGame.ForeColor = Color.DodgerBlue;
            BtnDownloadGame.Location = new Point(19, 24);
            BtnDownloadGame.Name = "BtnDownloadGame";
            BtnDownloadGame.Size = new Size(395, 23);
            BtnDownloadGame.TabIndex = 6;
            BtnDownloadGame.Text = "DOWNLOAD";
            BtnDownloadGame.UseVisualStyleBackColor = true;
            BtnDownloadGame.Click += BtnDownloadGame_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(19, 148);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(395, 23);
            progressBar1.TabIndex = 5;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(panel1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(444, 181);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Settings/Performance";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(groupBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(438, 175);
            panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox3);
            groupBox1.Controls.Add(checkBox2);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Location = new Point(15, 33);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(405, 114);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Cursor = Cursors.Hand;
            checkBox3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            checkBox3.Location = new Point(19, 76);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(243, 21);
            checkBox3.TabIndex = 2;
            checkBox3.Text = "Dark mode (recommended for night)";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Cursor = Cursors.Hand;
            checkBox2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            checkBox2.Location = new Point(19, 49);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(301, 21);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "Optimize the game for low systems (fps boost)";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Cursor = Cursors.Hand;
            checkBox1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            checkBox1.Location = new Point(19, 22);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(298, 21);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Automatically close Launcher after game starts";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label1.Location = new Point(128, 7);
            label1.Name = "label1";
            label1.Size = new Size(186, 21);
            label1.TabIndex = 0;
            label1.Text = "AOH3 Launcher settings";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panel2);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(444, 181);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "About";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(updateNotes);
            panel2.Controls.Add(checkForUpdates);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(pictureBox2);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(444, 181);
            panel2.TabIndex = 0;
            // 
            // updateNotes
            // 
            updateNotes.AutoSize = true;
            updateNotes.Cursor = Cursors.Hand;
            updateNotes.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            updateNotes.ForeColor = Color.CornflowerBlue;
            updateNotes.Location = new Point(138, 95);
            updateNotes.Name = "updateNotes";
            updateNotes.Size = new Size(190, 17);
            updateNotes.TabIndex = 11;
            updateNotes.Text = "Update History/Update Notes";
            updateNotes.Click += updateNotes_Click;
            // 
            // checkForUpdates
            // 
            checkForUpdates.AutoSize = true;
            checkForUpdates.Cursor = Cursors.Hand;
            checkForUpdates.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 162);
            checkForUpdates.ForeColor = Color.CornflowerBlue;
            checkForUpdates.Location = new Point(166, 145);
            checkForUpdates.Name = "checkForUpdates";
            checkForUpdates.Size = new Size(114, 17);
            checkForUpdates.TabIndex = 10;
            checkForUpdates.Text = "Check for updates";
            checkForUpdates.Click += checkForUpdates_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Black", 9.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 162);
            label5.Location = new Point(6, 118);
            label5.Name = "label5";
            label5.Size = new Size(433, 17);
            label5.TabIndex = 9;
            label5.Text = "-------------------------------------------------------------------------------------";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label4.Location = new Point(138, 67);
            label4.Name = "label4";
            label4.Size = new Size(287, 17);
            label4.TabIndex = 8;
            label4.Text = "A simple and useful Age of History 3 launcher";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label3.Location = new Point(138, 38);
            label3.Name = "label3";
            label3.Size = new Size(199, 17);
            label3.TabIndex = 7;
            label3.Text = "© 2024-2025 All rights reserved";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label2.Location = new Point(138, 11);
            label2.Name = "label2";
            label2.Size = new Size(74, 17);
            label2.TabIndex = 6;
            label2.Text = "Version: 1.4";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.aoh3launchericon;
            pictureBox2.Location = new Point(19, 12);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(113, 103);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(452, 207);
            Controls.Add(tabPage);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "AOH3 Launcher";
            Load += Form1_Load;
            tabPage.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage3.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabPage;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Panel panel1;
        private Label label1;
        private TabPage tabPage3;
        private GroupBox groupBox1;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Panel panel2;
        private PictureBox pictureBox2;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label checkForUpdates;
        private Button BtnPlay;
        private Button BtnDltGame;
        private Button BtnDownloadGame;
        private ProgressBar progressBar1;
        private Label labelDownload;
        private Label updateNotes;
        private Label label5;
    }
}
