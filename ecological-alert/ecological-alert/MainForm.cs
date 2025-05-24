using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;

namespace ecological_alert
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axToolbarControl1.SetBuddyControl(axMapControl1);
            axTOCControl1.SetBuddyControl(axMapControl1);
        }

        private void 缓冲区分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BufferForm form = new BufferForm(this.axMapControl1);
            form.ShowDialog();
        }

        private void 赋权重ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WeightForm form = new WeightForm(this.axMapControl1);
            form.ShowDialog();
        }
    }
}