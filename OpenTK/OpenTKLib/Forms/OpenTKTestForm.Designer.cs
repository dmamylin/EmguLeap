using OpenTKLib;

namespace OpenTKLib
{
    partial class OpenTKTestForm
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
            this.panelOpenKinect = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelOpenKinect
            // 
            this.panelOpenKinect.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelOpenKinect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOpenKinect.Location = new System.Drawing.Point(0, 0);
            this.panelOpenKinect.Name = "panelOpenKinect";
            this.panelOpenKinect.Size = new System.Drawing.Size(1312, 651);
            this.panelOpenKinect.TabIndex = 1;
            // 
            // OpenTKTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1312, 651);
            this.Controls.Add(this.panelOpenKinect);
            this.Name = "OpenTKTestForm";
            this.Text = "OpenTKTest";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelOpenKinect;

       
    }
}