namespace AutoLedgeBook.Forms
{
    partial class DebugForm
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
            MetroFramework.Controls.MetroLabel metroLabel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.logsListBox = new System.Windows.Forms.ListBox();
            metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            metroLabel1.AutoSize = true;
            metroLabel1.Location = new System.Drawing.Point(20, 61);
            metroLabel1.Name = "metroLabel1";
            metroLabel1.Size = new System.Drawing.Size(92, 19);
            metroLabel1.TabIndex = 1;
            metroLabel1.Text = "Логирование";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "garbage.ico");
            // 
            // logsListBox
            // 
            this.logsListBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.logsListBox.FormattingEnabled = true;
            this.logsListBox.IntegralHeight = false;
            this.logsListBox.ItemHeight = 16;
            this.logsListBox.Location = new System.Drawing.Point(20, 83);
            this.logsListBox.Name = "logsListBox";
            this.logsListBox.Size = new System.Drawing.Size(314, 277);
            this.logsListBox.TabIndex = 0;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 390);
            this.Controls.Add(metroLabel1);
            this.Controls.Add(this.logsListBox);
            this.Name = "DebugForm";
            this.Padding = new System.Windows.Forms.Padding(17, 60, 17, 17);
            this.Text = "Форма отладки";
            this.Load += new System.EventHandler(this.DebugForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListBox logsListBox;
    }
}