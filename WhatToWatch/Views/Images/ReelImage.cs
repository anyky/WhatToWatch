using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.MyEventArgs;
using Xamarin.Forms;

namespace WhatToWatch.Views.Images
{
    public class ReelImage : Image
    {
        private bool ison;
        public bool isOn
        {
            get { return ison; }
            set
            {
                ison = value;
                if (ison)
                    Source = (ImageSource)Application.Current.Resources["reelFilledImageSource"];
                else
                    Source = (ImageSource)Application.Current.Resources["reelEmptyImageSource"];
            }
        }
        public int index { get; set; }

        public ReelImage(bool on, int i, EventHandler<int> e)
        {
            isOn = on;
            index = i;

            Aspect = Aspect.AspectFill;
            GestureRecognizers.Add(new TapGestureRecognizer((View view) => e.Invoke(this, index + 1)));
        }
    }
}
