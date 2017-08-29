using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace WhatToWatch.Views.ImageViews
{
    public class ComplaintImageView : BaseImageView
    {
        public enum ComplaintType { None, HasComplaint, HasNotComplaint, Delete }

        private ComplaintType type;
        public ComplaintType Type
        {
            get { return type; }
            set
            {
                type = value;
                switch (type)
                {
                    case ComplaintType.HasComplaint:
                        image.BackgroundColor = Color.White;
                        image.Source = (ImageSource)Application.Current.Resources["fingerOnImageSource"];
                        break;
                    case ComplaintType.HasNotComplaint:
                        image.Source = (ImageSource)Application.Current.Resources["fingerOffImageSource"];
                        break;
                    case ComplaintType.Delete:
                        image.Source = (ImageSource)Application.Current.Resources["crossImageSource"];
                        break;
                    default:
                        image.Source = null;
                        break;
                }
            }
        }
        
        public ComplaintImageView(object context, ComplaintType type, EventHandler<EventArgs> e, double aW = 0.1): base(context, aW)
        {
            image.BackgroundColor = Color.Orange;
            Type = type;
            aspect = 1;
            GestureRecognizers.Add(new TapGestureRecognizer((View view) => e?.Invoke(this, new EventArgs())));
        }
    }
}
