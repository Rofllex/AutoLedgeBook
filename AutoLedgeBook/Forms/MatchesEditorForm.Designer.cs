namespace AutoLedgeBook.Forms
{
    partial class MatchesEditorForm
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
            this.consinmentsBookListView = new MetroFramework.Controls.MetroListView();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.matchedProductsListView = new System.Windows.Forms.ListView();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.allProductsListVIew = new System.Windows.Forms.ListView();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.addMatchTextBox = new System.Windows.Forms.TextBox();
            this.confirmAddMatchButton = new MetroFramework.Controls.MetroButton();
            this.metroContextMenu1 = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.SuspendLayout();
            // 
            // consinmentsBookListView
            // 
            this.consinmentsBookListView.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.consinmentsBookListView.FullRowSelect = true;
            this.consinmentsBookListView.HideSelection = true;
            this.consinmentsBookListView.Location = new System.Drawing.Point(20, 88);
            this.consinmentsBookListView.Name = "consinmentsBookListView";
            this.consinmentsBookListView.OwnerDraw = true;
            this.consinmentsBookListView.Size = new System.Drawing.Size(205, 344);
            this.consinmentsBookListView.TabIndex = 0;
            this.consinmentsBookListView.UseCompatibleStateImageBehavior = false;
            this.consinmentsBookListView.UseSelectable = true;
            this.consinmentsBookListView.View = System.Windows.Forms.View.List;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(20, 68);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(185, 19);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Продукты в книге накладных";
            // 
            // matchedProductsListView
            // 
            this.matchedProductsListView.Location = new System.Drawing.Point(230, 88);
            this.matchedProductsListView.Name = "matchedProductsListView";
            this.matchedProductsListView.Size = new System.Drawing.Size(217, 293);
            this.matchedProductsListView.TabIndex = 2;
            this.matchedProductsListView.UseCompatibleStateImageBehavior = false;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(230, 68);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(174, 19);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Сопоставленные продукты";
            // 
            // allProductsListVIew
            // 
            this.allProductsListVIew.Location = new System.Drawing.Point(452, 88);
            this.allProductsListVIew.Name = "allProductsListVIew";
            this.allProductsListVIew.Size = new System.Drawing.Size(201, 344);
            this.allProductsListVIew.TabIndex = 3;
            this.allProductsListVIew.UseCompatibleStateImageBehavior = false;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(452, 68);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(91, 19);
            this.metroLabel3.TabIndex = 1;
            this.metroLabel3.Text = "Все продукты";
            // 
            // addMatchTextBox
            // 
            this.addMatchTextBox.Location = new System.Drawing.Point(230, 386);
            this.addMatchTextBox.Name = "addMatchTextBox";
            this.addMatchTextBox.Size = new System.Drawing.Size(217, 21);
            this.addMatchTextBox.TabIndex = 4;
            // 
            // confirmAddMatchButton
            // 
            this.confirmAddMatchButton.Location = new System.Drawing.Point(230, 411);
            this.confirmAddMatchButton.Name = "confirmAddMatchButton";
            this.confirmAddMatchButton.Size = new System.Drawing.Size(217, 20);
            this.confirmAddMatchButton.TabIndex = 5;
            this.confirmAddMatchButton.Text = "Добавить";
            this.confirmAddMatchButton.UseSelectable = true;
            // 
            // metroContextMenu1
            // 
            this.metroContextMenu1.Name = "metroContextMenu1";
            this.metroContextMenu1.Size = new System.Drawing.Size(61, 4);
            // 
            // MatchesEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 461);
            this.Controls.Add(this.confirmAddMatchButton);
            this.Controls.Add(this.addMatchTextBox);
            this.Controls.Add(this.allProductsListVIew);
            this.Controls.Add(this.matchedProductsListView);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.consinmentsBookListView);
            this.Name = "MatchesEditorForm";
            this.Padding = new System.Windows.Forms.Padding(17, 52, 17, 17);
            this.Text = "Редактирование сопоставлений";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroListView consinmentsBookListView;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.ListView matchedProductsListView;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private System.Windows.Forms.ListView allProductsListVIew;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private System.Windows.Forms.TextBox addMatchTextBox;
        private MetroFramework.Controls.MetroButton confirmAddMatchButton;
        private MetroFramework.Controls.MetroContextMenu metroContextMenu1;
    }
}