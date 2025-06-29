using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.UnitOfWork;
using Domains.Interfaces.IUnitOfWork;
using ERP_BusinessLogic.Context;
using ERP_BusinessLogic.DbInitialize;
using ERP_BusinessLogic.Options;
using ERP_BusinessLogic.Services;
using ERP_Domians.IServices;
using ERP_Domians.Models;
using GP_ERP_SYSTEM_v1._0.Errors;
using GP_ERP_SYSTEM_v1._0.Helpers.AutomapperProfile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace GP_ERP_SYSTEM_v1._0
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Controllers + Self referencing loop handling
            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Swagger settings
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GP_ERP_SYSTEM_v1._0", Version = "v1" });
            });

            // DbContext settings
            services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("ERP_DB_connectionString")));

            // Identity Settings
            services.AddIdentity<ApplicationUser, IdentityRole>(
                opt =>
                {
                    opt.Password.RequiredLength = 8;
                    opt.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<ApplicationDbContext>();

            // Register JwtOptions with Options Pattern 
            services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(opt =>
            {
                JwtOptions jwtOptions = Configuration.GetSection("Jwt").Get<JwtOptions>();

                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    //ValidIssuer = Configuration["Jwt:ValidIssuer"],
                    ValidIssuer = jwtOptions.ValidIssuer,
                    //ValidAudience = Configuration["Jwt:ValidAudience"],
                    ValidAudience = jwtOptions.ValidAudience,
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            services.AddAuthorization();

            services.AddHttpClient();

            // Automapper 
            services.AddAutoMapper(typeof(ApplicationMapper));

            // Unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Database Initializer
            services.AddScoped<IDbInitializer, DbInitializer>();

            // Services Configurations
            services.AddScoped<ISupplierOrderService, SupplierOrderService>();
            services.AddScoped<IManufacturingOrderService, ManufacturingOrderService>();
            services.AddScoped<IDistributionOrderService, DistributionOrderService>();
            services.AddScoped<IAuthService, AuthService>();

            // Overriding ApiController ModelState Default Behavior
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToArray();

                    return new BadRequestObjectResult(new ErrorValidationResponse { Errors = errors });
                };
            });

            // CORS Policy
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // React فقط السماح لـ
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials(); // السماح باستخدام التوكنات والكوكيز
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GP_ERP_SYSTEM_v1._0 v1"));
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GP_ERP_SYSTEM_v1._0 v1"));

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseRouting();

            //app.UseStatusCodePagesWithReExecute("/errors/{0}");
            //app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
