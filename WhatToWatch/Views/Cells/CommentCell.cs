using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.Service;
using WhatToWatch.Views.ImageViews;
using Xamarin.Forms;

namespace WhatToWatch.Views.Cells
{
    public class CommentCell : ViewCell
    {
        public static event EventHandler<int> complaintEvent;
        public static event EventHandler<int> deleteEvent;
        public static void ResetEvents() { complaintEvent = deleteEvent = null; }
        private event EventHandler<EventArgs> compaintPressedEvent;

        private StackLayout textStack;

        private Label senderLoginLabel;
        private Label sendTimeLabel;
        private Label textLabel;
        private ComplaintImageView image;

        public static readonly BindableProperty CommentIdProperty =
            BindableProperty.Create("CommentId", typeof(int), typeof(CommentCell), 0);
        public static readonly BindableProperty SenderLoginProperty =
            BindableProperty.Create("SenderLogin", typeof(string), typeof(CommentCell));
        public static readonly BindableProperty SendTimeProperty =
            BindableProperty.Create("SendTime", typeof(string), typeof(CommentCell));
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(CommentCell));
        public static readonly BindableProperty HasComplaintProperty =
            BindableProperty.Create("HasComplaint", typeof(bool), typeof(CommentCell), false);

        public int CommentId
        {
            get { return (int)GetValue(CommentIdProperty); }
            set { SetValue(CommentIdProperty, value); }
        }
        public string SenderLogin
        {
            get { return (string)GetValue(SenderLoginProperty); }
            set { SetValue(SenderLoginProperty, value); }
        }
        public string SendTime
        {
            get { return (string)GetValue(SendTimeProperty); }
            set { SetValue(SendTimeProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public bool HasComplaint
        {
            get { return (bool)GetValue(HasComplaintProperty); }
            set { SetValue(HasComplaintProperty, value); }
        }

        public CommentCell()
        {
            var headerStack = new StackLayout();
            headerStack.Orientation = StackOrientation.Horizontal;
            headerStack.HorizontalOptions = LayoutOptions.FillAndExpand;

            senderLoginLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold,
            };
            sendTimeLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.End,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black
            };
            headerStack.Children.Add(senderLoginLabel);
            headerStack.Children.Add(sendTimeLabel);


            textStack = new StackLayout();
            textStack.Orientation = StackOrientation.Horizontal;
            textStack.HorizontalOptions = LayoutOptions.FillAndExpand;

            textLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.Black
            };

            textStack.Children.Add(textLabel);

            image = null;
            
            View = new Frame { Content = new StackLayout { Children = { headerStack, textStack } }, CornerRadius = 5, OutlineColor = Color.LightGray };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                if (DataService.Instance().LoggedIn)
                {
                    if (image == null)
                    {
                        if (DataService.Instance().Admin)
                        {
                            compaintPressedEvent += (object sender, EventArgs e) => deleteEvent?.Invoke(this, CommentId);
                            image = new ComplaintImageView(textStack, ComplaintImageView.ComplaintType.Delete, compaintPressedEvent);
                        }
                        else if (SenderLogin != DataService.Instance().Username)
                        {
                            if (HasComplaint)
                                image = new ComplaintImageView(textStack, ComplaintImageView.ComplaintType.HasComplaint, compaintPressedEvent);
                            else
                            {
                                compaintPressedEvent += (object sender, EventArgs e) =>
                                {
                                    complaintEvent?.Invoke(this, CommentId);
                                    HasComplaint = !HasComplaint;
                                    image.Type = ComplaintImageView.ComplaintType.HasComplaint;
                                };
                                image = new ComplaintImageView(textStack, ComplaintImageView.ComplaintType.HasNotComplaint, compaintPressedEvent);
                            }
                        }

                        if (image != null)
                            textStack.Children.Add(image);
                    }
                    else if (image.Type == ComplaintImageView.ComplaintType.HasNotComplaint && HasComplaint)
                    {
                        compaintPressedEvent = null;
                        image.Type = ComplaintImageView.ComplaintType.HasComplaint;
                    }
                }
                
                senderLoginLabel.Text = SenderLogin;
                sendTimeLabel.Text = SendTime;
                textLabel.Text = Text;
            }
        }
    }
}
