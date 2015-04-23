﻿namespace OpenCLTemplate
{
    partial class frmCLEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCLEdit));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rTBCLCode = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treeCLRef = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnLogs = new System.Windows.Forms.Button();
            this.btnCompileTest = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rTBCLCode);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(665, 446);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OpenCL Code";
            // 
            // rTBCLCode
            // 
            this.rTBCLCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rTBCLCode.Location = new System.Drawing.Point(6, 19);
            this.rTBCLCode.Name = "rTBCLCode";
            this.rTBCLCode.Size = new System.Drawing.Size(653, 421);
            this.rTBCLCode.TabIndex = 0;
            this.rTBCLCode.Text = "";
            this.rTBCLCode.WordWrap = false;
            this.rTBCLCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rTBCLCode_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.treeCLRef);
            this.groupBox2.Location = new System.Drawing.Point(683, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(259, 382);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Quick Reference";
            // 
            // treeCLRef
            // 
            this.treeCLRef.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeCLRef.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeCLRef.Location = new System.Drawing.Point(6, 19);
            this.treeCLRef.Name = "treeCLRef";
            this.treeCLRef.ShowNodeToolTips = true;
            this.treeCLRef.Size = new System.Drawing.Size(247, 357);
            this.treeCLRef.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnLogs);
            this.groupBox3.Controls.Add(this.btnCompileTest);
            this.groupBox3.Location = new System.Drawing.Point(683, 400);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(259, 58);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Compilation Test";
            // 
            // btnLogs
            // 
            this.btnLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogs.Location = new System.Drawing.Point(110, 19);
            this.btnLogs.Name = "btnLogs";
            this.btnLogs.Size = new System.Drawing.Size(143, 33);
            this.btnLogs.TabIndex = 0;
            this.btnLogs.Text = "View build logs...";
            this.btnLogs.UseVisualStyleBackColor = true;
            this.btnLogs.Click += new System.EventHandler(this.btnLogs_Click);
            // 
            // btnCompileTest
            // 
            this.btnCompileTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompileTest.Location = new System.Drawing.Point(6, 19);
            this.btnCompileTest.Name = "btnCompileTest";
            this.btnCompileTest.Size = new System.Drawing.Size(98, 33);
            this.btnCompileTest.TabIndex = 0;
            this.btnCompileTest.Text = "F5 - Test code";
            this.btnCompileTest.UseVisualStyleBackColor = true;
            this.btnCompileTest.Click += new System.EventHandler(this.btnCompileTest_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 461);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(954, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel.Text = "toolStripStatusLabel1";
            // 
            // frmCLEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 483);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCLEdit";
            this.Text = "OpenCL Editor";
            this.Load += new System.EventHandler(this.frmCLEdit_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCLEdit_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView treeCLRef;
        private System.Windows.Forms.RichTextBox rTBCLCode;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCompileTest;
        private System.Windows.Forms.Button btnLogs;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}