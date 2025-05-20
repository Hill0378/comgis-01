using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.SpatialAnalystTools;

namespace ecological_alert
{
    class RasterCalculator
    {
        public void SubtractRasters(string rasterPath1, string rasterPath2, string outputPath)
        {
            // 打开第一个栅格
            IRaster raster1 = LoadRaster(rasterPath1);
            IRaster raster2 = LoadRaster(rasterPath2);

            if (raster1 == null || raster2 == null)
            {
                MessageBox.Show("加载栅格失败！");
                return;
            }

            // 创建 Map Algebra 对象
            IMapAlgebraOp mapAlgebraOp = new RasterMapAlgebraOpClass();

            // 构造表达式，例如：第一个栅格减去第二个栅格
            string expression = "[Ras01] - [Ras02]";

            // 绑定栅格数据
            mapAlgebraOp.BindRaster((IGeoDataset)raster1, "Ras01");
            mapAlgebraOp.BindRaster((IGeoDataset)raster2, "Ras02");

            // 执行表达式
            IRaster result = (IRaster)mapAlgebraOp.Execute(expression);

            // 保存结果
            SaveRaster(result, outputPath);

            MessageBox.Show("栅格作差完成！");
        }

        private IRaster LoadRaster(string path)
        {
            IWorkspaceFactory wsFactory = new RasterWorkspaceFactory();
            string folder = System.IO.Path.GetDirectoryName(path);
            string file = System.IO.Path.GetFileName(path);

            IRasterWorkspace rasterWS = wsFactory.OpenFromFile(folder, 0) as IRasterWorkspace;
            IRasterDataset rasterDataset = rasterWS.OpenRasterDataset(file);

            IRaster raster = rasterDataset.CreateDefaultRaster();
            return raster;
        }

        public void SaveRaster(IRaster raster, string outputPath)
        {
            IWorkspaceFactory wsFactory = new RasterWorkspaceFactory();
            string folder = System.IO.Path.GetDirectoryName(outputPath);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(outputPath);

            IRasterWorkspace rasterWS = wsFactory.OpenFromFile(folder, 0) as IRasterWorkspace;
            ISaveAs saveAs = raster as ISaveAs;

            saveAs.SaveAs(fileName + ".tif", (IWorkspace)rasterWS, "TIFF");
        }
    }
}