using NPOI.POIFS.FileSystem;
using SDBS3000.Resources;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
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
                            b.NAME as Name,
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
                            a.isclear,
                            a.timestamp
                            from T_MeasureData  a
                            left join T_RotorSet b on a.RotorID = b.ID
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
    }
}
