using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.XSSF.UserModel;
using SDBS3000.Resources;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SDBS3000.Services
{
    public class PageReportService
    {
        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public ObservableCollection<RecordList> GetPageData(ObservableCollection<RecordList> result, int PageNum, int PageSize)
        {
            var nextPageData = result.Skip((PageNum - 1) * PageSize).Take(PageSize);
            var data = new ObservableCollection<RecordList>(nextPageData);
            return data;
        }
        /// <summary>
        /// 记录列表查询
        /// </summary>
        /// <param name="BeginTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="RotorID">转子ID</param>
        /// <param name="SelectType">查询类型</param>
        /// <param name="IsPage">是否分页</param>
        /// <param name="PageNum">页码</param>
        /// <param name="PageSize">每页数量</param>
        /// <returns></returns>
        public ObservableCollection<RecordList> GetData(DateTime? BeginTime, DateTime? EndTime, int? RotorID, int SelectType, bool IsPage, int PageNum, int PageSize)
        {
            var result = new ObservableCollection<RecordList>();
            string sql = @"select 
                             CAST(ROW_NUMBER() over(order by a.MODIFYTIME desc) as int) as RowID,
                            a.ID,
                            a.RotorID,
                            a.NAME,
                            a.UserID,
                            a.rpm,
                            a.singleL,
                            a.singleR,
                            a.fl,
                            a.ql,
                            a.fr,
                            a.qr,
                            a.fm,
                            a.qm,
                            a.Ispass,
                            a.Zcfs,
                            a.Jsms,
                            a.Clms,
                            a.Ccdw,
                            a.Yxldw,
                            a.Pmyyxl,
                            a.Pmeyxl,
                            a.Jyxl,
                            a.A,
                            a.B,
                            a.C,
                            a.R1,
                            a.R2,
                            a.Duringtime,
                            a.Speed,
                            a.Jjjz,
                            a.Pmyjjz,
                            a.Pmejjz,
                            a.MODIFYTIME,
                            CONVERT(VARCHAR, a.MODIFYTIME, 120) as OperateTime,
                            a.isclear,
                            a.timestamp
                            from T_MeasureData  a
                            where a.isclear = 0 ";
            string paramSql = string.Empty;
            switch (SelectType)
            {
                case (int)ListSelectType.Select:
                    if (BeginTime != null && EndTime != null && BeginTime <= EndTime)
                    {
                        paramSql = string.Concat(paramSql, " and CONVERT(date,a.MODIFYTIME) between @BeginTime and @EndTime");
                    }
                    if (RotorID > 0 && RotorID != null)
                    {
                        paramSql = string.Concat(paramSql, " and  a.RotorID = @RotorID");
                    }
                    break;
                case (int)ListSelectType.Today:
                    paramSql = string.Concat(paramSql, " and CONVERT(DATE, a.MODIFYTIME) = CONVERT(DATE, GETDATE())");
                    break;
                case (int)ListSelectType.Yesterday:
                    paramSql = string.Concat(paramSql, " and CONVERT(DATE, a.MODIFYTIME) = CONVERT(DATE, GETDATE()-1)");
                    break;
                case (int)ListSelectType.Month:
                    paramSql = string.Concat(paramSql, "  and MONTH(a.MODIFYTIME) = MONTH(GETDATE())");
                    break;
                case (int)ListSelectType.Year:
                    paramSql = string.Concat(paramSql, "  and YEAR(a.MODIFYTIME) = YEAR(GETDATE())");
                    break;
                default:
                    break;
            }
            sql = string.Concat(sql, paramSql);
            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                var data = Entity.Database.SqlQuery<RecordList>(sql, new[]
                {
                    new SqlParameter("BeginTime",Convert.ToDateTime(BeginTime).ToString("yyyy-MM-dd 00:00:00")),
                    new SqlParameter("EndTime",Convert.ToDateTime(EndTime).ToString("yyyy-MM-dd 00:00:00")),
                    new SqlParameter("RotorID",RotorID)
                });
                result = new ObservableCollection<RecordList>(data);
            }
            if (IsPage)
                return GetPageData(result, PageNum, PageSize);
            return result;
        }
        /// <summary>
        /// (软删除)清除列表数据
        /// </summary>
        /// <returns></returns>
        public bool ClearData()
        {
            int result = 0;
            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                result = Entity.Database.ExecuteSqlCommand("update T_MeasureData set isclear = 1");
            }
            return result > 0;
        }
        /// <summary>
        /// 查询转子字典
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<RotorDic> GetRotorDic()
        {
            var rotorDic = new ObservableCollection<RotorDic>();
            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                var data = Entity.T_RotorSet.Select(x => new RotorDic
                {
                    RotorID = x.ID,
                    RotorName = x.NAME
                });
                rotorDic = new ObservableCollection<RotorDic>(data);
            }
            return rotorDic;
        }
        /// <summary>
        /// 导出CPK报告
        /// </summary>
        public string ExportToCPK(ObservableCollection<RecordList> list)
        {
            string filePath = string.Empty;
            XSSFWorkbook workbook = null;//Excel实例
            XSSFSheet sheet = null;//表实例
            string data = string.Empty;
            List<double> measureValues = new List<double>();
            try
            {
                var model = list.FirstOrDefault();
                //模板路径   
                string excelTempPath = System.Environment.CurrentDirectory + @"\Resources\CPK.xlsx";
                string targetDir = $"D:\\Export\\CPK\\{DateTime.Now.ToString("yyyy-MM-dd")}\\";
                if (!Directory.Exists(targetDir))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(targetDir);
                    directoryInfo.Create();
                }

                string targetPath = string.Concat(targetDir, $"{DateTime.Now.ToString("HHmmss")} .xls");

                //读取Excel模板
                using (FileStream fs = new FileStream(excelTempPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    workbook = new XSSFWorkbook(fs);
                }
                if (Convert.ToInt32(model.Clms) == (int)MeasureMode.TwoPlaneDynamicBalance) //动平衡
                {
                    measureValues = list.Select(x => x.fl).ToList();  //左量值
                    sheet = GetSheet(workbook, 1, measureValues, Convert.ToDouble(model.Pmyyxl));
                    measureValues = list.Select(x => x.fr).ToList();  //右量值
                    sheet = GetSheet(workbook, 2, measureValues, Convert.ToDouble(model.Pmeyxl));
                    workbook.RemoveSheetAt(0);
                }
                else if (Convert.ToInt32(model.Clms) == (int)MeasureMode.StaticBalance) //静平衡
                {
                    measureValues = list.Select(x => x.fm).ToList();  //静量值
                    sheet = GetSheet(workbook, 0, measureValues, Convert.ToDouble(model.Jyxl));
                    workbook.RemoveSheetAt(1);
                    workbook.RemoveSheetAt(2);
                }
                else if (Convert.ToInt32(model.Clms) == (int)MeasureMode.DynamicStaticBalance)  //动静平衡
                {
                    measureValues = list.Select(x => x.fm).ToList();  //静量值
                    sheet = GetSheet(workbook, 0, measureValues, Convert.ToDouble(model.Jyxl));

                    measureValues = list.Select(x => x.fl).ToList();  //左量值
                    sheet = GetSheet(workbook, 1, measureValues, Convert.ToDouble(model.Pmyyxl));

                    measureValues = list.Select(x => x.fr).ToList();  //右量值
                    sheet = GetSheet(workbook, 2, measureValues, Convert.ToDouble(model.Pmeyxl));
                }
                using (FileStream fs = File.Create(targetPath))
                {
                    workbook.SetForceFormulaRecalculation(true);
                    workbook.Write(fs);
                    fs.Close();
                }
                return targetPath;
            }
            catch (Exception ex)
            {
                data = ex.Message;
            }
            return data;
        }
        /// <summary>
        /// 获取sheet
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="measureValue"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public XSSFSheet GetSheet(XSSFWorkbook workbook, int sheetIndex, List<double> measureValue, double allowValue)
        {
            XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(sheetIndex);

            int rowBegin = 9;//当前行10,索引是0开始
            int colBegin = 1;
            var rowIndex = 0;
            var colIndex = 0;
            for (int i = 0; i < measureValue.Count; i++)
            {
                if (rowIndex >= 5)
                {
                    if (i % 50 != 0) //50条记录为一批测量值
                        colIndex++;
                    else  //超过50条记录，列从头开始
                    {
                        rowBegin += 6;  //起始行重置，共间隔6行
                        colBegin = 1;   //起始列重置为1
                        colIndex = 0;   //列索引重置为0
                    }
                    rowIndex = 0;
                }
                var cell = sheet.GetRow(rowBegin + rowIndex).GetCell(colBegin + colIndex); 
                cell.SetCellValue(measureValue[i]); //测量值 
                rowIndex++;
            }
            sheet.GetRow(7).GetCell(9).SetCellValue(Convert.ToDouble(allowValue)); //允许量
            return sheet;
        }
    }
}
