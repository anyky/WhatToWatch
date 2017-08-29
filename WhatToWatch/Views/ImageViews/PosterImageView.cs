using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.MyEventArgs;
using Xamarin.Forms;

namespace WhatToWatch.Views.ImageViews
{
    public class PosterImageView : BaseImageView
    {
        public PosterImageView(object context, double aW, ImageSource src) : base(context, aW, src)
        {
            aspect = 1.5;
        }
    }
}
