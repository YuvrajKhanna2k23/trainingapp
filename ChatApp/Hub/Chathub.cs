using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Context;
using ChatApp.Models.MessageModel;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp
{
    public class Chathub: Hub
    {
        private readonly ArgusChatContext context;
        private readonly IChatService chatService;

        public Chathub(ArgusChatContext context, IChatService chatservice)
        {
            this.context = context;
            this.chatService = chatservice;
        }

      
        // build in method for starting the connection when we start connection
        #region OneToOneHub
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task ConnectDone(string userName)
        {
            string curSignalId = Context.ConnectionId;
            Profile user = context.Profiles.FirstOrDefault(p => p.UserName == userName);

            if (user != null)
            {

                if (this.context.Connections.Any(u => u.ProfileId == user.Id))
                {
                    IEnumerable<Connection> users = this.context.Connections.Where(u => u.ProfileId == user.Id);
                    this.context.Connections.RemoveRange(users);
                    context.SaveChanges();
                }

                Connection connUser = new Connection()
                {
                    ProfileId = user.Id,
                    SignalId = curSignalId,
                    DateTime = DateTime.Now,
                };
                await context.Connections.AddAsync(connUser);
                context.SaveChanges();

                await Clients.Caller.SendAsync("ResponseSuccess", user);
            }
            else
            {
                await Clients.Client(curSignalId).SendAsync("ResponseFail");
            }

        }

        public void ConnectRemove(string userName)
        {
            if (userName != null)
            {
                int userId = context.Profiles.FirstOrDefault(u => u.UserName == userName).Id;
                var connect = context.Connections.FirstOrDefault(u => u.ProfileId == userId);
                context.Connections.Remove(connect);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }

        public async Task sendMsg(TextMessageModel message)
        {
            Message newMessage = null;
            TextMessageModel response = null;
            string replyMessage;
            int messageFromId = message.SenderId;
            int messageToId = message.ReceiverId;
            newMessage = new Message
            {
                Content = message.Content,
                DateTime = DateTime.Now,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                RepliedToId = message.RepliedToId,
                IsReply = message.IsReply,
                RepliedContent = message.RepliedContent,
                IsSeen = 0,
                Type = "Null",
            };
            context.Messages.Add(newMessage);
            context.SaveChanges();
            if (message.RepliedToId == 0)
            {
                replyMessage = null;
            }
            else
            {
                var msg = context.Messages.FirstOrDefault(msg => msg.Id == message.RepliedToId);
                replyMessage = msg.Content;
            }
            response = new TextMessageModel
            {
                Id = newMessage.Id,
                RepliedContent = newMessage.RepliedContent,
                Content = newMessage.Content,
                DateTime = DateTime.Now,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                RepliedToId = message.RepliedToId,
                IsReply = message.RepliedToId,
                IsSeen = 0,
                Type = "Null",
            };
            this.chatService.ResponsesToUsersMessage(message.SenderId, message.ReceiverId, response);
        }


        #endregion

       /* #region GroupHub

        public async Task sendGroupMsg(GMessageInModel message)
        {
            GroupMessage newMessage = null;
            GMessageSendModel response = null;
            string replyMessage;
            int messageFromId = chatService.FetchUserIdByUsername();
            int groupId = message.GroupId;
            newMessage = new GroupMessages
            {
                Content = message.Content,
                CreatedAt = DateTime.Now,
                MessageFrom = messageFromId,
                GrpId = groupId,
                RepliedTo = (int)message.RepliedToId,
                Type = null,
            };
            context.GroupMessages.Add(newMessage);
            context.SaveChanges();
            if (message.RepliedTo == 0)
            {
                replyContent = "";
            }
            else
            {
                var msg = context.GroupMessages.FirstOrDefault(msg => msg.Id == message.RepliedToId);
                replyMessage = msg.Content;
            }
            var profile = context.Profiles.FirstOrDefault(p => p.Id == messageFromId);
            response = new GMessageSendModel
            {
                Id = newMessage.Id,
                Content = newMessage.Content,
                CreatedAt = (DateTime)newMessage.CreatedAt,
                MessageFrom = message.MessageFrom,
                MessageFromImage = profile.ImagePath,
                RepliedTo = replyMessage,
                Type = null,
            };
            var groupMemberIds = context.GroupMembers.Where(u => u.GrpId == groupId).Select(u => u.ProfileId).ToList();
            foreach (var memberId in groupMemberIds)
            {
                var connection = context.Connections.FirstOrDefault(u => u.ProfileId == memberId);
                if (connection != null)
                {
                    await Clients.Clients(connection.SignalId).SendAsync("RecieveMessageGroup", response);
                }
            }
        }
        #endregion*/

    }

}
