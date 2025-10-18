using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.BalTest
{
    public class receivedDataEvent : EventArgs
    {
        private string _recData;
        private int _no;

        public string RecData => this._recData;

        public int FrameNo => this._no;

        public receivedDataEvent(string recDataStr, int no)
        {
            this._recData = recDataStr;
            this._no = no;
        }
    }
}
