using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.DataSourcesFile;

namespace ecological_alert
{
    public partial class mask : Form
    {
        private AxMapControl _mapControl;
        public string CustomTempPath { get; set; }


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
                    clbRasterLayers.Items.Add(layer.Name, false);
                }
            }
        }

        // 加载矢量图层到复选框列表（支持多边形和线要素）
        private void LoadVectorLayers()
        {
            clbVectorLayers.Items.Clear();
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer is IFeatureLayer featureLayer)
                {
                    IFeatureClass featureClass = featureLayer.FeatureClass;
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon ||
                        featureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        clbVectorLayers.Items.Add(layer.Name, false);
                    }
                }
            }
        }

        // 执行栅格掩膜提取
        private void ClipRaster(IRasterLayer inputRasterLayer, IFeatureClass featureClass, string outputName)
        {
            IRasterDataset maskRaster = null;
            IWorkspace workspace = null;
            IWorkspaceFactory workspaceFactory = null;

            try
            {
                // 验证输入数据
                ValidateSpatialReference(inputRasterLayer, featureClass);

                // 转换矢量到临时栅格
                maskRaster = ConvertFeatureToRaster(featureClass, inputRasterLayer.Raster, out workspaceFactory, out workspace);

                // 执行掩膜提取
                IExtractionOp extractionOp = new RasterExtractionOpClass();
                IRaster outputRaster = (IRaster)extractionOp.Raster(
                    (IGeoDataset)inputRasterLayer.Raster,
                    (IGeoDataset)maskRaster
                );

                // 保存结果
                SaveRasterDataset(outputRaster, outputName);
            }
            catch (COMException comEx)
            {
                HandleComException(comEx);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 清理资源
                if (maskRaster != null) Marshal.ReleaseComObject(maskRaster);
                if (workspace != null) Marshal.ReleaseComObject(workspace);
                if (workspaceFactory != null) Marshal.ReleaseComObject(workspaceFactory);
            }
        }

        private IRasterDataset ConvertFeatureToRaster(IFeatureClass featureClass, IRaster templateRaster,
            out IWorkspaceFactory workspaceFactory, out IWorkspace workspace)
        {
            workspaceFactory = new RasterWorkspaceFactoryClass();
            workspace = null;
            IRasterDataset rasterDataset = null;

            try
            {
                // 创建临时目录
                string tempPath = GetValidTempPath();
                workspace = workspaceFactory.OpenFromFile(tempPath, 0);

                // 设置转换环境
                IConversionOp conversionOp = new RasterConversionOpClass();
                IRasterAnalysisEnvironment env = (IRasterAnalysisEnvironment)conversionOp;
                env.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, ((IRasterProps)templateRaster).MeanCellSize().X);
                env.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvValue, ((IGeoDataset)templateRaster).Extent);

                // 执行转换
                rasterDataset = conversionOp.ToRasterDataset(
                    (IGeoDataset)featureClass,
                    "TIFF",
                    workspace,
                    Guid.NewGuid().ToString("N")
                );

                return rasterDataset;
            }
            catch
            {
                // 清理部分创建的对象
                if (rasterDataset != null) Marshal.ReleaseComObject(rasterDataset);
                if (workspace != null) Marshal.ReleaseComObject(workspace);
                if (workspaceFactory != null) Marshal.ReleaseComObject(workspaceFactory);
                throw;
            }
        }

        private void SaveRasterDataset(IRaster raster, string outputName)
        {
            IWorkspace workspace = null;
            IWorkspaceFactory workspaceFactory = null;
            IRasterDataset outputDataset = null;

            try
            {
                // 创建输出路径
                string tempPath = GetValidTempPath();
                string outputFileName = $"{outputName}_{Guid.NewGuid().ToString("N")}.tif";
                string outputPath = System.IO.Path.Combine(tempPath, outputFileName);

                // 创建工作空间
                workspaceFactory = new RasterWorkspaceFactoryClass();
                workspace = workspaceFactory.OpenFromFile(tempPath, 0);

                // 保存数据集
                outputDataset = ((IRaster2)raster).RasterDataset;
                ISaveAs saveAs = (ISaveAs)outputDataset;
                saveAs.SaveAs(outputFileName, workspace, "TIFF");

                // 验证文件生成
                if (!File.Exists(outputPath))
                {
                    throw new FileNotFoundException("输出文件创建失败");
                }

                // 添加图层到地图
                Invoke((MethodInvoker)delegate
                {
                    IRasterLayer newLayer = new RasterLayerClass();
                    newLayer.CreateFromFilePath(outputPath);
                    newLayer.Name = outputName;
                    _mapControl.AddLayer(newLayer, 0);
                    _mapControl.Refresh();
                });
            }
            finally
            {
                if (outputDataset != null) Marshal.ReleaseComObject(outputDataset);
                if (workspace != null) Marshal.ReleaseComObject(workspace);
                if (workspaceFactory != null) Marshal.ReleaseComObject(workspaceFactory);
            }
        }

        #region Helper Methods
        private string GetValidTempPath()
        {
            string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ArcGIS_Temp");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }

        private void ValidateSpatialReference(IRasterLayer rasterLayer, IFeatureClass featureClass)
        {
            ISpatialReference rasterSR = ((IGeoDataset)rasterLayer.Raster).SpatialReference;
            ISpatialReference featureSR = ((IGeoDataset)featureClass).SpatialReference;


        }

        private void HandleComException(COMException comEx)
        {
            string errorMsg = $"COM错误 (0x{comEx.ErrorCode:X8}):\n";
            switch ((uint)comEx.ErrorCode)
            {
                case 0x80004005:
                    errorMsg += "未指定的错误，请检查：\n- 文件写入权限\n- 磁盘空间\n- 路径有效性";
                    break;
                case 0x80070002:
                    errorMsg += "文件未找到，请检查临时目录路径";
                    break;
                case 0x80040301:
                    errorMsg += "空间参考不匹配";
                    break;
                default:
                    errorMsg += comEx.Message;
                    break;
            }

            MessageBox.Show(errorMsg, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

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
                MessageBox.Show("请至少选择一个矢量图层！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 获取选中的栅格图层
            List<IRasterLayer> rasterLayers = new List<IRasterLayer>();
            foreach (var item in clbRasterLayers.CheckedItems)
            {
                string name = item.ToString();
                IRasterLayer layer = FindRasterLayer(name);
                if (layer != null) rasterLayers.Add(layer);
            }

            // 获取选中的矢量图层
            List<IFeatureLayer> vectorLayers = new List<IFeatureLayer>();
            foreach (var item in clbVectorLayers.CheckedItems)
            {
                string name = item.ToString();
                IFeatureLayer layer = FindVectorLayer(name);
                if (layer != null) vectorLayers.Add(layer);
            }

            // 遍历所有组合执行掩膜提取
            foreach (IRasterLayer rasterLayer in rasterLayers)
            {
                foreach (IFeatureLayer vectorLayer in vectorLayers)
                {
                    try
                    {
                        string outputName = $"{rasterLayer.Name}_掩膜_{vectorLayer.Name}";
                        ClipRaster(rasterLayer, vectorLayer.FeatureClass, outputName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"处理 {vectorLayer.Name} 时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

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

        private IFeatureLayer FindVectorLayer(string layerName)
        {
            for (int i = 0; i < _mapControl.Map.LayerCount; i++)
            {
                ILayer layer = _mapControl.Map.get_Layer(i);
                if (layer.Name == layerName && layer is IFeatureLayer featureLayer)
                {
                    IFeatureClass featureClass = featureLayer.FeatureClass;
                    if (featureClass.ShapeType == esriGeometryType.esriGeometryPolygon ||
                        featureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        return featureLayer;
                    }
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