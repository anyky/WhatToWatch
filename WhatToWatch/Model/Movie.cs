using System;
using System.Collections.Generic;

namespace WhatToWatch.Model
{
    public class Movie
    {
        public int MovieId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }

        public string Actors
        {
            get;
            set;
        }

        public string Directors
        {
            get;
            set;
        }

        public string Genres
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string PosterURL
        {
            get;
            set;
        }

        public string FullPosterURL
        {
            get
            {
                //return Service.DataService.Instance().BaseUrl + PosterURL;
                return PosterURL;
            }
        }

        public bool CanComment
        {
            get;
            set;
        }

        public bool InFavorite
        {
            get;
            set;
        }

        private readonly List<Mark> marks = new List<Mark>();
        private readonly List<Commentary> comments = new List<Commentary>();

        public List<Mark> GetMarks()
        {
            return marks;
        }

        public List<Commentary> GetComments()
        {
            return comments;
        }

        public Commentary GetComment(int id)
        {
            return comments.Find(i => i.CommentId == id);
        }

        public void AddNewMark(int id, int type, string name)
        {
            marks.Add(new Mark() { MarkId = id, MarkType = type, Name = name});
        }

        public void AddMarkValue(int id, int value)
        {
            marks.Find(i => i.MarkId == id).Value = value;
        }

        public void AddMarkAverage(int id, float average)
        {
            marks.Find(i => i.MarkId == id).Average = average;
        }
        
        public List<Mark> GetMainMarks()
        {
            return marks.FindAll(i => i.MarkType == 1);
        }

        public int GetMarkValue(int markId)
        {
            return marks.Find(i => i.MarkId == markId).Value;
        }

        public float GetMarkAverage(int markId)
        {
            return marks.Find(i => i.MarkId == markId).Average;
        }
        
        public List<Mark> GetEmotMarks()
        {
            return marks.FindAll(i => i.MarkType == 2);
        }

        public void AddComment(int commentId, string sender, string time, string text, bool last = false)
        {
            var comment = new Commentary() { CommentId = commentId, SenderLogin = sender, SendTime = DateTime.ParseExact(time, "MM/dd/yyyy HH:mm:ss", null).ToLocalTime().ToString(), Text = text, HasComplaint = false };
            if (last)
                comments.Insert(0, comment);
            else
                comments.Add(comment);
        }

        public void HasComplaint(int commentId, bool hasComplaint)
        {
            comments.Find(i => i.CommentId == commentId).HasComplaint = hasComplaint;
        }

        public void DeleteComment(int commentId)
        {
            comments.Remove(comments.Find(i => i.CommentId == commentId));
        }

        public void AddComplaint(int commentId)
        {
            comments.Find(i => i.CommentId == commentId).HasComplaint = true;
        }

        public Movie This
        {
            get { return this; }
        }
    }
}
