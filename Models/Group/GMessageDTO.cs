﻿using System;

namespace ChatApp.Models.Group
{
    public class GMessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string MessageFrom { get; set; }
        public string MessageFromImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RepliedTo { get; set; }
        public string Type { get; set; }
    }
}