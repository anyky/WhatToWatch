using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.Model;
using WhatToWatch.Pages;
using WhatToWatch.Service;
using WhatToWatch.Views.Cells;
using Xamarin.Forms;

namespace WhatToWatch.Views.ListViews
{
    public class CommentListView : ListView
    {
        private List<Commentary> comments;

        public CommentListView()
        {
            UpdateComments(null);

            this.ItemSelected +=
                (object sender, SelectedItemChangedEventArgs e) =>
                {
                    if (e.SelectedItem == null)
                        return;
                    ((ListView)sender).SelectedItem = null;
                };
        }

        public void UpdateComments(List<Commentary> com)
        {
            this.HasUnevenRows = true;
            this.comments = com;
            this.ItemsSource = null;
            this.ItemsSource = comments;
            if (comments != null)
            {
                this.ItemTemplate = new DataTemplate(typeof(CommentCell));
                this.ItemTemplate.SetBinding(CommentCell.CommentIdProperty, "CommentId");
                this.ItemTemplate.SetBinding(CommentCell.SenderLoginProperty, "SenderLogin");
                this.ItemTemplate.SetBinding(CommentCell.SendTimeProperty, "SendTime");
                this.ItemTemplate.SetBinding(CommentCell.TextProperty, "Text");
                this.ItemTemplate.SetBinding(CommentCell.HasComplaintProperty, "HasComplaint");
            }
        }
    }
}
