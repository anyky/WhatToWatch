using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.Model;
using WhatToWatch.MyEventArgs;
using WhatToWatch.Views.Images;
using Xamarin.Forms;

namespace WhatToWatch.Views
{
    public class RatingView : ContentView
    {
        private Label nameLabel;
        private List<ReelImage> reelImages;
        private Label averageLabel;
        private Mark mark;

        public RatingView(Mark m, EventHandler<ChangeMarkEventArgs> reelTappedEvent)
        {
            mark = m;
            reelImages = new List<ReelImage>();
            for (int i = 0; i < 10; i++)
            {
                reelImages.Add(new ReelImage(false, i, (object sender, int val) => reelTappedEvent.Invoke(this, new ChangeMarkEventArgs(this.mark.MarkId, val))));
            }
            nameLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.Black };
            averageLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.Black };
            

            var textLayout = new StackLayout();
            textLayout.Orientation = StackOrientation.Horizontal;
            textLayout.VerticalOptions = LayoutOptions.Fill;
            textLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            textLayout.Children.Add(nameLabel);
            textLayout.Children.Add(averageLabel);

            var starsLayout = new RelativeLayout();
            starsLayout.Children.Add(reelImages[0],
                Constraint.Constant(0),
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => parent.Width / 10),
                Constraint.RelativeToParent((parent) => parent.Width / 10));
            for (int i = 1; i < 10; i++)
            {
                starsLayout.Children.Add(reelImages[i],
                Constraint.RelativeToView(reelImages[i - 1], (parent, view) => view.X + view.Width),
                Constraint.RelativeToView(reelImages[i - 1], (parent, view) => view.Y),
                Constraint.RelativeToParent((parent) => parent.Width / 10),
                Constraint.RelativeToParent((parent) => parent.Width / 10));
            }

            var stack = new StackLayout { Children = { textLayout, starsLayout }, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Fill };

            var frame = new Frame
            {
                Content = stack,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };


            frame.CornerRadius = 10;
            frame.OutlineColor = Color.Black;

            Content = frame;

            UpdateMark();
        }

        public void UpdateMark()
        {
            if (mark.Value > 0)
                Content.BackgroundColor = Color.Orange;
            nameLabel.Text = mark.Name;
            for (int i = 0; i < 10; i++)
            {
                reelImages[i].isOn = i < mark.Value;
            }
            averageLabel.Text = mark.Average.ToString();
        }

        public void UpdateMark(Mark m)
        {
            mark = m;
            UpdateMark();
        }
    }
}
