﻿using ChatApp.Business.Helpers;
using System;

namespace ChatApp.Context.EntityClasses
{
	public class Message
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public int MessageFrom { get; set; }
		public int MessageTo { get; set; }
		public DateTime? CreatedAt { get; set; }
	}
}
