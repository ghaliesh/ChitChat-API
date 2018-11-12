using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Hubs;
using API.Models.Entities;
using API.Models.Repositories;
using API.Models.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
      {
        builder.WithOrigins("http://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
      }));

      services.AddSignalR();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<IUserRepo, UserRepo>();
      services.AddScoped<IMessageRepository, MessageRepository>();
      services.AddAutoMapper();
      services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<APIDbContext>();
      services.AddDbContext<APIDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
      JwtSecurityTokenHandler.DefaultInboundClaimFilter.Clear();

      services.AddHttpContextAccessor();
      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
      });
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(config =>
      {
        config.RequireHttpsMetadata = false;
        config.SaveToken = true;
        config.TokenValidationParameters = new TokenValidationParameters

        {
          ValidIssuer = Configuration["JwtIssuer"],
          ValidAudience = Configuration["JwtIssuer"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
          ClockSkew = TimeSpan.Zero // remove delay of token when expire
        };
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
        app.UseHttpsRedirection();
      }
      app.UseCors("CorsPolicy");

      app.UseSignalR(route =>
      {
        route.MapHub<ChatHub>("/hubs/chat");
      });
      app.UseMvc();
    }
  }
}