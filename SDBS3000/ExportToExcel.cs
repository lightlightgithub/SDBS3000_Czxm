using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SDBS3000.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SDBS3000
{
    public class Export
    {
        /* 1.切换中英文的标志怎么传递过来？
         * 2.键值对如何添加（添加的键和值是什么）
         * 3.如何根据标志切换中英文
         */
        public static string ExportToExcel<T>(IEnumerable<T> data)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                Type t = data.GetType().GenericTypeArguments[0];

                DescriptionAttribute classname = (DescriptionAttribute)t.GetCustomAttribute(typeof(DescriptionAttribute));

                //创建一个工作簿
                IWorkbook workbook = new HSSFWorkbook();

                //创建一个 sheet 表
                string sheetname = "sheet1";
                if (Attribute.IsDefined(t, (typeof(DescriptionAttribute))))
                    sheetname =((DescriptionAttribute)t.GetCustomAttribute(typeof(DescriptionAttribute))).Description;
                
                ISheet sheet = workbook.CreateSheet(sheetname);

                //创建一行
                IRow rowH = sheet.CreateRow(0);

                //创建一个单元格
                ICell cell = null;

                //创建单元格样式
                ICellStyle cellStyle = workbook.CreateCellStyle();
                ICellStyle cellStyle1 = workbook.CreateCellStyle();

                //创建格式
                IDataFormat dataFormat = workbook.CreateDataFormat();

                //设置为文本格式，也可以为 text，即 dataFormat.GetFormat("text");
                cellStyle.DataFormat = dataFormat.GetFormat("@");
                cellStyle1.DataFormat = dataFormat.GetFormat("0.00_");

                PropertyInfo[] propertys = t.GetProperties();

                for (int i = 0; i < propertys.Count(); i++)
                {
                    if (Attribute.IsDefined(propertys[i], (typeof(DescriptionAttribute))))
                    {
                        try
                        {
                            rowH.CreateCell(i).SetCellValue(LanguageManager.Instance._resourceManager.GetString(Convert.ToString(((DescriptionAttribute)propertys[i].GetCustomAttribute(typeof(DescriptionAttribute))).Description)));
                            rowH.Cells.LastOrDefault().CellStyle = cellStyle;
                        }
                        catch (Exception e)
                        {
                            SDBS3000.Log.Log.Write(SDBS3000.Log.Log.LogType.ERROR, e.ToString());
                        }
                    }
                }

                var ge = data.GetEnumerator();
                for (int i = 0; i < data.Count(); i++)
                {
                    try
                    {
                        IRow row = sheet.CreateRow(i + 1);
                        ge.MoveNext();
                        for (int j = 0; j < propertys.Count(); j++)
                        {
                            var curobj = ge.Current;

                            if (propertys[j].GetValue(curobj) == null)
                            {
                                continue;
                            }
                            cell = row.CreateCell(j);
                            if (propertys[j].PropertyType.Name == "Double" || propertys[j].PropertyType.Name == "Int32" || propertys[j].PropertyType.Name == "Nullable`1")
                            {
                                cell.SetCellValue(Double.Parse(propertys[j].GetValue(curobj).ToString()));
                                cell.CellStyle = cellStyle1;
                            }
                            else if (propertys[j].PropertyType.Name == "String" || propertys[j].PropertyType.Name == "DateTime")
                            {
                                cell.SetCellValue(propertys[j].GetValue(curobj).ToString());
                                cell.CellStyle = cellStyle;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        SDBS3000.Log.Log.Write(SDBS3000.Log.Log.LogType.ERROR, e.ToString());
                    }
                }

                //设置导出文件路径
                StringBuilder sb = new StringBuilder();
                sb.Append("D:");
                sb.Append("\\Export\\");
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd"));
                sb.Append("\\");
                string path = sb.ToString();
                //HttpContext.Current.Server.MapPath("Export/");
                if (!Directory.Exists(path))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    directoryInfo.Create();
                }

                string savePath = path + "balancedata" + DateTime.Now.ToString("HHmmss") + ".xls";

                FileStream file = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write);

                MemoryStream ms = new MemoryStream();

                workbook.Write(ms);

                //转换为字节数组
                byte[] bytes = ms.ToArray();

                file.Write(bytes, 0, bytes.Length);
                file.Flush();

                ms.Close();
                ms.Dispose();
                file.Close();
                file.Dispose();
                workbook.Close();
                sheet = null;
                workbook = null;
                return "导出路径:" + savePath;
            }
            catch (Exception ex)
            {
                return "导出异常：" + ex.StackTrace;
            }
        }

    }
}
