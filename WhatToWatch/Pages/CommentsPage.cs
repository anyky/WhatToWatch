using System;
using WhatToWatch.Handlers;
using WhatToWatch.Service;
using WhatToWatch.Views;
using WhatToWatch.Views.Cells;
using WhatToWatch.Views.ListViews;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class CommentsPage : ContentPage
    {
        private Label commentStatus;
        private CommentListView commentsList;
        private ExpandableEditor commentEditor;
        private Button commentButton;
        private StackLayout addCommentLayout;

        private bool texting = false;

        public CommentsPage()
        {
            commentStatus = new Label { Style = (Style)Application.Current.Resources["centeredLabelStyle"] };

            CommentCell.ResetEvents();
            CommentCell.complaintEvent += CommentCell_complaintEvent; ;
            CommentCell.deleteEvent += CommentCell_deleteEvent; ;
            commentsList = new CommentListView();

            commentButton = new Button();
            commentButton.Text = AppResources.Comment;
            commentButton.Style = (Style)Application.Current.Resources["buttonStyle"];
            commentButton.Clicked += CommentButton_Clicked;
            
            var layout = new StackLayout();
            layout.BackgroundColor = Color.White;
            layout.Children.Add(commentStatus);
            layout.Children.Add(commentsList);
            layout.VerticalOptions = LayoutOptions.FillAndExpand;

            addCommentLayout = new StackLayout { BackgroundColor = Color.Orange, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
            commentEditor = new ExpandableEditor { BackgroundColor = Color.White, Margin = new Thickness { Top = 3, Left = 3, Right = 3 }, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.FillAndExpand };
            
            try
            {
                UpdateComments();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
            
            addCommentLayout.Children.Add(commentButton);
            layout.Children.Add(addCommentLayout);

            Content = layout;
        }

        private void CommentCell_deleteEvent(object sender, int e)
        {
            try
            {
                DataService.Instance().DeleteComment(e);
                UpdateComments();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void CommentCell_complaintEvent(object sender, int e)
        {
            try
            {
                DataService.Instance().AddComplaint(e);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void CommentButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if(DataService.Instance().LoggedIn)
                {
                    if(texting)
                    {
                        if (commentEditor.Text != null)
                        {
                            DataService.Instance().AddComment(commentEditor.Text);
                            commentEditor.Text = "";
                            UpdateComments();
                        }
                        addCommentLayout.Children.Remove(commentEditor);
                        texting = false;
                    }
                    else
                    {
                        texting = true;
                        addCommentLayout.Children.Insert(addCommentLayout.Children.IndexOf(commentButton), commentEditor);
                        commentEditor.Focus();
                    }
                }
                else
                    ExceptionHandler.HandleWarning(this, AppResources.LogInToComment);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void UpdateComments()
        {
            var comments = DataService.Instance().GetComments();

            if (comments.Count > 0)
            {
                commentStatus.Text = AppResources.CommentsFound + ": " + comments.Count;
                commentsList.UpdateComments(comments);
            }
            else
            {
                commentStatus.Text = AppResources.NoCommentsFound;
                commentsList.UpdateComments(null);
            }

            commentStatus.Focus();
        }
    }
}
