using System.Windows.Forms;

namespace EmguLeap
{
	partial class Distances
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
			this.Name = new System.Windows.Forms.Label();
			this.amount = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Name
			// 
			this.Name.AutoSize = true;
			this.Name.Location = new System.Drawing.Point(129, 54);
			this.Name.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.Name.Name = "Name";
			this.Name.Size = new System.Drawing.Size(97, 13);
			this.Name.TabIndex = 0;
			this.Name.Text = "Distance to center:";
			// 
			// amount
			// 
			this.amount.AutoSize = true;
			this.amount.Location = new System.Drawing.Point(129, 76);
			this.amount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.amount.Name = "amount";
			this.amount.Size = new System.Drawing.Size(13, 13);
			this.amount.TabIndex = 1;
			this.amount.Text = "0";
			// 
			// Distances
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(356, 190);
			this.Controls.Add(this.amount);
			this.Controls.Add(this.Name);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Text = "Distances";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label Name;
		private System.Windows.Forms.Label amount;
	}
}