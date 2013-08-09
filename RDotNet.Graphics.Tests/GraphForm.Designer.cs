namespace RDotNet.Graphics.Tests
{
	partial class GraphForm
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
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
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
			this.successButton = new System.Windows.Forms.Button();
			this.failureButton = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.plotButton = new System.Windows.Forms.Button();
			this.graphPanel = new RDotNet.Graphics.Tests.GraphPanel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// successButton
			// 
			this.successButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.successButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.successButton.Location = new System.Drawing.Point(475, 346);
			this.successButton.Name = "successButton";
			this.successButton.Size = new System.Drawing.Size(75, 23);
			this.successButton.TabIndex = 1;
			this.successButton.Text = "Success";
			this.successButton.UseVisualStyleBackColor = true;
			// 
			// failureButton
			// 
			this.failureButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.failureButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.failureButton.Location = new System.Drawing.Point(556, 346);
			this.failureButton.Name = "failureButton";
			this.failureButton.Size = new System.Drawing.Size(75, 23);
			this.failureButton.TabIndex = 2;
			this.failureButton.Text = "Failure";
			this.failureButton.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.graphPanel);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.pictureBox);
			this.splitContainer1.Size = new System.Drawing.Size(644, 340);
			this.splitContainer1.SplitterDistance = 320;
			this.splitContainer1.TabIndex = 2;
			// 
			// pictureBox
			// 
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(320, 340);
			this.pictureBox.TabIndex = 4;
			this.pictureBox.TabStop = false;
			// 
			// plotButton
			// 
			this.plotButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.plotButton.Location = new System.Drawing.Point(12, 346);
			this.plotButton.Name = "plotButton";
			this.plotButton.Size = new System.Drawing.Size(457, 23);
			this.plotButton.TabIndex = 0;
			this.plotButton.Text = "Click here and check two images are same";
			this.plotButton.UseVisualStyleBackColor = true;
			this.plotButton.Click += new System.EventHandler(this.plotButton_Click);
			// 
			// graphPanel
			// 
			this.graphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphPanel.Location = new System.Drawing.Point(0, 0);
			this.graphPanel.Name = "graphPanel";
			this.graphPanel.Size = new System.Drawing.Size(320, 340);
			this.graphPanel.TabIndex = 0;
			// 
			// GraphForm
			// 
			this.AcceptButton = this.plotButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.failureButton;
			this.ClientSize = new System.Drawing.Size(643, 381);
			this.Controls.Add(this.plotButton);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.failureButton);
			this.Controls.Add(this.successButton);
			this.Name = "GraphForm";
			this.Text = "GraphForm";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button successButton;
		private System.Windows.Forms.Button failureButton;
		private System.Windows.Forms.SplitContainer splitContainer1;
		internal GraphPanel graphPanel;
		private System.Windows.Forms.Button plotButton;
		private System.Windows.Forms.PictureBox pictureBox;
	}
}