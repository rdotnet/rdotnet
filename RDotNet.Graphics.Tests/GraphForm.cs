using System;
using System.Windows.Forms;

namespace RDotNet.Graphics
{
    public partial class GraphForm : Form
    {
        public GraphForm()
        {
            InitializeComponent();
        }

        public string Code { get; set; }

        public string TempImagePath { get; set; }

        private void plotButton_Click(object sender, EventArgs e)
        {
            var engine = REngine.GetInstance();
            engine.Evaluate(Code);
            engine.Evaluate(string.Format("png('{0}', {1}, {2})", TempImagePath.Replace('\\', '/'), this.pictureBox.Width, this.pictureBox.Height));
            engine.Evaluate(Code);
            engine.Evaluate("dev.off()");
            this.pictureBox.ImageLocation = TempImagePath;
        }
    }
}