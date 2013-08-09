using System;
using System.Windows.Forms;

namespace RDotNet.Graphics.Tests
{
	public partial class GraphForm : Form
	{
		public GraphForm()
		{
			InitializeComponent();
		}

		public REngine Engine { get; set; }
		public string Code { get; set; }
		public string TempImagePath { get; set; }

		private void plotButton_Click(object sender, EventArgs e)
		{
			Engine.Evaluate(Code);
			Engine.Evaluate(string.Format("png('{0}', {1}, {2})", TempImagePath.Replace('\\', '/'), this.pictureBox.Width, this.pictureBox.Height));
			Engine.Evaluate(Code);
			Engine.Evaluate("dev.off()");
			this.pictureBox.ImageLocation = TempImagePath;
		}
	}
}
