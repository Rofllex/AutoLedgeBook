namespace AutoLedgeBook.Forms
{
    partial class OpenConsinmentsBookForm
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
            this.openConsinmentsBookButton = new MetroFramework.Controls.MetroButton();
            this.canteenRadioButton = new MetroFramework.Controls.MetroRadioButton();
            this.ledgeRadioButton = new MetroFramework.Controls.MetroRadioButton();
            this.metroProgressSpinner1 = new MetroFramework.Controls.MetroProgressSpinner();
            this.metroProgressBar1 = new MetroFramework.Controls.MetroProgressBar();
            this.logLabel = new MetroFramework.Controls.MetroLabel();
            this.recentFilesListBox = new System.Windows.Forms.ListBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // openConsinmentsBookButton
            // 
            this.openConsinmentsBookButton.Location = new System.Drawing.Point(20, 128);
            this.openConsinmentsBookButton.Name = "openConsinmentsBookButton";
            this.openConsinmentsBookButton.Size = new System.Drawing.Size(168, 20);
            this.openConsinmentsBookButton.TabIndex = 4;
            this.openConsinmentsBookButton.Text = "Выбор книги";
            this.openConsinmentsBookButton.UseSelectable = true;
            // 
            // canteenRadioButton
            // 
            this.canteenRadioButton.AutoSize = true;
            this.canteenRadioButton.Location = new System.Drawing.Point(20, 99);
            this.canteenRadioButton.Name = "canteenRadioButton";
            this.canteenRadioButton.Size = new System.Drawing.Size(110, 15);
            this.canteenRadioButton.TabIndex = 2;
            this.canteenRadioButton.Text = "Книга столовой";
            this.canteenRadioButton.UseSelectable = true;
            // 
            // ledgeRadioButton
            // 
            this.ledgeRadioButton.AutoSize = true;
            this.ledgeRadioButton.Checked = true;
            this.ledgeRadioButton.Location = new System.Drawing.Point(20, 81);
            this.ledgeRadioButton.Name = "ledgeRadioButton";
            this.ledgeRadioButton.Size = new System.Drawing.Size(126, 15);
            this.ledgeRadioButton.TabIndex = 3;
            this.ledgeRadioButton.TabStop = true;
            this.ledgeRadioButton.Text = "Книга кладовщика";
            this.ledgeRadioButton.UseSelectable = true;
            // 
            // metroProgressSpinner1
            // 
            this.metroProgressSpinner1.Location = new System.Drawing.Point(194, 128);
            this.metroProgressSpinner1.Maximum = 100;
            this.metroProgressSpinner1.Name = "metroProgressSpinner1";
            this.metroProgressSpinner1.Size = new System.Drawing.Size(14, 14);
            this.metroProgressSpinner1.Speed = 2F;
            this.metroProgressSpinner1.TabIndex = 5;
            this.metroProgressSpinner1.Text = "metroProgressSpinner1";
            this.metroProgressSpinner1.UseSelectable = true;
            // 
            // metroProgressBar1
            // 
            this.metroProgressBar1.Location = new System.Drawing.Point(20, 153);
            this.metroProgressBar1.Name = "metroProgressBar1";
            this.metroProgressBar1.Size = new System.Drawing.Size(188, 20);
            this.metroProgressBar1.TabIndex = 6;
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.logLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.logLabel.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.logLabel.Location = new System.Drawing.Point(20, 190);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(30, 15);
            this.logLabel.TabIndex = 7;
            this.logLabel.Text = "Test";
            this.logLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.logLabel.UseCustomForeColor = true;
            // 
            // recentFilesListBox
            // 
            this.recentFilesListBox.FormattingEnabled = true;
            this.recentFilesListBox.Location = new System.Drawing.Point(214, 79);
            this.recentFilesListBox.Name = "recentFilesListBox";
            this.recentFilesListBox.Size = new System.Drawing.Size(229, 134);
            this.recentFilesListBox.TabIndex = 8;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(245, 57);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(113, 19);
            this.metroLabel1.TabIndex = 9;
            this.metroLabel1.Text = "Недавние файлы";
            // 
            // OpenConsinmentsBookForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 224);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.recentFilesListBox);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.metroProgressBar1);
            this.Controls.Add(this.metroProgressSpinner1);
            this.Controls.Add(this.openConsinmentsBookButton);
            this.Controls.Add(this.canteenRadioButton);
            this.Controls.Add(this.ledgeRadioButton);
            this.Name = "OpenConsinmentsBookForm";
            this.Padding = new System.Windows.Forms.Padding(17, 60, 17, 17);
            this.Text = "Выбор книги накладных";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroButton openConsinmentsBookButton;
        private MetroFramework.Controls.MetroRadioButton canteenRadioButton;
        private MetroFramework.Controls.MetroRadioButton ledgeRadioButton;
        private MetroFramework.Controls.MetroProgressSpinner metroProgressSpinner1;
        private MetroFramework.Controls.MetroProgressBar metroProgressBar1;
        private MetroFramework.Controls.MetroLabel logLabel;
        private System.Windows.Forms.ListBox recentFilesListBox;
        private MetroFramework.Controls.MetroLabel metroLabel1;
    }
}