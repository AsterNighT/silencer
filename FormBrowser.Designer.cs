namespace silencer
{
    partial class FormBrowser
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
            this.listBoxProcessName = new System.Windows.Forms.ListBox();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxProcessName
            // 
            this.listBoxProcessName.FormattingEnabled = true;
            this.listBoxProcessName.ItemHeight = 31;
            this.listBoxProcessName.Location = new System.Drawing.Point(35, 29);
            this.listBoxProcessName.Name = "listBoxProcessName";
            this.listBoxProcessName.Size = new System.Drawing.Size(506, 314);
            this.listBoxProcessName.TabIndex = 0;
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.Location = new System.Drawing.Point(601, 129);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(142, 134);
            this.buttonConfirm.TabIndex = 1;
            this.buttonConfirm.Text = "确定";
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 374);
            this.Controls.Add(this.buttonConfirm);
            this.Controls.Add(this.listBoxProcessName);
            this.Name = "FormBrowser";
            this.Text = "FormBrowser";
            this.Load += new System.EventHandler(this.FormBrowser_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox listBoxProcessName;
        private Button buttonConfirm;
    }
}