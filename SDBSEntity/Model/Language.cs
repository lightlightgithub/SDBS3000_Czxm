using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity
{
    public class Language
    {
        private int currentLanguage;
        public int CurrentLanguage
        {

            get
            {
                return currentLanguage;
            }
            set
            {
                currentLanguage = value;
                OnLanChange();
            }
        }

        public event EventHandler OnLanChangeEvent;

        public void OnLanChange()
        {
            OnLanChangeEvent?.Invoke(this, null);
        }
    }
}
