using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Repositories;
using API.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Route("/api/messages")]
  public class MessageController : ControllerBase
  {
    private readonly IMessageRepository messageRepo;
    private readonly IUserRepo userRepo;
    private readonly IMapper mapper;

    public MessageController(IMessageRepository messageRepo, IUserRepo userRepo, IMapper mapper)
    {
      this.messageRepo = messageRepo;
      this.userRepo = userRepo;
      this.mapper = mapper;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddMessage([FromBody] MessageResource model)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState.ValidationState);
      var message = mapper.Map<MessageResource, Message>(model);
      var author = userRepo.GetUser(model.UserId);
      if (author is null) return BadRequest("Message must have a user");
      message.User = author;
      message = await messageRepo.AddMessage(message);
      var result = new MessageResult() { User = author, Payload = message.Payload };
      return Ok(result);
    }
    private async Task<IActionResult> AddAMessage([FromBody] MessageResource model)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState.ValidationState);
      var message = mapper.Map<MessageResource, Message>(model);
      var author = userRepo.GetUser(model.UserId);
      if (author is null) return BadRequest("Message must have a user");
      message.User = author;
      message = await messageRepo.AddMessage(message);
      var result = new MessageResult() { User = author, Payload = message.Payload };
      return Ok(result);
    }
  }
}