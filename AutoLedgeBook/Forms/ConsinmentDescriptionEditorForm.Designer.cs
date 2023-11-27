namespace AutoLedgeBook.Forms
{
    partial class ConsinmentDescriptionEditorForm
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
            MetroFramework.Controls.MetroLabel metroLabel1;
            MetroFramework.Controls.MetroLabel metroLabel2;
            MetroFramework.Controls.MetroLabel metroLabel3;
            MetroFramework.Controls.MetroLabel metroLabel4;
            this.consinmentNumberTextBox = new System.Windows.Forms.TextBox();
            this.consinmentTypeTextBox = new System.Windows.Forms.TextBox();
            this.consinmentPersonsCountTextBox = new System.Windows.Forms.TextBox();
            this.consinmentDestinationTextBox = new System.Windows.Forms.TextBox();
            this.confirmButton = new MetroFramework.Controls.MetroButton();
            metroLabel1 = new MetroFramework.Controls.MetroLabel();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            metroLabel3 = new MetroFramework.Controls.MetroLabel();
            metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            metroLabel1.AutoSize = true;
            metroLabel1.Location = new System.Drawing.Point(30, 115);
            metroLabel1.Name = "metroLabel1";
            metroLabel1.Size = new System.Drawing.Size(95, 19);
            metroLabel1.TabIndex = 2;
            metroLabel1.Text = "№ накладной";
            // 
            // metroLabel2
            // 
            metroLabel2.AutoSize = true;
            metroLabel2.Location = new System.Drawing.Point(30, 144);
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Size = new System.Drawing.Size(102, 19);
            metroLabel2.TabIndex = 2;
            metroLabel2.Text = "Тип накладной";
            // 
            // metroLabel3
            // 
            metroLabel3.AutoSize = true;
            metroLabel3.Location = new System.Drawing.Point(30, 173);
            metroLabel3.Name = "metroLabel3";
            metroLabel3.Size = new System.Drawing.Size(133, 19);
            metroLabel3.TabIndex = 2;
            metroLabel3.Text = "Кол-во питающихся";
            // 
            // metroLabel4
            // 
            metroLabel4.AutoSize = true;
            metroLabel4.Location = new System.Drawing.Point(30, 202);
            metroLabel4.Name = "metroLabel4";
            metroLabel4.Size = new System.Drawing.Size(92, 19);
            metroLabel4.TabIndex = 2;
            metroLabel4.Text = "Направление";
            // 
            // consinmentNumberTextBox
            // 
            this.consinmentNumberTextBox.Enabled = false;
            this.consinmentNumberTextBox.Location = new System.Drawing.Point(169, 115);
            this.consinmentNumberTextBox.Name = "consinmentNumberTextBox";
            this.consinmentNumberTextBox.Size = new System.Drawing.Size(198, 23);
            this.consinmentNumberTextBox.TabIndex = 0;
            // 
            // consinmentTypeTextBox
            // 
            this.consinmentTypeTextBox.Location = new System.Drawing.Point(169, 144);
            this.consinmentTypeTextBox.Name = "consinmentTypeTextBox";
            this.consinmentTypeTextBox.Size = new System.Drawing.Size(198, 23);
            this.consinmentTypeTextBox.TabIndex = 0;
            // 
            // consinmentPersonsCountTextBox
            // 
            this.consinmentPersonsCountTextBox.Location = new System.Drawing.Point(169, 173);
            this.consinmentPersonsCountTextBox.Name = "consinmentPersonsCountTextBox";
            this.consinmentPersonsCountTextBox.Size = new System.Drawing.Size(198, 23);
            this.consinmentPersonsCountTextBox.TabIndex = 0;
            // 
            // consinmentDestinationTextBox
            // 
            this.consinmentDestinationTextBox.Location = new System.Drawing.Point(169, 202);
            this.consinmentDestinationTextBox.Name = "consinmentDestinationTextBox";
            this.consinmentDestinationTextBox.Size = new System.Drawing.Size(198, 23);
            this.consinmentDestinationTextBox.TabIndex = 0;
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(169, 231);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(198, 23);
            this.confirmButton.TabIndex = 1;
            this.confirmButton.Text = "Подтвердить";
            this.confirmButton.UseSelectable = true;
            // 
            // ConsinmentDescriptionEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 268);
            this.Controls.Add(metroLabel4);
            this.Controls.Add(metroLabel3);
            this.Controls.Add(metroLabel2);
            this.Controls.Add(metroLabel1);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.consinmentDestinationTextBox);
            this.Controls.Add(this.consinmentPersonsCountTextBox);
            this.Controls.Add(this.consinmentTypeTextBox);
            this.Controls.Add(this.consinmentNumberTextBox);
            this.Name = "ConsinmentDescriptionEditorForm";
            this.Text = "Редактирование информации о накладной";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox consinmentNumberTextBox;
        private System.Windows.Forms.TextBox consinmentTypeTextBox;
        private System.Windows.Forms.TextBox consinmentPersonsCountTextBox;
        private System.Windows.Forms.TextBox consinmentDestinationTextBox;
        private MetroFramework.Controls.MetroButton confirmButton;
    }
}