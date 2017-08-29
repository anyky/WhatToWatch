using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace WhatToWatch.Views
{
    public class ExpandableEditor : Editor
    {
        public ExpandableEditor()
        {
            TextChanged += (object sender, TextChangedEventArgs e) => InvalidateMeasure();
        }
    }
}
