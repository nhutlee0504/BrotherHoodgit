

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SanGiaoDich_BrotherHood.Server.Configurations;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Server.Services;
using System;
using System.Text;

namespace SanGiaoDich_BrotherHood.Server
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
            services.AddControllers();
            services.AddRazorPages();
			services.AddDbContext<ApplicationDbContext>(options =>
	  options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
		  sqlOptions => sqlOptions.EnableRetryOnFailure(
			  maxRetryCount: 3, // Số lần thử lại tối đa
			  maxRetryDelay: TimeSpan.FromSeconds(5), // Thời gian tối đa giữa các lần thử lại
			  errorNumbersToAdd: null) // Các mã lỗi SQL có thể thử lại
	  )
  );

			services.AddHttpClient();

            // Swagger configuration
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

            // JWT configuration
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

			// Cấu hình chung cho các Authentication schemes (Google, Facebook)
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie() // Cấu hình Cookie Authentication một lần duy nhất
			.AddGoogle(options =>
			{
				options.ClientId = Configuration["Authentication:Google:ClientId"];
				options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
				options.CallbackPath = "/signin-google"; // Đảm bảo rằng bạn có route này trong Configure
				options.SaveTokens = true;
			})
			.AddFacebook(options =>
			{
				options.AppId = Configuration["Authentication:Facebook:AppId"];
				options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
				options.Scope.Add("email");
				options.Fields.Add("email");
				options.Fields.Add("name");
				options.SaveTokens = true;
			});


			// Dependency injections
			services.AddHttpContextAccessor();
            services.AddScoped<IProduct, ProductResponse>();
            services.AddScoped<IFavorite, FavoriteResponse>();
            services.AddScoped<IImageProduct, ImageProductResponse>();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IAdmin, AdminService>();
            services.AddScoped<IConversation, ConversationRespone>();
            services.AddScoped<IRating, RatingService>();
            services.AddScoped<IAddressDetail, AddressDetailResponse>();
            services.AddScoped<IBill, BillResponse>();
            services.AddScoped<IBillDetail, BillDetailResponse>();
            services.AddScoped<ICart, CartResponse>();
            services.AddScoped<ICategory, CategoryResponse>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<ICartItem, CartItemReponse>();
            services.AddScoped<FirebaseStorageService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IVnpayThongkeService,VnpayThongkeService>();
            services.AddScoped<IMessage,MessageResponse>();
            services.AddSingleton<ProfanityFilterService>();

            services.AddHttpClient();

            // Đăng ký các dịch vụ của bạn (ví dụ: ESMSService)
            services.AddSingleton<ESMSService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.WithOrigins("https://localhost:5001")  // Thêm URL nguồn của bạn
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAllOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
