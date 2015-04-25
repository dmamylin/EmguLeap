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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Name
			// 
			this.Name.AutoSize = true;
			this.Name.Location = new System.Drawing.Point(172, 66);
			this.Name.Name = "Name";
			this.Name.Size = new System.Drawing.Size(109, 17);
			this.Name.TabIndex = 0;
			this.Name.Text = "Distance to line:";
			// 
			// amount
			// 
			this.amount.AutoSize = true;
			this.amount.Location = new System.Drawing.Point(172, 83);
			this.amount.Name = "amount";
			this.amount.Size = new System.Drawing.Size(16, 17);
			this.amount.TabIndex = 1;
			this.amount.Text = "0";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(172, 131);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Raw distance:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(172, 148);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 17);
			this.label2.TabIndex = 3;
			this.label2.Text = "0";
			// 
			// Distances
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(475, 234);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.amount);
			this.Controls.Add(this.Name);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Text = "Distances";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label Name;
		private System.Windows.Forms.Label amount;
		private Label label1;
		private Label label2;
	}
}