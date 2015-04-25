namespace EmguLeap
{
	partial class DistanceSensorForm
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
			this.Image = new System.Windows.Forms.PictureBox();
			this.Angle = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.amount = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.Image)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Angle)).BeginInit();
			this.SuspendLayout();
			// 
			// Image
			// 
			this.Image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.Image.Location = new System.Drawing.Point(12, 12);
			this.Image.Name = "Image";
			this.Image.Size = new System.Drawing.Size(569, 237);
			this.Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.Image.TabIndex = 0;
			this.Image.TabStop = false;
			// 
			// Angle
			// 
			this.Angle.Location = new System.Drawing.Point(587, 12);
			this.Angle.Maximum = 75;
			this.Angle.Minimum = -75;
			this.Angle.Name = "Angle";
			this.Angle.Size = new System.Drawing.Size(142, 45);
			this.Angle.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(587, 98);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Distance is:";
			// 
			// amount
			// 
			this.amount.AutoSize = true;
			this.amount.Location = new System.Drawing.Point(636, 111);
			this.amount.Name = "amount";
			this.amount.Size = new System.Drawing.Size(13, 13);
			this.amount.TabIndex = 3;
			this.amount.Text = "0";
			this.amount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// DistanceSensorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(741, 261);
			this.Controls.Add(this.amount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Angle);
			this.Controls.Add(this.Image);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "DistanceSensorForm";
			this.Text = "DistanceSensor";
			((System.ComponentModel.ISupportInitialize)(this.Image)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Angle)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox Image;
		private System.Windows.Forms.TrackBar Angle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label amount;
	}
}