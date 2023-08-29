namespace Compress.Win
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusBox = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bAdd = new System.Windows.Forms.ToolStripButton();
            this.bExtract = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbAddress = new System.Windows.Forms.ToolStripTextBox();
            this.bUp = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBox
            // 
            this.statusBox.Location = new System.Drawing.Point(0, 456);
            this.statusBox.Name = "statusBox";
            this.statusBox.Size = new System.Drawing.Size(826, 22);
            this.statusBox.TabIndex = 0;
            this.statusBox.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bAdd,
            this.bExtract,
            this.toolStripSeparator1,
            this.tbAddress,
            this.bUp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(826, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bAdd
            // 
            this.bAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bAdd.Image = ((System.Drawing.Image)(resources.GetObject("bAdd.Image")));
            this.bAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(33, 22);
            this.bAdd.Text = "Add";
            // 
            // bExtract
            // 
            this.bExtract.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bExtract.Image = ((System.Drawing.Image)(resources.GetObject("bExtract.Image")));
            this.bExtract.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bExtract.Name = "bExtract";
            this.bExtract.Size = new System.Drawing.Size(46, 22);
            this.bExtract.Text = "Extract";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbAddress
            // 
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(300, 25);
            // 
            // bUp
            // 
            this.bUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bUp.Image = ((System.Drawing.Image)(resources.GetObject("bUp.Image")));
            this.bUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(26, 22);
            this.bUp.Text = "Up";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 28);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(826, 425);
            this.dataGridView1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 478);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusBox);
            this.Name = "MainForm";
            this.Text = "Compress";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bAdd;
        private System.Windows.Forms.ToolStripButton bExtract;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox tbAddress;
        private System.Windows.Forms.ToolStripButton bUp;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}

