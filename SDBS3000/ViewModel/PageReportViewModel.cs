using SDBS3000.Services;
using SDBS3000.Views;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SDBS3000.ViewModel
{
    public class PageReportViewModel:INotifyPropertyChanged
    {
        private static  PageReportService pageReportService = new PageReportService();
        private ObservableCollection<T_MeasureData> result = new ObservableCollection<T_MeasureData>();
        #region 属性
        /// <summary>
        /// 总记录数
        /// </summary>
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set { 
                totalCount = value;
                NotifyPropertyChanged("TotalCount");
            }
        }
        /// <summary>
        /// 总页数
        /// </summary>
        private int pageCount = 0;
        public int PageCount
        {
            get { return pageCount; }
            set
            {
                pageCount = value;
                NotifyPropertyChanged("PageCount");
            }
        }
        /// <summary>
        /// 每页数量
        /// </summary>
        private int pageSize  = 15;
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                pageSize = value;
                NotifyPropertyChanged("PageSize");
            }
        }
        /// <summary>
        /// 页码
        /// </summary>
        private int pageNum=1;
   
        public int PageNum
        {
            get { return pageNum; }
            set
            {
                pageNum = value;
                NotifyPropertyChanged("PageNum");
            }

        }
        /// <summary>
        /// 检索条件开始时间
        /// </summary>
        private DateTime? beginTime = DateTime.Now;
        public DateTime? BeginTime
        {
            get { return beginTime; }
            set
            {
                beginTime = value;
                NotifyPropertyChanged("BeginTime");
            }
        }
        /// <summary>
        /// 检索条件j结束时间
        /// </summary>
        private DateTime? endTime = DateTime.Now.AddDays(1);
        public DateTime? EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                NotifyPropertyChanged("EndTime");
            }
        }
        /// <summary>
        /// 当日
        /// </summary>
        private bool today;
        public bool Today
        {
            get { return today; }
            set
            {
                today = value;
                NotifyPropertyChanged("Today");
            }
        }

        /// <summary>
        /// 昨日
        /// </summary>
        private bool lastday;
        public bool LastDay
        {
            get { return lastday; }
            set
            {
                lastday = value;
                NotifyPropertyChanged("LastDay");
            }
        }
        /// <summary>
        /// 本月
        /// </summary>
        private bool thisMonth;
        public bool ThisMonth
        {
            get { return thisMonth; }
            set
            {
                thisMonth = value;
                NotifyPropertyChanged("ThisMonth");
            }
        }
        /// <summary>
        /// 当年
        /// </summary>
        private bool thisYear;
        public bool ThisYear
        {
            get { return thisYear; }
            set
            {
                thisYear = value;
                NotifyPropertyChanged("ThisYear");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 列表集合
        /// </summary>

        private ObservableCollection<T_MeasureData> dtaResult;
        public ObservableCollection<T_MeasureData> DataResult
        {
            get { return dtaResult; }
            set
            {
                dtaResult = value;
                NotifyPropertyChanged("DataResult");
            }
        }
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public PageReportViewModel()
        {
            result = pageReportService.GetData(BeginTime, EndTime, (int)ListSelectType.Select, false, PageNum, PageSize);
            GetPageData(result);

            LastPageCommand = new RelayCommand((age) =>
            {
                if (PageNum-1 >0)
                {
                    PageNum -= 1;
                    DataResult = pageReportService.GetPageData(result, PageNum, PageSize);
                }
            });
            NextPageCommand = new RelayCommand((age) =>
            {
                if (PageNum+1 <= PageCount)
                {
                    PageNum += 1;
                    DataResult = pageReportService.GetPageData(result, PageNum, PageSize);
                }
            });
            SearchCommand = new RelayCommand((age) =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, (int)ListSelectType.Select, false, PageNum, PageSize);
                GetPageData(result);
            });
            TodayCommand = new RelayCommand((age) =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, (int)ListSelectType.Today, false, PageNum, PageSize);
                GetPageData(result);
            });
            YesterdayCommand = new RelayCommand((age) =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, (int)ListSelectType.Yesterday, false, PageNum, PageSize);
                GetPageData(result);
            });
            ThisMonthCommand = new RelayCommand((age) =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, (int)ListSelectType.Month, false, PageNum, PageSize);
                GetPageData(result);
            });
            ThisYearCommand= new RelayCommand((age) =>
            {
               
                result = pageReportService.GetData(BeginTime, EndTime, (int)ListSelectType.Year, false, PageNum, PageSize);
                GetPageData(result);
            });
            ClearCommand = new RelayCommand((age) =>
            {
                var isDelete = pageReportService.ClearData();
                if (isDelete)
                {
                    result = pageReportService.GetData(BeginTime, EndTime, (int)ListSelectType.Select, false, PageNum, PageSize);
                    GetPageData(result);
                }
            });
            ExportToExcelCommand = new RelayCommand((age) =>
            {
                var rt = Export.ExportToExcel<T_MeasureData>(DataResult);
                NewMessageBox.Show(rt);
            });
        }
        /// <summary>
        /// 下一页
        /// </summary>
        public ICommand LastPageCommand { get; set; }
        /// <summary>
        /// 上一页
        /// </summary>
        public ICommand NextPageCommand { get; set; }
        /// <summary>
        /// 查询
        /// </summary>
        public ICommand SearchCommand { get; set; }
        /// <summary>
        /// 查询当日
        /// </summary>
        public ICommand TodayCommand { get; set; }
        /// <summary>
        /// 查询昨日
        /// </summary>
        public ICommand YesterdayCommand { get; set;}
        /// <summary>
        /// 查询当月
        /// </summary>
        public ICommand ThisMonthCommand { get; set; }
        /// <summary>
        /// 查询当年
        /// </summary>
        public ICommand ThisYearCommand { get; set; }
        /// <summary>
        /// 清楚数据
        /// </summary>
        public ICommand ClearCommand { get; set; }
        /// <summary>
        /// 导出到excel
        /// </summary>
        public ICommand ExportToExcelCommand { get; set; }
        /// <summary>
        /// 初次切换按钮时更新数据集、页数、总数量
        /// </summary>
        /// <param name="result"></param>
        public void GetPageData(ObservableCollection<T_MeasureData> result)
        {
            PageNum = 1;
            TotalCount = result.Count;
            PageCount = TotalCount % PageSize == 0 ? TotalCount / PageSize : (TotalCount / PageSize + 1);
            DataResult = pageReportService.GetPageData(result, PageNum, PageSize);
        }

    }
    public class UserInfoModel
    {
        public string UserID { get; set; }
        public string NAME { get; set; }
    }
}
