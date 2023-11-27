namespace AutoLedgeBook.Forms
{
    partial class MainForm
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
            MetroFramework.Controls.MetroLabel metroLabel6;
            MetroFramework.Controls.MetroLabel metroLabel3;
            MetroFramework.Controls.MetroLabel metroLabel4;
            MetroFramework.Controls.MetroLabel metroLabel5;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.matchesPanel = new MetroFramework.Controls.MetroPanel();
            this.matchesFileNameLabel = new MetroFramework.Controls.MetroLabel();
            this.selectMatchesButton = new MetroFramework.Controls.MetroButton();
            this.consinmentsToAddGrid = new MetroFramework.Controls.MetroGrid();
            this.ledgeBookMonthLabel = new MetroFramework.Controls.MetroLabel();
            this.ledgeBookTypeLabel = new MetroFramework.Controls.MetroLabel();
            this.addConsinmentButton = new MetroFramework.Controls.MetroButton();
            this.metroContextMenu1 = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.fillConsinmentsProgressBar = new MetroFramework.Controls.MetroProgressBar();
            this.metroProgressSpinner1 = new MetroFramework.Controls.MetroProgressSpinner();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.licenseInfoLabel = new MetroFramework.Controls.MetroLabel();
            metroLabel6 = new MetroFramework.Controls.MetroLabel();
            metroLabel3 = new MetroFramework.Controls.MetroLabel();
            metroLabel4 = new MetroFramework.Controls.MetroLabel();
            metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.matchesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.consinmentsToAddGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // metroLabel6
            // 
            metroLabel6.AutoSize = true;
            metroLabel6.Location = new System.Drawing.Point(13, 3);
            metroLabel6.Name = "metroLabel6";
            metroLabel6.Size = new System.Drawing.Size(156, 19);
            metroLabel6.TabIndex = 2;
            metroLabel6.Text = "Выбрать сопоставления";
            // 
            // metroLabel3
            // 
            metroLabel3.AutoSize = true;
            metroLabel3.Location = new System.Drawing.Point(20, 52);
            metroLabel3.Name = "metroLabel3";
            metroLabel3.Size = new System.Drawing.Size(133, 19);
            metroLabel3.TabIndex = 10;
            metroLabel3.Text = "Загруженная книга: ";
            // 
            // metroLabel4
            // 
            metroLabel4.AutoSize = true;
            metroLabel4.Location = new System.Drawing.Point(20, 68);
            metroLabel4.Name = "metroLabel4";
            metroLabel4.Size = new System.Drawing.Size(56, 19);
            metroLabel4.TabIndex = 11;
            metroLabel4.Text = "Месяц: ";
            // 
            // metroLabel5
            // 
            metroLabel5.AutoSize = true;
            metroLabel5.Location = new System.Drawing.Point(20, 85);
            metroLabel5.Name = "metroLabel5";
            metroLabel5.Size = new System.Drawing.Size(168, 19);
            metroLabel5.TabIndex = 12;
            metroLabel5.Text = "Накладные к добавлению";
            // 
            // matchesPanel
            // 
            this.matchesPanel.AllowDrop = true;
            this.matchesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.matchesPanel.Controls.Add(this.matchesFileNameLabel);
            this.matchesPanel.Controls.Add(this.selectMatchesButton);
            this.matchesPanel.Controls.Add(metroLabel6);
            this.matchesPanel.HorizontalScrollbarBarColor = true;
            this.matchesPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.matchesPanel.HorizontalScrollbarSize = 9;
            this.matchesPanel.Location = new System.Drawing.Point(525, 23);
            this.matchesPanel.Name = "matchesPanel";
            this.matchesPanel.Size = new System.Drawing.Size(171, 79);
            this.matchesPanel.TabIndex = 13;
            this.matchesPanel.VerticalScrollbarBarColor = true;
            this.matchesPanel.VerticalScrollbarHighlightOnWheel = false;
            this.matchesPanel.VerticalScrollbarSize = 9;
            // 
            // matchesFileNameLabel
            // 
            this.matchesFileNameLabel.AutoSize = true;
            this.matchesFileNameLabel.Location = new System.Drawing.Point(13, 29);
            this.matchesFileNameLabel.Name = "matchesFileNameLabel";
            this.matchesFileNameLabel.Size = new System.Drawing.Size(144, 19);
            this.matchesFileNameLabel.TabIndex = 4;
            this.matchesFileNameLabel.Text = "Наименование файла";
            // 
            // selectMatchesButton
            // 
            this.selectMatchesButton.Location = new System.Drawing.Point(13, 51);
            this.selectMatchesButton.Name = "selectMatchesButton";
            this.selectMatchesButton.Size = new System.Drawing.Size(144, 20);
            this.selectMatchesButton.TabIndex = 3;
            this.selectMatchesButton.Text = "Выбор файла";
            this.selectMatchesButton.UseSelectable = true;
            // 
            // consinmentsToAddGrid
            // 
            this.consinmentsToAddGrid.AllowUserToAddRows = false;
            this.consinmentsToAddGrid.AllowUserToDeleteRows = false;
            this.consinmentsToAddGrid.AllowUserToResizeRows = false;
            this.consinmentsToAddGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consinmentsToAddGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.consinmentsToAddGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.consinmentsToAddGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.consinmentsToAddGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consinmentsToAddGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.consinmentsToAddGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.consinmentsToAddGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.consinmentsToAddGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.consinmentsToAddGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.consinmentsToAddGrid.EnableHeadersVisualStyles = false;
            this.consinmentsToAddGrid.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.consinmentsToAddGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.consinmentsToAddGrid.HighLightPercentage = 0.1F;
            this.consinmentsToAddGrid.Location = new System.Drawing.Point(20, 136);
            this.consinmentsToAddGrid.Name = "consinmentsToAddGrid";
            this.consinmentsToAddGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.consinmentsToAddGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.consinmentsToAddGrid.RowHeadersVisible = false;
            this.consinmentsToAddGrid.RowHeadersWidth = 63;
            this.consinmentsToAddGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.consinmentsToAddGrid.RowTemplate.Height = 25;
            this.consinmentsToAddGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.consinmentsToAddGrid.Size = new System.Drawing.Size(677, 370);
            this.consinmentsToAddGrid.TabIndex = 9;
            this.consinmentsToAddGrid.UseCustomBackColor = true;
            this.consinmentsToAddGrid.UseCustomForeColor = true;
            // 
            // ledgeBookMonthLabel
            // 
            this.ledgeBookMonthLabel.AutoSize = true;
            this.ledgeBookMonthLabel.Location = new System.Drawing.Point(73, 68);
            this.ledgeBookMonthLabel.Name = "ledgeBookMonthLabel";
            this.ledgeBookMonthLabel.Size = new System.Drawing.Size(62, 19);
            this.ledgeBookMonthLabel.TabIndex = 11;
            this.ledgeBookMonthLabel.Text = "{ месяц }";
            // 
            // ledgeBookTypeLabel
            // 
            this.ledgeBookTypeLabel.AutoSize = true;
            this.ledgeBookTypeLabel.Location = new System.Drawing.Point(141, 52);
            this.ledgeBookTypeLabel.Name = "ledgeBookTypeLabel";
            this.ledgeBookTypeLabel.Size = new System.Drawing.Size(148, 19);
            this.ledgeBookTypeLabel.TabIndex = 10;
            this.ledgeBookTypeLabel.Text = "{наименование книги}";
            // 
            // addConsinmentButton
            // 
            this.addConsinmentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addConsinmentButton.Location = new System.Drawing.Point(393, 79);
            this.addConsinmentButton.Name = "addConsinmentButton";
            this.addConsinmentButton.Size = new System.Drawing.Size(128, 23);
            this.addConsinmentButton.TabIndex = 14;
            this.addConsinmentButton.Text = "Добавить накладную";
            this.addConsinmentButton.UseSelectable = true;
            // 
            // metroContextMenu1
            // 
            this.metroContextMenu1.Name = "metroContextMenu1";
            this.metroContextMenu1.Size = new System.Drawing.Size(61, 4);
            // 
            // fillConsinmentsProgressBar
            // 
            this.fillConsinmentsProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fillConsinmentsProgressBar.Location = new System.Drawing.Point(20, 104);
            this.fillConsinmentsProgressBar.Name = "fillConsinmentsProgressBar";
            this.fillConsinmentsProgressBar.Size = new System.Drawing.Size(677, 20);
            this.fillConsinmentsProgressBar.TabIndex = 15;
            // 
            // metroProgressSpinner1
            // 
            this.metroProgressSpinner1.Location = new System.Drawing.Point(393, 33);
            this.metroProgressSpinner1.Maximum = 100;
            this.metroProgressSpinner1.Name = "metroProgressSpinner1";
            this.metroProgressSpinner1.Size = new System.Drawing.Size(14, 14);
            this.metroProgressSpinner1.TabIndex = 16;
            this.metroProgressSpinner1.Text = "metroProgressSpinner1";
            this.metroProgressSpinner1.UseSelectable = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(20, 509);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(73, 19);
            this.metroLabel1.TabIndex = 17;
            this.metroLabel1.Text = "Лицензия:";
            // 
            // licenseInfoLabel
            // 
            this.licenseInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.licenseInfoLabel.AutoSize = true;
            this.licenseInfoLabel.Location = new System.Drawing.Point(87, 509);
            this.licenseInfoLabel.Name = "licenseInfoLabel";
            this.licenseInfoLabel.Size = new System.Drawing.Size(77, 19);
            this.licenseInfoLabel.TabIndex = 18;
            this.licenseInfoLabel.Text = "{licenseInfo}";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(722, 526);
            this.Controls.Add(this.licenseInfoLabel);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.metroProgressSpinner1);
            this.Controls.Add(this.fillConsinmentsProgressBar);
            this.Controls.Add(this.addConsinmentButton);
            this.Controls.Add(this.matchesPanel);
            this.Controls.Add(metroLabel5);
            this.Controls.Add(this.ledgeBookMonthLabel);
            this.Controls.Add(metroLabel4);
            this.Controls.Add(this.ledgeBookTypeLabel);
            this.Controls.Add(metroLabel3);
            this.Controls.Add(this.consinmentsToAddGrid);
            this.MinimumSize = new System.Drawing.Size(562, 443);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(17, 60, 17, 17);
            this.Text = "Автоматическое заполнение книги";
            this.matchesPanel.ResumeLayout(false);
            this.matchesPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.consinmentsToAddGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Controls.MetroGrid consinmentsToAddGrid;
        private MetroFramework.Controls.MetroLabel ledgeBookMonthLabel;
        private MetroFramework.Controls.MetroLabel ledgeBookTypeLabel;
        private MetroFramework.Controls.MetroPanel matchesPanel;
        private MetroFramework.Controls.MetroLabel matchesFileNameLabel;
        private MetroFramework.Controls.MetroButton selectMatchesButton;
        private MetroFramework.Controls.MetroButton addConsinmentButton;
        private MetroFramework.Controls.MetroContextMenu metroContextMenu1;
        private MetroFramework.Controls.MetroProgressBar fillConsinmentsProgressBar;
        private MetroFramework.Controls.MetroProgressSpinner metroProgressSpinner1;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel licenseInfoLabel;
    }
}