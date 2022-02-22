﻿namespace FrwkQuickWait.Domain.Entity
{
    public class MessageInput
    {
        public int? Status { get; set; }
        public string Method { get; set; }
        public string Content { get; set; }

        public MessageInput(int? status, string method, string content)
        {
            Status = status;
            Method = method;
            Content = content;
        }

    }
}
