using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Repositories;
using API.Models.UnitOfWork;
using API.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
  [Route("/api/account")]
  public class AccountController : ControllerBase
  {
    private readonly SignInManager<AppUser> signInManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserRepo userRepo;
    private readonly IUnitOfWork unitOfWork;
    private readonly IHostingEnvironment host;
    private readonly IConfiguration configuration;
    private readonly UserManager<AppUser> userManager;
    private readonly IMapper mapper;

    public AccountController(SignInManager<AppUser> signInManager, IHttpContextAccessor httpContextAccessor, IUserRepo userRepo, IUnitOfWork unitOfWork, IHostingEnvironment host, UserManager<AppUser> userManager, IMapper mapper, IConfiguration configuration)
    {
      this.signInManager = signInManager;
      this.httpContextAccessor = httpContextAccessor;
      this.userRepo = userRepo;
      this.unitOfWork = unitOfWork;
      this.host = host;
      this.userManager = userManager;
      this.mapper = mapper;
      this.configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserResource user)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);
      var myUser = await userManager.FindByEmailAsync(user.Email);
      var result = await signInManager.CheckPasswordSignInAsync(myUser, user.Password, false);
      if (!result.Succeeded) return BadRequest("Invalid credintial.");
      await signInManager.SignInAsync(myUser, false);
      return Ok(this.GenerateWenToken(myUser, user.Email));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] UserResource user, IFormFile file)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);
      var newUser = mapper.Map<UserResource, AppUser>(user);
      newUser.UserName = user.Email;
      var result = await userManager.CreateAsync(newUser, user.Password);
      if (!result.Succeeded) return BadRequest(result.Errors);
      await signInManager.SignInAsync(newUser, false);
      var token = this.GenerateWenToken(newUser, user.Email).Result.ToString();
      if (!(file is null)) await this.AddPhotoToUser(file, newUser);
      await unitOfWork.Commit();
      return Ok(new UserResultModel() { Token = token, UserId = newUser.Id });
    }

    [HttpGet("whoami")]
    public async Task<IActionResult> GetUserByToken([FromHeader] string token)
    {
      var handler = new JwtSecurityTokenHandler();
      var mytoken = handler.ReadJwtToken(token) as JwtSecurityToken;
      var email = mytoken.Claims.SingleOrDefault(a => a.Type == "sub").Value;
      var user = await userManager.FindByEmailAsync(email);
      return Ok(user);
    }

    private async Task AddPhotoToUser([FromForm] IFormFile file, AppUser user)
    {
      var uploadPath = Path.Combine(host.WebRootPath, "Uploads");
      if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
      var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
      var filePath = Path.Combine(uploadPath, fileName);
      using(var stream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }
      var photo = new Photo() { ImgURL = fileName };
      user.Photo = photo;
      await unitOfWork.Commit();
    }

    [HttpPost("warn")]
    public async Task<IActionResult> WarnUser([FromBody] string userId)
    {
      var user = userRepo.GetUser(userId);
      var newUserState = userRepo.WarnUser(user);
      await unitOfWork.Commit();
      return Ok(newUserState);
    }

    private async Task<object> GenerateWenToken(AppUser user, string email)
    {
      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
      };
      var userEmail = user.Email;
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["JwtExpireDays"]));

      var token = new JwtSecurityToken(
        configuration["JwtIssuer"],
        configuration["JwtIssuer"],
        claims,
        expires : expires,
        signingCredentials : creds
      );
      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}