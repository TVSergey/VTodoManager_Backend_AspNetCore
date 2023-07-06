using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Vtodo.Controllers;
using Vtodo.DataAccess.Postgres;
using Vtodo.DomainServices.Implementation;
using Vtodo.DomainServices.Interfaces;
using Vtodo.Infrastructure.Implementation;
using Vtodo.Infrastructure.Implementation.Options;
using Vtodo.Infrastructure.Implementation.Services;
using Vtodo.Infrastructure.Interfaces.DataAccess;
using Vtodo.Infrastructure.Interfaces.Services;
using Vtodo.UseCases.Handlers.Accounts.Mappings;
using Vtodo.UseCases.Handlers.Boards.Mappings;
using Vtodo.UseCases.Handlers.Projects.Mappings;
using Vtodo.UseCases.Handlers.Projects.Queries.GetProject;
using Vtodo.UseCases.Handlers.Tasks.Mappings;
using Vtodo.Web.Utils;

namespace Vtodo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(typeof(ProjectsAutoMapperProfile));
                cfg.AddProfile(typeof(BoardsAutoMapperProfile));
                cfg.AddProfile(typeof(TasksAutoMapperProfile));
                cfg.AddProfile(typeof(AccountsAutoMapperProfile));
            });
            
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetProjectRequest).Assembly);
            });

            services.AddOptions();
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            services.Configure<HasherOptions>(Configuration.GetSection("HasherOptions"));
            services.Configure<ProjectFilesOptions>(Configuration.GetSection("ProjectFilesOptions"));
            
            services.AddDbContext<IDbContext, AppDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PgSqlConnection"),
                    x => x.MigrationsAssembly("Vtodo.DataAccess.Postgres.Migrations")));
            
            services.AddAuthentication(
                op =>
                {
                    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            ).AddJwtBearer(
                op =>
                {
                    op.SaveToken = true;
                    op.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JwtOptions:Issuer"],
                        ValidAudience = Configuration["JwtOptions:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtOptions:Key"]!)),
                        ClockSkew = TimeSpan.Zero,
                    };
                }
            );

            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IProjectSecurityService, ProjectSecurityService>();
            services.AddScoped<IProjectsFilesService, ProjectsFilesService>();
            services.AddScoped<ICurrentAccountService, CurrentAccountService>();
            services.AddScoped<IClientInfoService, ClientInfoService>();
            services.AddScoped<IFileManagerService, FileManagerService>();

            services.AddHttpContextAccessor();
            
            services.AddCors();

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
            
            services.AddControllers()
                .AddApplicationPart(typeof(ProjectsController).Assembly);

            services.AddSwaggerGen(config =>
            {
                const string xmlFileProjectControllersDocs = "Vtodo.Controllers.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileProjectControllersDocs);
                config.IncludeXmlComments(xmlPath);
                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.RoutePrefix = string.Empty;
                config.SwaggerEndpoint("swagger/v1/swagger.json", "VTodo private api");
            });
            
            app.UseHttpsRedirection();
            app.UseCors(x => x
                .WithOrigins(Configuration.GetValue<string>("ClientAddress"))
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader());
            
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always
            });
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}