namespace EmguLeap
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
			this.left = new System.Windows.Forms.PictureBox();
			this.right = new System.Windows.Forms.PictureBox();
			this.disparity = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.left)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.right)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.disparity)).BeginInit();
			this.SuspendLayout();
			// 
			// left
			// 
			this.left.Location = new System.Drawing.Point(12, 12);
			this.left.Name = "left";
			this.left.Size = new System.Drawing.Size(266, 175);
			this.left.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.left.TabIndex = 0;
			this.left.TabStop = false;
			// 
			// right
			// 
			this.right.Location = new System.Drawing.Point(283, 12);
			this.right.Name = "right";
			this.right.Size = new System.Drawing.Size(266, 175);
			this.right.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.right.TabIndex = 1;
			this.right.TabStop = false;
			// 
			// disparity
			// 
			this.disparity.Location = new System.Drawing.Point(12, 203);
			this.disparity.Name = "disparity";
			this.disparity.Size = new System.Drawing.Size(537, 218);
			this.disparity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.disparity.TabIndex = 2;
			this.disparity.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(561, 433);
			this.Controls.Add(this.disparity);
			this.Controls.Add(this.right);
			this.Controls.Add(this.left);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.Text = "MainForm";
			((System.ComponentModel.ISupportInitialize)(this.left)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.right)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.disparity)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox left;
		private System.Windows.Forms.PictureBox right;
		private System.Windows.Forms.PictureBox disparity;
	}
}