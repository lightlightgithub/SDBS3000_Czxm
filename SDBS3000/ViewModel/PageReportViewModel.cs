using GalaSoft.MvvmLight.Command;
using SDBS3000.Services;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace SDBS3000.ViewModel
{
    public class PageReportViewModel:INotifyPropertyChanged
    {
        private static  PageReportService pageReportService = new PageReportService();
        private ObservableCollection<RecordList> result = new ObservableCollection<RecordList>();
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
        /// 检索条件结束时间
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
        /// 转子下拉框集合
        /// </summary>
        private ObservableCollection<RotorDic> rotorItems;
        public ObservableCollection<RotorDic> RotorItems
        {
            get { return rotorItems; }
            set
            {
                rotorItems = value;
                NotifyPropertyChanged("RotorItems");
            }
        }
        /// <summary>
        /// 选中的转子
        /// </summary>
        private RotorDic currentRotor;
        public RotorDic CurrentRotor
        {
            get { return currentRotor; }
            set
            {
                currentRotor = value;
                NotifyPropertyChanged("CurrentRotor");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 列表集合
        /// </summary>

        private ObservableCollection<RecordList> dataResult;
        public ObservableCollection<RecordList> DataResult
        {
            get { return dataResult; }
            set
            {
                dataResult = value;
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
            RotorItems = pageReportService.GetRotorDic();
            CurrentRotor = RotorItems.FirstOrDefault();
            result = pageReportService.GetData(BeginTime, EndTime, CurrentRotor?.RotorID, (int)ListSelectType.Select, false, PageNum, PageSize);
            GetPageData(result);

            LastPageCommand = new RelayCommand(() =>
            {
                if (PageNum-1 >0)
                {
                    PageNum -= 1;
                    DataResult = pageReportService.GetPageData(result, PageNum, PageSize);
                }
            });
            NextPageCommand = new RelayCommand(() =>
            {
                if (PageNum+1 <= PageCount)
                {
                    PageNum += 1;
                    DataResult = pageReportService.GetPageData(result, PageNum, PageSize);
                }
            });
            SearchCommand = new RelayCommand(() =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, CurrentRotor?.RotorID, (int)ListSelectType.Select, false, PageNum, PageSize);
                GetPageData(result);
            });
            TodayCommand = new RelayCommand(() =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, CurrentRotor?.RotorID, (int)ListSelectType.Today, false, PageNum, PageSize);
                GetPageData(result);
            });
            YesterdayCommand = new RelayCommand(() =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, CurrentRotor?.RotorID, (int)ListSelectType.Yesterday, false, PageNum, PageSize);
                GetPageData(result);
            });
            ThisMonthCommand = new RelayCommand(() =>
            {
                result = pageReportService.GetData(BeginTime, EndTime, CurrentRotor?.RotorID, (int)ListSelectType.Month, false, PageNum, PageSize);
                GetPageData(result);
            });
            ThisYearCommand= new RelayCommand(() =>
            {
               
                result = pageReportService.GetData(BeginTime, EndTime, CurrentRotor?.RotorID, (int)ListSelectType.Year, false, PageNum, PageSize);
                GetPageData(result);
            });
            ClearCommand = new RelayCommand(() =>
            {
                var isDelete = pageReportService.ClearData();
                if (isDelete)
                {
                    result = pageReportService.GetData(BeginTime, EndTime, CurrentRotor?.RotorID, (int)ListSelectType.Select, false, PageNum, PageSize);
                    GetPageData(result);
                }
            });
            ExportToExcelCommand = new RelayCommand(() =>
            {
                var rt = Export.ExportToExcel<T_MeasureData>(DataResult);
                NewMessageBox.Show(rt);
            });
            SelectAllCommand = new RelayCommand(() =>
            {
                SelectAll();
            });
            SelectNoneCommand = new RelayCommand(() =>
            {
                SelectNone();
            });
            ExportToCPKCommand = new RelayCommand(() =>
            {
                var list = result.Where(x => x.IsSelected).ToList();
                if (list.Count < 5)
                {
                    NewMessageBox.Show("查看CPK报告需要勾选的样本检测量至少5条");
                    return;
                }
                if (list.Count > 150)
                {
                    NewMessageBox.Show("查看CPK报告需要勾选的样本检测量至多150条");
                    return;
                }
                var data = pageReportService.ExportToCPK(new ObservableCollection<RecordList>(list));
                NewMessageBox.Show(data);
            });
        }

        public void SelectAll()
        {
            foreach (var item in result)
            {
                item.IsSelected = true; 
            }
            GetPageData(result);

        }
        public void SelectNone()
        {
            foreach (var item in result)
            {
                item.IsSelected = false;

            }
            GetPageData(result);

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
        /// 全选
        /// </summary>
        public ICommand SelectAllCommand { get; set; }
        /// <summary>
        /// 取消全选
        /// </summary>
        public ICommand SelectNoneCommand { get; set; }
        /// <summary>
        /// 导出cpk
        /// </summary>
        public ICommand ExportToCPKCommand { get; set; }
        /// <summary>
        /// 初次切换按钮时更新数据集、页数、总数量
        /// </summary>
        /// <param name="result"></param>
        public void GetPageData(ObservableCollection<RecordList> result)
        {
            PageNum = 1;
            TotalCount = result.Count;
            PageCount = TotalCount % PageSize == 0 ? TotalCount / PageSize : (TotalCount / PageSize + 1);
            DataResult = pageReportService.GetPageData(result, PageNum, PageSize);
        }

    }
}
