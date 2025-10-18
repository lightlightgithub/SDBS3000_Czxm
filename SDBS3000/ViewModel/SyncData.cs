using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModel
{
    public class SyncBalData
    {
        public void SyncCal(T_Caldata T_Caldata)
        {
            if (T_Caldata != null)
            {

                MainViewModel.bal._runDB.set0.set0_val = new double[4] { T_Caldata.v0, T_Caldata.v1, T_Caldata.v2, T_Caldata.v3 };
                MainViewModel.bal._runDB.set0.cal_h = new double[4] { T_Caldata.h0, T_Caldata.h1, T_Caldata.h2, T_Caldata.h3 };
                MainViewModel.bal._runDB.set0.cal_ar = new double[4] { T_Caldata.ar0, T_Caldata.ar1, T_Caldata.ar2, T_Caldata.ar3 };
                MainViewModel.bal._runDB.set0.cal_ai = new double[4] { T_Caldata.ai0, T_Caldata.ai1, T_Caldata.ai2, T_Caldata.ai3 };
            }
            else
            {
                MainViewModel.bal._runDB.set0.set0_val = new double[4];
                MainViewModel.bal._runDB.set0.cal_h = new double[4];
                MainViewModel.bal._runDB.set0.cal_ar = new double[4];
                MainViewModel.bal._runDB.set0.cal_ai = new double[4];
            }
        }

        public void SyncClamp(T_Clampdata clamp)
        {
            if (clamp != null)
            {
                MainViewModel.bal._runDB.set_clamp.test_times = clamp.test_times;
                MainViewModel.bal._runDB.set_clamp.cps_val = new double[4] { clamp.cps_val_1, clamp.cps_val_2, clamp.cps_val_3, clamp.cps_val_4 };
            }
            else
            {
                MainViewModel.bal._runDB.set_clamp.test_times = 2;
                MainViewModel.bal._runDB.set_clamp.cps_val = new double[4] { 0, 0, 0, 0 };

            }
        }
    }
}
