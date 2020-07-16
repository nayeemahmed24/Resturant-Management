using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT_Token;
using JWT_Token.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.Error_Handler;
using Repository;
using Repository.Configurations;
using Services.AddonServices;
using Services.AdminAuth;
using Services.AdminAuth.Contracts;
using Services.Email;
using Services.Email.Configuration;
using Services.FileUploadService;
using Services.Helper_Services;
using Services.MenuServices;
using Services.OrderService;
using Services.UserServices;
using Services.Paginator;
using Services.Sort_Service;
using Services.TableServices;

namespace Resturant_Management
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyCorsOrigin = "MyCorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<JwtSetting>(Configuration.GetSection(nameof(JwtSetting)));
            services.AddSingleton<IJwtSetting>(jwtSetting => jwtSetting.GetRequiredService<IOptions<JwtSetting>>().Value);
            
            services.Configure<DatabaseSettings>(
                Configuration.GetSection(nameof(DatabaseSettings)));
            
            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton<IMongoRepository, MongoRepository>();
            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<IAdminVerificationService, AdminVerficationService>();
            services.AddSingleton<IAdminAccessService, AdminAccessService>();
            services.AddSingleton<IAddonService, AddonService>();
            services.AddSingleton<ISortService, SortService>();
            services.AddSingleton<ITableService, TableService>();
            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<IExceptionModelGenerator, ExceptionModelGenerator>();
            services.AddSingleton<IPasswordManager, PasswordManager>();
            services.AddSingleton<IUserAccessService, UserAccessService>();
            services.Configure<MailSetting>(Configuration.GetSection(nameof(MailSetting)));
            services.AddSingleton<IMailSetting>(mailSetting => mailSetting.GetRequiredService<IOptions<MailSetting>>().Value);
            services.AddSingleton<IMailSender, MailSender>();
            services.AddSingleton<CustomTokenValidator>();
            services.AddSingleton<IFileUploadService, FileUploadService>();
            services.AddSingleton<IStoragePathService, StoragePathService>();
            services.AddSingleton<IMenuServices, MenuServices>();
            services.Configure<Routes>(Configuration.GetSection(nameof(Routes)));
            services.AddSingleton<IRoutes>(mailVarify => mailVarify.GetRequiredService<IOptions<Routes>>().Value);
            services.AddSingleton<IPaginator, Paginator>();


            var serviceProvider = services.BuildServiceProvider();

            var jwtService = serviceProvider.GetService<IJwtSetting>();

            var key = Encoding.ASCII.GetBytes(jwtService.SecretKey);
            services.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(auth =>
                {
                    auth.RequireHttpsMetadata = false;
                    auth.SaveToken = false;
                    auth.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                    };
                });
            services.AddCors(options =>
            {
                options.AddPolicy(MyCorsOrigin, builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "http://sequenceweb.mydomain.com:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(MyCorsOrigin);


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
