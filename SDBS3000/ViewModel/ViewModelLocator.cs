/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SDBS3000"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace SDBS3000.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MeasureViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<UserManagementViewModel>();
            SimpleIoc.Default.Register<PosViewModel>();
            SimpleIoc.Default.Register<CalViewModel>();
            SimpleIoc.Default.Register<SetViewModel>();
            SimpleIoc.Default.Register<RtSetViewModel>();
            SimpleIoc.Default.Register<ClampViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public MeasureViewModel WinMeasure
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MeasureViewModel>();
            }
        }

        public LoginViewModel WinLogin
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public UserManagementViewModel UserManagement
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UserManagementViewModel>();
            }
        }

        public PosViewModel Pos
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PosViewModel>();
            }
        }

        public CalViewModel Cal
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CalViewModel>();
            }
        }

        public ClampViewModel Clamp
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ClampViewModel>();
            }
        }

        public SetViewModel Set
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SetViewModel>();
            }
        }

        public RtSetViewModel RtSet
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RtSetViewModel>();
            }
        }



        public static void Cleanup()
        {

        }

        public static void Cleanup<T>() where T : class
        {
            SimpleIoc.Default.Unregister<T>();
            SimpleIoc.Default.Register<T>();
        }
    }
}