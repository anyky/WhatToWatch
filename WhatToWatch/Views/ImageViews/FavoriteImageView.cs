using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace WhatToWatch.Views.ImageViews
{
    public class FavoriteImageView : BaseImageView
    {
        private bool ison;
        public bool isOn
        {
            get { return ison; }
            set
            {
                ison = value;
                if (ison)
                    image.Source = (ImageSource)Application.Current.Resources["heartOnImageSource"];
                else
                    image.Source = (ImageSource)Application.Current.Resources["heartOffImageSource"];
            }
        }

        public FavoriteImageView(object context, bool on, EventHandler<EventArgs> e, double aW = 0.1): base(context, aW)
        {
            aspect = 1;
            isOn = on;
            image.BackgroundColor = Color.Orange;
            GestureRecognizers.Add(new TapGestureRecognizer((View view) => e?.Invoke(this, new EventArgs())));
        }
    }
}
