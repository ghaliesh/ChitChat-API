using API.Models.Entities;
using AutoMapper;

namespace API.Resources
{
  public class Mapper : Profile
  {
    public Mapper()
    {
      //Domain models to API models
      CreateMap<AppUser, UserResource>().ForMember(ur => ur.Password, a => a.Ignore());
      CreateMap<Message, MessageResource>();

      // API models to Domain models
      CreateMap<UserResource, AppUser>();
      CreateMap<MessageResource, Message>().ForMember(m => m.User, mr => mr.Ignore());
    }
  }
}