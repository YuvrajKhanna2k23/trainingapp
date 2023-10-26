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
            /*int messageFromId = chatService.FetchUserIdByUsername();
            int messageToId = chatService.FetchUserIdByUsername(message.MessageTo);*/
            newMessage = new Message
            {
                Content = message.Content,
                DateTime = DateTime.Now,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                RepliedToId = message.RepliedToId,
                IsReply = message.IsReply,
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




    }

}
