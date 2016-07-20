namespace Menu {
    partial class FormIcon {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormIcon));
            this.img16 = new System.Windows.Forms.PictureBox();
            this.numIcon = new System.Windows.Forms.NumericUpDown();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.img32 = new System.Windows.Forms.PictureBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.img16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img32)).BeginInit();
            this.SuspendLayout();
            // 
            // img16
            // 
            this.img16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.img16.BackColor = System.Drawing.Color.Transparent;
            this.img16.Location = new System.Drawing.Point(384, 35);
            this.img16.Name = "img16";
            this.img16.Size = new System.Drawing.Size(16, 16);
            this.img16.TabIndex = 0;
            this.img16.TabStop = false;
            // 
            // numIcon
            // 
            this.numIcon.Location = new System.Drawing.Point(87, 42);
            this.numIcon.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numIcon.Name = "numIcon";
            this.numIcon.Size = new System.Drawing.Size(84, 20);
            this.numIcon.TabIndex = 1;
            this.numIcon.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numIcon.ValueChanged += new System.EventHandler(this.refresh);
            // 
            // txtPath
            // 
            this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtPath.Location = new System.Drawing.Point(50, 16);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(317, 20);
            this.txtPath.TabIndex = 2;
            this.txtPath.Text = "C:\\Windows\\System32\\imageres.dll";
            this.txtPath.TextChanged += new System.EventHandler(this.refresh);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Icon number:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(373, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.refresh);
            // 
            // img32
            // 
            this.img32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.img32.BackColor = System.Drawing.Color.Transparent;
            this.img32.Location = new System.Drawing.Point(406, 19);
            this.img32.Name = "img32";
            this.img32.Size = new System.Drawing.Size(32, 32);
            this.img32.TabIndex = 0;
            this.img32.TabStop = false;
            // 
            // txtResult
            // 
            this.txtResult.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtResult.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtResult.Location = new System.Drawing.Point(58, 68);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(309, 20);
            this.txtResult.TabIndex = 2;
            this.txtResult.Text = "C:\\Windows\\System32\\imageres.dll,0";
            this.txtResult.TextChanged += new System.EventHandler(this.refresh);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Result:";
            // 
            // FormIcon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 102);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.numIcon);
            this.Controls.Add(this.img32);
            this.Controls.Add(this.img16);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormIcon";
            this.Text = "Find icon";
            this.Load += new System.EventHandler(this.FormIcon_Load);
            ((System.ComponentModel.ISupportInitialize)(this.img16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img32)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox img16;
        private System.Windows.Forms.NumericUpDown numIcon;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox img32;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label3;
    }
}