using System;
using API.Models.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
  public class ChatHub : Hub
  {
    private readonly IUserRepo userRepo;

    public ChatHub(IUserRepo userRepo)
    {
      this.userRepo = userRepo;
    }
    public void Echo(string message, string userId)
    {
      var user = this.userRepo.GetUser(userId);
      var date = DateTime.Now;
      Clients.All.SendAsync("Send", message, user, date);
    }
  }
}