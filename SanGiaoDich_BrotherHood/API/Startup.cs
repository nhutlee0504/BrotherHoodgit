using API.Data;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Title", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter 'Bearer' [space] and then your token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } }
                });
            });

            // Load JWT configuration from appsettings.json
            var jwtSection = Configuration.GetSection("JWT");
            var key = Encoding.UTF8.GetBytes(jwtSection["Secret"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["ValidIssuer"],
                    ValidAudience = jwtSection["ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
			services.AddDbContext<ApplicationDbContext>(options =>
		 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
			 sqlOptions => sqlOptions.EnableRetryOnFailure(
				 maxRetryCount: 3, // Số lần thử lại tối đa
				 maxRetryDelay: TimeSpan.FromSeconds(5), // Thời gian tối đa giữa các lần thử lại
				 errorNumbersToAdd: null) // Các mã lỗi SQL có thể thử lại
		 )
	 );

			services.AddHttpContextAccessor(); // ??ng kư IHttpContextAccessor
            services.AddScoped<IProduct, ProductResponse>();
            services.AddScoped<IFavorite, FavoriteResponse>();
            services.AddScoped<IImageProduct, ImageProductResponse>();
            //services.AddScoped<IMessage, MessageResponse>();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IAdmin, AdminService>();
            services.AddScoped<IRating, RatingService>();
            services.AddScoped<IAddressDetail, AddressDetailResponse>();
            services.AddScoped<IBill, BillResponse>();
            services.AddScoped<IBillDetail, BillDetailResponse>();
            services.AddScoped<ICart, CartResponse>();
            services.AddScoped<ICategory, CategoryResponse>();
            services.AddScoped<ICartItem, CartItemResponse>();
            services.AddScoped<IConversation, ConversationRespone>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseStaticFiles(); // Thêm ḍng này ?? cho phép ph?c v? t?p t?nh

            app.UseRouting();
            app.UseHttpsRedirection(); // Thêm n?u b?n mu?n s? d?ng HTTPS
            app.UseAuthentication(); // ??m b?o dùng tr??c app.UseAuthorization()
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
