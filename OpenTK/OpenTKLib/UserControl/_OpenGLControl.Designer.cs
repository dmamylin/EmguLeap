using OpenTK;

namespace OpenTKLib
{
    partial class OpenGLControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenGLControl));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNormalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triangulateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convexHullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triangulateNearestNeighbourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveNormalsOfCurrentModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonColor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.comboViewMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.comboModels = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.comboCamera = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.comboRightMouse = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.comboTransparency = new System.Windows.Forms.ToolStripComboBox();
            this.glControl1 = new OpenTK.GLControl();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1233, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadModelToolStripMenuItem,
            this.testToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadModelToolStripMenuItem
            // 
            this.loadModelToolStripMenuItem.Name = "loadModelToolStripMenuItem";
            this.loadModelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadModelToolStripMenuItem.Text = "Load Model";
            this.loadModelToolStripMenuItem.Click += new System.EventHandler(this.loadModelToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.testToolStripMenuItem.Text = "Test Sphere";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAxesToolStripMenuItem,
            this.showNormalsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // showAxesToolStripMenuItem
            // 
            this.showAxesToolStripMenuItem.Name = "showAxesToolStripMenuItem";
            this.showAxesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showAxesToolStripMenuItem.Text = "Show Axis";
            this.showAxesToolStripMenuItem.Click += new System.EventHandler(this.showAxesToolStripMenuItem_Click);
            // 
            // showNormalsToolStripMenuItem
            // 
            this.showNormalsToolStripMenuItem.Name = "showNormalsToolStripMenuItem";
            this.showNormalsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showNormalsToolStripMenuItem.Text = "Show Normals";
            this.showNormalsToolStripMenuItem.Click += new System.EventHandler(this.showNormalsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeSelectedModelToolStripMenuItem,
            this.triangulateToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // removeSelectedModelToolStripMenuItem
            // 
            this.removeSelectedModelToolStripMenuItem.Name = "removeSelectedModelToolStripMenuItem";
            this.removeSelectedModelToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.removeSelectedModelToolStripMenuItem.Text = "Remove Selected Model";
            this.removeSelectedModelToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedModelToolStripMenuItem_Click);
            // 
            // triangulateToolStripMenuItem
            // 
            this.triangulateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convexHullToolStripMenuItem,
            this.triangulateNearestNeighbourToolStripMenuItem,
            this.saveNormalsOfCurrentModelToolStripMenuItem});
            this.triangulateToolStripMenuItem.Name = "triangulateToolStripMenuItem";
            this.triangulateToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.triangulateToolStripMenuItem.Text = "Triangulate";
            // 
            // convexHullToolStripMenuItem
            // 
            this.convexHullToolStripMenuItem.Name = "convexHullToolStripMenuItem";
            this.convexHullToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.convexHullToolStripMenuItem.Text = "Convex Hull";
            this.convexHullToolStripMenuItem.Click += new System.EventHandler(this.convexHullToolStripMenuItem_Click);
            // 
            // triangulateNearestNeighbourToolStripMenuItem
            // 
            this.triangulateNearestNeighbourToolStripMenuItem.Name = "triangulateNearestNeighbourToolStripMenuItem";
            this.triangulateNearestNeighbourToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.triangulateNearestNeighbourToolStripMenuItem.Text = "Triangulate - nearest neighbour";
            this.triangulateNearestNeighbourToolStripMenuItem.Click += new System.EventHandler(this.triangulateNearestNeighbourToolStripMenuItem_Click);
            // 
            // saveNormalsOfCurrentModelToolStripMenuItem
            // 
            this.saveNormalsOfCurrentModelToolStripMenuItem.Name = "saveNormalsOfCurrentModelToolStripMenuItem";
            this.saveNormalsOfCurrentModelToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.saveNormalsOfCurrentModelToolStripMenuItem.Text = "Save Normals of current model";
            this.saveNormalsOfCurrentModelToolStripMenuItem.Click += new System.EventHandler(this.saveNormalsOfCurrentModelToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonColor,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.comboViewMode,
            this.toolStripLabel2,
            this.comboModels,
            this.toolStripLabel3,
            this.comboCamera,
            this.toolStripLabel4,
            this.comboRightMouse,
            this.toolStripLabel5,
            this.comboTransparency});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1233, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonColor
            // 
            this.toolStripButtonColor.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStripButtonColor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStripButtonColor.BackgroundImage")));
            this.toolStripButtonColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonColor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonColor.Image")));
            this.toolStripButtonColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonColor.Name = "toolStripButtonColor";
            this.toolStripButtonColor.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonColor.Text = "toolStripButton1";
            this.toolStripButtonColor.ToolTipText = "Change the color of the selected model";
            this.toolStripButtonColor.Click += new System.EventHandler(this.toolStripButtonColor_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(66, 22);
            this.toolStripLabel1.Text = "View Mode";
            // 
            // comboViewMode
            // 
            this.comboViewMode.Name = "comboViewMode";
            this.comboViewMode.Size = new System.Drawing.Size(121, 25);
            this.comboViewMode.SelectedIndexChanged += new System.EventHandler(this.comboViewMode_SelectedIndexChanged);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(88, 22);
            this.toolStripLabel2.Text = "Selected Model";
            // 
            // comboModels
            // 
            this.comboModels.Name = "comboModels";
            this.comboModels.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(121, 22);
            this.toolStripLabel3.Text = "Left Mouse (Camera):";
            // 
            // comboCamera
            // 
            this.comboCamera.Items.AddRange(new object[] {
            "Rotate",
            "Translate"});
            this.comboCamera.Name = "comboCamera";
            this.comboCamera.Size = new System.Drawing.Size(121, 25);
            this.comboCamera.SelectedIndexChanged += new System.EventHandler(this.comboCamera_SelectedIndexChanged);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(122, 22);
            this.toolStripLabel4.Text = "Right Mouse (Model):";
            // 
            // comboRightMouse
            // 
            this.comboRightMouse.Items.AddRange(new object[] {
            "X Translate",
            "Y Translate",
            "Z Translate",
            "X Rotate",
            "Y Rotate",
            "Z Rotate"});
            this.comboRightMouse.Name = "comboRightMouse";
            this.comboRightMouse.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(78, 22);
            this.toolStripLabel5.Text = "Transparency";
            // 
            // comboTransparency
            // 
            this.comboTransparency.Name = "comboTransparency";
            this.comboTransparency.Size = new System.Drawing.Size(121, 25);
            this.comboTransparency.SelectedIndexChanged += new System.EventHandler(this.comboTransparency_SelectedIndexChanged);

            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 49);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(1233, 533);
            this.glControl1.TabIndex = 12;
            this.glControl1.VSync = false;
            // 
            // OpenGLControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "OpenGLControl";
            this.Size = new System.Drawing.Size(1233, 582);
            this.Load += new System.EventHandler(this.OpenGLControl_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonColor;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private GLControl glControl1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox comboViewMode;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox comboModels;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox comboCamera;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripComboBox comboRightMouse;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripComboBox comboTransparency;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triangulateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convexHullToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triangulateNearestNeighbourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveNormalsOfCurrentModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showNormalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    }
}
