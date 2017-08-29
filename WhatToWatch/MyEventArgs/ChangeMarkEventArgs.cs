using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatToWatch.MyEventArgs
{
    public class ChangeMarkEventArgs : EventArgs
    {
        public int markId { get; set; }
        public int markVal { get; set; }

        public ChangeMarkEventArgs(int id, int val)
        {
            markId = id;
            markVal = val;
        }
    }
}
