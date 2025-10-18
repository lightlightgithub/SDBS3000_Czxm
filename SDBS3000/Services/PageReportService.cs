using NPOI.POIFS.FileSystem;
using SDBS3000.Resources;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SDBS3000.Services
{
    public interface IPageReportService
    {
       // ObservableCollection<T_MeasureData> GetMeasureData();
        ObservableCollection<T_MeasureData> GetData(DateTime? BeginTime, DateTime? EndTime, int SelectType, bool IsPage, int PageNum, int PageSize);
        ObservableCollection<T_MeasureData> GetPageData(ObservableCollection<T_MeasureData> result, int PageNum, int PageSize);
        bool ClearData();
    }
    public class PageReportService : IPageReportService
    {
        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public ObservableCollection<T_MeasureData> GetPageData(ObservableCollection<T_MeasureData> result,int PageNum, int PageSize)
        {
            var nextPageData = result.OrderByDescending(p => p.ID).Skip((PageNum - 1) * PageSize).Take(PageSize);
            var data = new ObservableCollection<T_MeasureData>(nextPageData);
            return data;
        }
        /// <summary>
        /// 记录列表查询
        /// </summary>
        /// <param name="BeginTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="SelectType">查询类型</param>
        /// <param name="IsPage">是否分页</param>
        /// <param name="PageNum">页码</param>
        /// <param name="PageSize">每页数量</param>
        /// <returns></returns>
        public ObservableCollection<T_MeasureData> GetData(DateTime? BeginTime, DateTime? EndTime,int SelectType,bool IsPage, int PageNum, int PageSize)
        {
            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                var data = Entity.T_MeasureData.Where(p => p.isclear == 0);
                var today = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                var yeasday = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00"));
               
                switch (SelectType)
                {
                    case (int)ListSelectType.Select:
                        if (BeginTime != null && EndTime != null && BeginTime <= EndTime)
                        {
                            BeginTime = Convert.ToDateTime(Convert.ToDateTime(BeginTime).ToString("yyyy-MM-dd 00:00:00"));
                            EndTime = Convert.ToDateTime(Convert.ToDateTime(EndTime).ToString("yyyy-MM-dd 00:00:00"));
                        }
                        data = data.Where(x => x.MODIFYTIME >= BeginTime && x.MODIFYTIME <= EndTime);
                        break;
                    case (int)ListSelectType.Today:
                        data = data.Where(x => x.MODIFYTIME == today);
                        break;
                    case (int)ListSelectType.Yesterday:
                        data = data.Where(x => x.MODIFYTIME == yeasday);
                        break;
                    case (int)ListSelectType.Month:
                        data = data.Where(x => x.MODIFYTIME.Year ==DateTime.Now.Year && x.MODIFYTIME.Month==DateTime.Now.Month);
                        break;
                    case (int)ListSelectType.Year:
                        data = data.Where(x => x.MODIFYTIME.Year == DateTime.Now.Year);
                        break;
                    default:
                        break;
                }
                var result = new ObservableCollection<T_MeasureData>(data);
                if (IsPage)
                    return GetPageData(result, PageNum, PageSize);
                return result;
               
            }
        }
        /// <summary>
        /// 软删除列表数据
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
    }
}
