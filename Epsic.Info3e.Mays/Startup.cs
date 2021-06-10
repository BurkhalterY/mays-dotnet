using System;
using System.Text;
using Epsic.Info3e.Mays.Authorization;
using Epsic.Info3e.Mays.Config;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Middlewares;
using Epsic.Info3e.Mays.Models;
using Epsic.Info3e.Mays.Seeders;
using Epsic.Info3e.Mays.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Epsic.Info3e.Mays
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

            services.AddControllers();

            services.AddDbContext<MaysDbContext>(x => x.UseSqlite(@"Data Source=Mays.db;"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Epsic.Authx", Version = "v1" });
                // To Enable authorization using Swagger (JWT)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        System.Array.Empty<string>()
                    }
                });
            });

            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            services.AddIdentity<User, IdentityRole>(options => {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;

                // Prevent email-like usernames
                options.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters.Replace("@", "");
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<MaysDbContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);

                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Premium", policy => policy.Requirements.Add(new PremiumRequirement()));
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new AdminRequirement()));
                options.AddPolicy("SameUserPost", policy => policy.Requirements.Add(new SameUserPostRequirement()));
                options.AddPolicy("SameUserComment", policy => policy.Requirements.Add(new SameUserCommentRequirement()));
                options.AddPolicy("Extension", policy => policy.Requirements.Add(new ExtensionRequirement()));
            });

            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<MaysDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddTransient<IPostService, DbPostService>();
            services.AddTransient<ILikeService, DbLikeService>();
            services.AddTransient<IPremiumService, DbPremiumService>();
            services.AddSingleton<IAuthorizationHandler, ExtensionAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddFile("Logs/log.txt", LogLevel.Debug);

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Epsic.Info3e.Mays v1"));
            }
            else
            {
                loggerFactory.AddFile("Logs/log.txt", LogLevel.Error);
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<MaysDbContext>();
                context.Database.Migrate();

                var services = serviceScope.ServiceProvider;

                try
                {
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<User>>();

                    SeedDataApplicationRoles.SeedRoles(rolesManager);
                    SeedDataApplicationUsers.SeedUsers(userManager);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseExpirationCheck();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
