using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Carto;


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

        private void 年间差异ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RasterSubtractForm form = new RasterSubtractForm(this.axMapControl1);
            form.ShowDialog();

        }

        private void slope计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SlopeForm form = new SlopeForm(axMapControl1); // 注意替换为你的 MapControl 控件名
            form.ShowDialog();

        }

        private void 均值求差ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_MeanDifference form = new Form_MeanDifference(axMapControl1);
            form.ShowDialog();
        }

        private void 赋颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignColorForm form = new AssignColorForm(axMapControl1); // 传入地图控件
            form.ShowDialog();
        }
    }
}
