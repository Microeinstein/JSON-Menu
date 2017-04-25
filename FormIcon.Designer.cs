namespace Micro.Menu {
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.larger = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.view = new System.Windows.Forms.ListView();
            this.txtPath = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(501, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.refresh);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtResult.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtResult.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtResult.Location = new System.Drawing.Point(58, 347);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(507, 20);
            this.txtResult.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 350);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Result:";
            // 
            // larger
            // 
            this.larger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.larger.AutoSize = true;
            this.larger.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.larger.Checked = true;
            this.larger.CheckState = System.Windows.Forms.CheckState.Checked;
            this.larger.Location = new System.Drawing.Point(481, 34);
            this.larger.Name = "larger";
            this.larger.Size = new System.Drawing.Size(84, 17);
            this.larger.TabIndex = 4;
            this.larger.Text = "Larger icons";
            this.larger.UseVisualStyleBackColor = true;
            this.larger.CheckedChanged += new System.EventHandler(this.switchSmallLarge);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Preview:";
            // 
            // view
            // 
            this.view.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.view.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.view.HideSelection = false;
            this.view.Location = new System.Drawing.Point(12, 51);
            this.view.MultiSelect = false;
            this.view.Name = "view";
            this.view.ShowGroups = false;
            this.view.Size = new System.Drawing.Size(553, 290);
            this.view.TabIndex = 5;
            this.view.UseCompatibleStateImageBehavior = false;
            this.view.SelectedIndexChanged += new System.EventHandler(this.viewSelect);
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtPath.Items.AddRange(new object[] {
            "C:\\Windows\\System32\\shell32.dll",
            "C:\\Windows\\System32\\imageres.dll",
            "C:\\Windows\\System32\\DDORes.dll",
            "C:\\Windows\\System32\\setupapi.dll",
            "C:\\Windows\\System32\\pifmgr.dll",
            "C:\\Windows\\System32\\moricons.dll",
            "C:\\Windows\\System32\\compstui.dll",
            "C:\\Windows\\System32\\connect.dll",
            "C:\\Windows\\System32\\user32.dll",
            "C:\\Windows\\System32\\wdc.dll",
            "C:\\Windows\\System32\\Vault.dll",
            "C:\\Windows\\System32\\WorkFoldersRes.dll",
            "C:\\Windows\\System32\\themecpl.dll",
            "C:\\Windows\\System32\\PhotoScreensaver.scr",
            "C:\\Windows\\System32\\desk.cpl",
            "C:\\Windows\\System32\\telephon.cpl",
            "C:\\Windows\\System32\\wusa.exe"});
            this.txtPath.Location = new System.Drawing.Point(58, 6);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(437, 21);
            this.txtPath.TabIndex = 1;
            this.txtPath.Text = "C:\\Windows\\System32\\imageres.dll";
            this.txtPath.SelectedIndexChanged += new System.EventHandler(this.txtPathSelect);
            this.txtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPathKey);
            // 
            // FormIcon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 379);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.view);
            this.Controls.Add(this.larger);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormIcon";
            this.Text = "Find icon";
            this.Shown += new System.EventHandler(this.refresh);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox larger;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView view;
        private System.Windows.Forms.ComboBox txtPath;
    }
}