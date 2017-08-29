using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace WhatToWatch.Views.Cells
{
    public class CriteriaView : ContentView
    {
        public readonly int CriteriaId;
        public bool IsOn
        {
            get { return switchControl.IsToggled; }
        }

        private Switch switchControl;

        public CriteriaView(int id, bool isOn, string name)
        {
            CriteriaId = id;

            var label = new Label
            {
                Text = name,
                Style = (Style)Application.Current.Resources["centeredLabelStyle"],
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Fill
            };

            switchControl = new Switch
            {
                IsToggled = isOn,
                BackgroundColor = Color.LightGray
            };
            
            var stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                Children = { label, switchControl }
            };

            Content = new Frame { Content = stack, CornerRadius = 5, OutlineColor = Color.LightGray };
        } 
    }
}
