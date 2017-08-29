namespace WhatToWatch.Model
{
    public class Commentary
    {
        public int CommentId
        {
            get;
            set;
        }
        public string SenderLogin
        {
            get;
            set;
        }
        public string SendTime
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }

        public bool HasComplaint
        {
            get;
            set;
        }
    }
}
