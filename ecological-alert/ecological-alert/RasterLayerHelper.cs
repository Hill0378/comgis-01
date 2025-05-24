using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Windows.Forms;

namespace ecological_alert
{
    public static class RasterLayerHelper
    {
        public static IRasterLayer CreateRasterLayerFromFile(string filePath)
        {
            try
            {
                // 打开栅格数据集
                RasterWorkspaceFactory workspaceFactory = new RasterWorkspaceFactory();
                string directory = System.IO.Path.GetDirectoryName(filePath);
                string fileName = System.IO.Path.GetFileName(filePath);

                IRasterWorkspace rasterWorkspace = (IRasterWorkspace)workspaceFactory.OpenFromFile(directory, 0);
                IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);

                // 创建包含所有波段的栅格
                IRaster raster = ((IRasterDataset2)rasterDataset).CreateFullRaster();

                // 创建栅格图层
                IRasterLayer rasterLayer = new RasterLayerClass();
                rasterLayer.CreateFromRaster(raster);
                rasterLayer.Name = System.IO.Path.GetFileNameWithoutExtension(filePath);

                return rasterLayer;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建栅格图层失败: {ex.Message}");
                return null;
            }
        }
    }
}