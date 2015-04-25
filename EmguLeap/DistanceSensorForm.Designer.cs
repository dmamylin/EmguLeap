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
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.Image)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Angle)).BeginInit();
			this.SuspendLayout();
			// 
			// Image
			// 
			this.Image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.Image.Location = new System.Drawing.Point(16, 15);
			this.Image.Margin = new System.Windows.Forms.Padding(4);
			this.Image.Name = "Image";
			this.Image.Size = new System.Drawing.Size(759, 292);
			this.Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.Image.TabIndex = 0;
			this.Image.TabStop = false;
			// 
			// Angle
			// 
			this.Angle.Location = new System.Drawing.Point(783, 15);
			this.Angle.Margin = new System.Windows.Forms.Padding(4);
			this.Angle.Maximum = 74;
			this.Angle.Minimum = -75;
			this.Angle.Name = "Angle";
			this.Angle.Size = new System.Drawing.Size(189, 56);
			this.Angle.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(783, 121);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Distance (cm):";
			// 
			// amount
			// 
			this.amount.AutoSize = true;
			this.amount.Location = new System.Drawing.Point(848, 137);
			this.amount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.amount.Name = "amount";
			this.amount.Size = new System.Drawing.Size(16, 17);
			this.amount.TabIndex = 3;
			this.amount.Text = "0";
			this.amount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(786, 165);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Raw distance:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(848, 182);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 17);
			this.label3.TabIndex = 5;
			this.label3.Text = "0";
			// 
			// DistanceSensorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(988, 321);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.amount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Angle);
			this.Controls.Add(this.Image);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}