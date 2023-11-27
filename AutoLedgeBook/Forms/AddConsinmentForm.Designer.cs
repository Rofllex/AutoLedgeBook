namespace AutoLedgeBook.Forms
{
    partial class AddConsinmentForm
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
            this.selectConsinmentFileNameLabel = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.confirmConsinmentButton = new MetroFramework.Controls.MetroButton();
            this.selectConsinmentDateComboBox = new MetroFramework.Controls.MetroComboBox();
            this.selectConsinmentFileButton = new MetroFramework.Controls.MetroButton();
            this.consinmentsBookLoadProgressBar = new MetroFramework.Controls.MetroProgressBar();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.selectedConsinmentSummaryPcsLabel = new MetroFramework.Controls.MetroLabel();
            this.selectedConsinmentSummaryWeightLabel = new MetroFramework.Controls.MetroLabel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.selectedConsinmentTypeLabel = new MetroFramework.Controls.MetroLabel();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.selectedConsinmentPersonsCountLabel = new MetroFramework.Controls.MetroLabel();
            this.consinmentTypeComboBox = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // selectConsinmentFileNameLabel
            // 
            this.selectConsinmentFileNameLabel.AutoSize = true;
            this.selectConsinmentFileNameLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.selectConsinmentFileNameLabel.Location = new System.Drawing.Point(19, 143);
            this.selectConsinmentFileNameLabel.Name = "selectConsinmentFileNameLabel";
            this.selectConsinmentFileNameLabel.Size = new System.Drawing.Size(124, 15);
            this.selectConsinmentFileNameLabel.TabIndex = 8;
            this.selectConsinmentFileNameLabel.Text = "Наименование фалйа";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(19, 162);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(82, 19);
            this.metroLabel2.TabIndex = 7;
            this.metroLabel2.Text = "Выбор даты";
            // 
            // confirmConsinmentButton
            // 
            this.confirmConsinmentButton.Location = new System.Drawing.Point(19, 333);
            this.confirmConsinmentButton.Name = "confirmConsinmentButton";
            this.confirmConsinmentButton.Size = new System.Drawing.Size(217, 23);
            this.confirmConsinmentButton.TabIndex = 6;
            this.confirmConsinmentButton.Text = "Подтвердить";
            this.confirmConsinmentButton.UseSelectable = true;
            // 
            // selectConsinmentDateComboBox
            // 
            this.selectConsinmentDateComboBox.FormattingEnabled = true;
            this.selectConsinmentDateComboBox.ItemHeight = 23;
            this.selectConsinmentDateComboBox.Location = new System.Drawing.Point(19, 184);
            this.selectConsinmentDateComboBox.Name = "selectConsinmentDateComboBox";
            this.selectConsinmentDateComboBox.Size = new System.Drawing.Size(217, 29);
            this.selectConsinmentDateComboBox.TabIndex = 5;
            this.selectConsinmentDateComboBox.UseSelectable = true;
            // 
            // selectConsinmentFileButton
            // 
            this.selectConsinmentFileButton.Location = new System.Drawing.Point(21, 115);
            this.selectConsinmentFileButton.Name = "selectConsinmentFileButton";
            this.selectConsinmentFileButton.Size = new System.Drawing.Size(215, 23);
            this.selectConsinmentFileButton.TabIndex = 4;
            this.selectConsinmentFileButton.Text = "Выбор файла";
            this.selectConsinmentFileButton.UseSelectable = true;
            // 
            // consinmentsBookLoadProgressBar
            // 
            this.consinmentsBookLoadProgressBar.Location = new System.Drawing.Point(19, 304);
            this.consinmentsBookLoadProgressBar.Name = "consinmentsBookLoadProgressBar";
            this.consinmentsBookLoadProgressBar.Size = new System.Drawing.Size(217, 23);
            this.consinmentsBookLoadProgressBar.TabIndex = 9;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(19, 260);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(66, 19);
            this.metroLabel1.TabIndex = 10;
            this.metroLabel1.Text = "Всего шт:";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(19, 279);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(61, 19);
            this.metroLabel3.TabIndex = 10;
            this.metroLabel3.Text = "Всего кг:";
            // 
            // selectedConsinmentSummaryPcsLabel
            // 
            this.selectedConsinmentSummaryPcsLabel.AutoSize = true;
            this.selectedConsinmentSummaryPcsLabel.Location = new System.Drawing.Point(91, 260);
            this.selectedConsinmentSummaryPcsLabel.Name = "selectedConsinmentSummaryPcsLabel";
            this.selectedConsinmentSummaryPcsLabel.Size = new System.Drawing.Size(25, 19);
            this.selectedConsinmentSummaryPcsLabel.TabIndex = 11;
            this.selectedConsinmentSummaryPcsLabel.Text = "шт";
            // 
            // selectedConsinmentSummaryWeightLabel
            // 
            this.selectedConsinmentSummaryWeightLabel.AutoSize = true;
            this.selectedConsinmentSummaryWeightLabel.Location = new System.Drawing.Point(91, 279);
            this.selectedConsinmentSummaryWeightLabel.Name = "selectedConsinmentSummaryWeightLabel";
            this.selectedConsinmentSummaryWeightLabel.Size = new System.Drawing.Size(20, 19);
            this.selectedConsinmentSummaryWeightLabel.TabIndex = 11;
            this.selectedConsinmentSummaryWeightLabel.Text = "кг";
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(19, 226);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(39, 19);
            this.metroLabel4.TabIndex = 10;
            this.metroLabel4.Text = "Тип: ";
            // 
            // selectedConsinmentTypeLabel
            // 
            this.selectedConsinmentTypeLabel.AutoSize = true;
            this.selectedConsinmentTypeLabel.Location = new System.Drawing.Point(91, 226);
            this.selectedConsinmentTypeLabel.Name = "selectedConsinmentTypeLabel";
            this.selectedConsinmentTypeLabel.Size = new System.Drawing.Size(30, 19);
            this.selectedConsinmentTypeLabel.TabIndex = 11;
            this.selectedConsinmentTypeLabel.Text = "тип";
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(19, 245);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(28, 19);
            this.metroLabel5.TabIndex = 10;
            this.metroLabel5.Text = "На:";
            // 
            // selectedConsinmentPersonsCountLabel
            // 
            this.selectedConsinmentPersonsCountLabel.AutoSize = true;
            this.selectedConsinmentPersonsCountLabel.Location = new System.Drawing.Point(91, 245);
            this.selectedConsinmentPersonsCountLabel.Name = "selectedConsinmentPersonsCountLabel";
            this.selectedConsinmentPersonsCountLabel.Size = new System.Drawing.Size(33, 19);
            this.selectedConsinmentPersonsCountLabel.TabIndex = 11;
            this.selectedConsinmentPersonsCountLabel.Text = "чел.";
            // 
            // consinmentTypeComboBox
            // 
            this.consinmentTypeComboBox.FormattingEnabled = true;
            this.consinmentTypeComboBox.ItemHeight = 23;
            this.consinmentTypeComboBox.Location = new System.Drawing.Point(23, 80);
            this.consinmentTypeComboBox.Name = "consinmentTypeComboBox";
            this.consinmentTypeComboBox.Size = new System.Drawing.Size(215, 29);
            this.consinmentTypeComboBox.TabIndex = 12;
            this.consinmentTypeComboBox.UseSelectable = true;
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(21, 58);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(102, 19);
            this.metroLabel6.TabIndex = 13;
            this.metroLabel6.Text = "Тип накладной";
            // 
            // AddConsinmentForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 374);
            this.Controls.Add(this.metroLabel6);
            this.Controls.Add(this.consinmentTypeComboBox);
            this.Controls.Add(this.selectedConsinmentSummaryWeightLabel);
            this.Controls.Add(this.selectedConsinmentPersonsCountLabel);
            this.Controls.Add(this.selectedConsinmentTypeLabel);
            this.Controls.Add(this.selectedConsinmentSummaryPcsLabel);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.metroLabel4);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.consinmentsBookLoadProgressBar);
            this.Controls.Add(this.selectConsinmentFileNameLabel);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.confirmConsinmentButton);
            this.Controls.Add(this.selectConsinmentFileButton);
            this.Controls.Add(this.selectConsinmentDateComboBox);
            this.Name = "AddConsinmentForm";
            this.Resizable = false;
            this.Text = "Добавить накладную";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Controls.MetroLabel selectConsinmentFileNameLabel;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroButton confirmConsinmentButton;
        private MetroFramework.Controls.MetroComboBox selectConsinmentDateComboBox;
        private MetroFramework.Controls.MetroButton selectConsinmentFileButton;
        private MetroFramework.Controls.MetroProgressBar consinmentsBookLoadProgressBar;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel selectedConsinmentSummaryPcsLabel;
        private MetroFramework.Controls.MetroLabel selectedConsinmentSummaryWeightLabel;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel selectedConsinmentTypeLabel;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel selectedConsinmentPersonsCountLabel;
        private MetroFramework.Controls.MetroComboBox consinmentTypeComboBox;
        private MetroFramework.Controls.MetroLabel metroLabel6;
    }
}