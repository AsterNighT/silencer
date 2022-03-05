namespace silencer
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonBegin = new System.Windows.Forms.Button();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.listBoxWhitelist = new System.Windows.Forms.ListBox();
            this.buttonAddItem = new System.Windows.Forms.Button();
            this.buttonDeleteItem = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonBegin
            // 
            this.buttonBegin.Location = new System.Drawing.Point(51, 329);
            this.buttonBegin.Name = "buttonBegin";
            this.buttonBegin.Size = new System.Drawing.Size(148, 86);
            this.buttonBegin.TabIndex = 0;
            this.buttonBegin.Text = "开始";
            this.buttonBegin.UseVisualStyleBackColor = true;
            this.buttonBegin.Click += new System.EventHandler(this.beginListening);
            // 
            // buttonEnd
            // 
            this.buttonEnd.Location = new System.Drawing.Point(267, 329);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(148, 86);
            this.buttonEnd.TabIndex = 1;
            this.buttonEnd.Text = "结束";
            this.buttonEnd.UseVisualStyleBackColor = true;
            this.buttonEnd.Click += new System.EventHandler(this.endListening);
            // 
            // listBoxWhitelist
            // 
            this.listBoxWhitelist.FormattingEnabled = true;
            this.listBoxWhitelist.ItemHeight = 31;
            this.listBoxWhitelist.Location = new System.Drawing.Point(51, 43);
            this.listBoxWhitelist.Name = "listBoxWhitelist";
            this.listBoxWhitelist.Size = new System.Drawing.Size(364, 252);
            this.listBoxWhitelist.TabIndex = 2;
            // 
            // buttonAddItem
            // 
            this.buttonAddItem.Location = new System.Drawing.Point(428, 81);
            this.buttonAddItem.Name = "buttonAddItem";
            this.buttonAddItem.Size = new System.Drawing.Size(52, 50);
            this.buttonAddItem.TabIndex = 3;
            this.buttonAddItem.Text = "+";
            this.buttonAddItem.UseVisualStyleBackColor = true;
            this.buttonAddItem.Click += new System.EventHandler(this.buttonAddItem_Click);
            // 
            // buttonDeleteItem
            // 
            this.buttonDeleteItem.Location = new System.Drawing.Point(428, 201);
            this.buttonDeleteItem.Name = "buttonDeleteItem";
            this.buttonDeleteItem.Size = new System.Drawing.Size(52, 50);
            this.buttonDeleteItem.TabIndex = 4;
            this.buttonDeleteItem.Text = "-";
            this.buttonDeleteItem.UseVisualStyleBackColor = true;
            this.buttonDeleteItem.Click += new System.EventHandler(this.buttonDeleteItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 448);
            this.Controls.Add(this.buttonDeleteItem);
            this.Controls.Add(this.buttonAddItem);
            this.Controls.Add(this.listBoxWhitelist);
            this.Controls.Add(this.buttonEnd);
            this.Controls.Add(this.buttonBegin);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Silencer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Button buttonBegin;
        private Button buttonEnd;
        private ListBox listBoxWhitelist;
        private Button buttonAddItem;
        private Button buttonDeleteItem;
    }
}