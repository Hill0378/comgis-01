using System;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.AnalysisTools;

namespace ecological_alert
{
    public partial class BufferForm : Form
    {
        public BufferForm()
        {
            InitializeComponent();
        }

        // 接收MapControl中的数据
        private IHookHelper mHookHelper = new HookHelperClass();
        // 缓冲区文件输出路径
        public string strOutputPath;

        // 重写构造函数，添加参数hook，用于传入MapControl中的数据
        public BufferForm(object hook)
        {
            InitializeComponent();
            this.mHookHelper.Hook = hook;
        }

        private IFeatureLayer GetFeatureLayer(string layerName)
        {
            for (int i = 0; i < this.mHookHelper.FocusMap.LayerCount; i++)
            {
                ILayer pLayer = this.mHookHelper.FocusMap.get_Layer(i);
                if (pLayer.Name == layerName)
                {
                    return pLayer as IFeatureLayer;
                }
            }
            return null;
        }

        private void BufferForm_Load(object sender, EventArgs e)
        {
            // 传入数据为空时返回
            if (null == mHookHelper || null == mHookHelper.Hook || 0 == mHookHelper.FocusMap.LayerCount)
            {
                MessageBox.Show("地图中没有可用的图层!", "错误");
                this.Close();
                return;
            }

            // 获取图层名称并加入cboLayers
            for (int i = 0; i < this.mHookHelper.FocusMap.LayerCount; i++)
            {
                ILayer pLayer = this.mHookHelper.FocusMap.get_Layer(i);
                cboLayers.Items.Add(pLayer.Name);
            }

            // 默认选择第一个图层
            if (cboLayers.Items.Count > 0)
                cboLayers.SelectedIndex = 0;
        }

        private void btnOutputLayer_Click(object sender, EventArgs e)
        {
            // 使用保存文件对话框让用户选择输出路径和文件名
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                CheckPathExists = true,
                Filter = "Shapefile (*.shp)|*.shp",
                OverwritePrompt = true,
                Title = "选择缓冲区输出位置",
                RestoreDirectory = true
            };

            // 设置默认文件名
            if (cboLayers.SelectedItem != null)
            {
                saveDlg.FileName = $"{cboLayers.SelectedItem}_buffer.shp";
            }

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = saveDlg.FileName;
            }
        }

        private void btnBuffer_Click(object sender, EventArgs e)
        {
            // 验证缓冲半径输入
            if (!double.TryParse(txtBufferDistance.Text, out double bufferDistance) || bufferDistance <= 0)
            {
                MessageBox.Show("请输入有效的缓冲半径(必须为正数)!", "输入错误");
                txtBufferDistance.Focus();
                return;
            }

            // 验证输出路径
            if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
            {
                MessageBox.Show("请先选择输出路径!", "路径错误");
                btnOutputLayer.Focus();
                return;
            }

            try
            {
                string dir = Path.GetDirectoryName(txtOutputPath.Text);
                string ext = Path.GetExtension(txtOutputPath.Text);

                if (!Directory.Exists(dir) || ext.ToLower() != ".shp")
                {
                    MessageBox.Show("输出路径无效或文件扩展名不是.shp!", "路径错误");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"路径错误: {ex.Message}", "错误");
                return;
            }

            // 验证图层选择
            if (cboLayers.SelectedItem == null)
            {
                MessageBox.Show("请选择要分析的图层!", "选择错误");
                return;
            }

            // 获取图层
            IFeatureLayer pFeatureLayer = GetFeatureLayer(cboLayers.SelectedItem.ToString());
            if (pFeatureLayer == null)
            {
                MessageBox.Show($"图层 {cboLayers.SelectedItem} 不存在!", "错误");
                return;
            }

            // 执行缓冲区分析
            try
            {
                Geoprocessor gp = new Geoprocessor { OverwriteOutput = true };
                var buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(pFeatureLayer, txtOutputPath.Text, bufferDistance.ToString());

                IGeoProcessorResult results = (IGeoProcessorResult)gp.Execute(buffer, null);

                if (results.Status != esriJobStatus.esriJobSucceeded)
                {
                    MessageBox.Show($"缓冲区生成失败! 错误信息: {results.GetMessages(-1)}", "错误");
                }
                else
                {
                    this.strOutputPath = txtOutputPath.Text;
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show($"成功生成缓冲区!\n半径: {bufferDistance}\n输出位置: {txtOutputPath.Text}", "成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"执行缓冲区分析时出错: {ex.Message}", "错误");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}