namespace OpenTKLib
{
    partial class SettingsForm
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPointSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPointSizeAxis = new System.Windows.Forms.TextBox();
            this.buttonColorModels = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonColorBackground = new System.Windows.Forms.Button();
            this.buttonBackColor = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(85, 438);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(176, 23);
            this.buttonOK.TabIndex = 27;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(345, 438);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 23);
            this.button1.TabIndex = 28;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Point Size for point cloud";
            // 
            // textBoxPointSize
            // 
            this.textBoxPointSize.Location = new System.Drawing.Point(217, 68);
            this.textBoxPointSize.Name = "textBoxPointSize";
            this.textBoxPointSize.Size = new System.Drawing.Size(100, 20);
            this.textBoxPointSize.TabIndex = 30;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Point Size for axis";
            // 
            // textBoxPointSizeAxis
            // 
            this.textBoxPointSizeAxis.Location = new System.Drawing.Point(217, 101);
            this.textBoxPointSizeAxis.Name = "textBoxPointSizeAxis";
            this.textBoxPointSizeAxis.Size = new System.Drawing.Size(100, 20);
            this.textBoxPointSizeAxis.TabIndex = 32;
            // 
            // buttonColorModels
            // 
            this.buttonColorModels.Location = new System.Drawing.Point(217, 129);
            this.buttonColorModels.Name = "buttonColorModels";
            this.buttonColorModels.Size = new System.Drawing.Size(123, 23);
            this.buttonColorModels.TabIndex = 33;
            this.buttonColorModels.Text = "Change";
            this.buttonColorModels.UseVisualStyleBackColor = true;
            this.buttonColorModels.Click += new System.EventHandler(this.buttonColorBack_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(66, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Color of all Models";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Color of current model";
            // 
            // buttonColorBackground
            // 
            this.buttonColorBackground.Location = new System.Drawing.Point(217, 162);
            this.buttonColorBackground.Name = "buttonColorBackground";
            this.buttonColorBackground.Size = new System.Drawing.Size(123, 23);
            this.buttonColorBackground.TabIndex = 36;
            this.buttonColorBackground.Text = "Change";
            this.buttonColorBackground.UseVisualStyleBackColor = true;
            this.buttonColorBackground.Click += new System.EventHandler(this.buttonColorBackground_Click);
            // 
            // buttonBackColor
            // 
            this.buttonBackColor.Location = new System.Drawing.Point(217, 191);
            this.buttonBackColor.Name = "buttonBackColor";
            this.buttonBackColor.Size = new System.Drawing.Size(123, 23);
            this.buttonBackColor.TabIndex = 37;
            this.buttonBackColor.Text = "Change";
            this.buttonBackColor.UseVisualStyleBackColor = true;
            this.buttonBackColor.Click += new System.EventHandler(this.buttonBackColor_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(66, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Color of background";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 486);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonBackColor);
            this.Controls.Add(this.buttonColorBackground);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonColorModels);
            this.Controls.Add(this.textBoxPointSizeAxis);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPointSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonOK);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPointSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPointSizeAxis;
        private System.Windows.Forms.Button buttonColorModels;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonColorBackground;
        private System.Windows.Forms.Button buttonBackColor;
        private System.Windows.Forms.Label label5;

    }
}