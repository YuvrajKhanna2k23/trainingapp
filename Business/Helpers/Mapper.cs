﻿using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.Helpers
{
    public static class Mapper
    {
        public static List<profileDTO> profileMapper(List<Profile> profiles)
        {
            List<profileDTO> profileDTOs = new List<profileDTO>();
            foreach (Profile profile in profiles)
            {
                profileDTOs.Add(new profileDTO
                {
                    FirstName= profile.FirstName,
                    LastName= profile.LastName,
                    UserName= profile.UserName,
                    Email= profile.Email,
                    ProfileType= profile.ProfileType,
                    CreatedAt= profile.CreatedAt,
                    CreatedBy= profile.CreatedBy,
                    LastUpdatedAt= profile.LastUpdatedAt,
                    LastUpdatedBy= profile.LastUpdatedBy,
                    imagePath= profile.imagePath
                });
            }
            return profileDTOs;
        }

        public static List<ChatDTO> chatMapper(List<Chat> sent, List<Chat> recieved)
        {
            List<ChatDTO> chatDTOs = new List<ChatDTO>();
            ChatDTO sentChats = new ChatDTO();            
            ChatDTO receivedChat = new ChatDTO();
            if(sent.Count > 0)
            {
                sentChats.IsSent= true;
                List<chatFormat> temp = new List<chatFormat>();
                foreach (Chat chat in sent)
                {
                    temp.Add(new chatFormat
                    {
                        content = chat.content,
                        sentAt = chat.sentAt
                    });
                }
                sentChats.chatList = temp;
            }
            if(recieved.Count > 0)
            {
                receivedChat.IsSent = false;
                List<chatFormat> temp = new List<chatFormat>();
                foreach (Chat chat in recieved)
                {
                    temp.Add(new chatFormat
                    {
                        content = chat.content,
                        sentAt = chat.sentAt
                    });
                }
                receivedChat.chatList = temp;
            }
            chatDTOs.Add(sentChats);
            chatDTOs.Add(receivedChat);
            return chatDTOs;
        }
    }
}