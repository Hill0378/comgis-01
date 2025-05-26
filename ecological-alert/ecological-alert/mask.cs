using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.GeoAnalyst;

namespace ecological_alert
{
    public partial class mask : Form
    {
        private AxMapControl _mapControl;

        public mask(AxMapControl axMapControl1)
        {
            InitializeComponent();
            _mapControl = axMapControl1;
            LoadRasterLayers();
            LoadVectorLayers();
        }

        // 加载栅格图层到复选框列表
        private void LoadRasterLayers()
        {
            clbRasterLayers.Items.Clear();
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer is IRasterLayer)
                {
                    clbRasterLayers.Items.Add(layer.Name, true);
                }
            }
        }

        // 加载多边形矢量图层到复选框列表
        private void LoadVectorLayers()
        {
            clbVectorLayers.Items.Clear();
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer is IFeatureLayer featureLayer)
                {
                    IFeatureClass featureClass = featureLayer.FeatureClass;
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        clbVectorLayers.Items.Add(layer.Name, true);
                    }
                }
            }
        }

        // 执行栅格掩膜提取
        private void ClipRaster(IRaster raster, IPolygon clipPolygon, string outputName)
        {
            try
            {
                // 设置分析环境
                IExtractionOp extractionOp = new RasterExtractionOpClass();
                IRasterAnalysisEnvironment env = (IRasterAnalysisEnvironment)extractionOp;
                env.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, ((IRasterProps)raster).MeanCellSize().X);
                env.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvValue, clipPolygon.Envelope);

                // 执行掩膜提取
                IGeoDataset outputDataset = extractionOp.Polygon((IGeoDataset)raster, clipPolygon, true);

                // 保存结果并添加到地图
                SaveAndDisplayResult(outputDataset, outputName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"掩膜提取失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 保存并显示结果栅格
        //private void SaveAndDisplayResult(IGeoDataset geoDataset, string layerName)
        //{
        //    IRasterLayer rasterLayer = new RasterLayer();
        //    rasterLayer.CreateFromRaster((IRaster)geoDataset);
        //    rasterLayer.Name = layerName;

        //    // 添加到地图控件
        //    _mapControl.AddLayer(rasterLayer, 0);
        //    _mapControl.Refresh();
        //}
        // 保存并显示结果栅格
        private void SaveAndDisplayResult(IGeoDataset geoDataset, string layerName)
        {
            try
            {
                // 设置输出路径
                string outputFolder = System.IO.Path.Combine(Application.StartupPath, "outputs");
                if (!System.IO.Directory.Exists(outputFolder))
                    System.IO.Directory.CreateDirectory(outputFolder);

                string outputPath = System.IO.Path.Combine(outputFolder, layerName + ".tif");

                // 创建工作空间
                IWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
                string folder = System.IO.Path.GetDirectoryName(outputPath);
                string name = System.IO.Path.GetFileName(outputPath);

                // 强制转换为 IRasterDataset
                IRasterDataset rasterDataset = (IRasterDataset)((ISaveAs)geoDataset).SaveAs(name, workspaceFactory.OpenFromFile(folder, 0), "TIFF");

                // 从保存的栅格创建图层
                IRasterLayer rasterLayer = new RasterLayerClass();
                rasterLayer.CreateFromDataset(rasterDataset);
                rasterLayer.Name = layerName;

                // 添加到地图控件
                _mapControl.AddLayer(rasterLayer, 0);
                _mapControl.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存输出栅格失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            // 输入验证
            if (clbRasterLayers.CheckedItems.Count == 0)
            {
                MessageBox.Show("请至少选择一个栅格图层！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (clbVectorLayers.CheckedItems.Count == 0)
            {
                MessageBox.Show("请至少选择一个多边形矢量图层！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 获取选中的栅格图层
            List<IRasterLayer> rasterLayers = new List<IRasterLayer>();
            foreach (var item in clbRasterLayers.CheckedItems)
            {
                string name = item.ToString(); // 显式转换为字符串
                IRasterLayer layer = FindRasterLayer(name);
                if (layer != null) rasterLayers.Add(layer);
            }

            // 获取选中的矢量图层
            List<IFeatureLayer> vectorLayers = new List<IFeatureLayer>();
            foreach (var item in clbVectorLayers.CheckedItems)
            {
                string name = item.ToString(); // 显式转换为字符串
                IFeatureLayer layer = FindVectorLayer(name);
                if (layer != null) vectorLayers.Add(layer);
            }


            // 遍历所有组合执行掩膜提取
            foreach (IRasterLayer rasterLayer in rasterLayers)
            {
                foreach (IFeatureLayer vectorLayer in vectorLayers)
                {
                    IFeatureCursor cursor = null;
                    try
                    {
                        cursor = vectorLayer.FeatureClass.Search(new QueryFilter(), true);
                        IFeature feature = cursor.NextFeature();
                        if (feature?.Shape is IPolygon polygon)
                        {
                            string outputName = $"{rasterLayer.Name}_掩膜_{vectorLayer.Name}";
                            ClipRaster(rasterLayer.Raster, polygon, outputName);
                        }
                        else
                        {
                            MessageBox.Show($"矢量图层 {vectorLayer.Name} 中未找到有效多边形！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"处理 {vectorLayer.Name} 时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // 释放COM对象
                        if (cursor != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(cursor);
                    }
                }
            }

            // 关闭对话框
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 查找栅格图层
        private IRasterLayer FindRasterLayer(string layerName)
        {
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer.Name == layerName && layer is IRasterLayer rasterLayer)
                {
                    return rasterLayer;
                }
            }
            return null;
        }

        // 查找矢量图层
        private IFeatureLayer FindVectorLayer(string layerName)
        {
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer.Name == layerName && layer is IFeatureLayer featureLayer &&
                    featureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    return featureLayer;
                }
            }
            return null;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


    }
}