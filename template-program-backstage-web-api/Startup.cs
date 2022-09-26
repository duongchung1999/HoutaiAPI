using Backend.Controllers.v2;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using Utils;

namespace template_program_backstage_web_api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            //启用JWT
            //services
            //    .AddAuthentication(Options => {
            //        Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(options => {
            //        options.TokenValidationParameters = new TokenValidationParameters {
            //            ValidIssuer = "account_manager",
            //            ValidAudience = "user",
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("43bc972595573795"))
            //        };
            //    });
            services.AddJwt<JwtHandler>();

            //services.AddCorsAccessor();
            services.AddCors(options => {
                options.AddPolicy(name: "ABC", builder => {
                    builder
                        .WithExposedHeaders(new string[] { "access-token", "x-access-token" })
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            //认证与鉴权
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, b => {
                    //b.LoginPath = "/Home/Login";
                });
            //services.AddJwt<JwtHandler>();

            services
                .AddControllers()
               .AddInjectWithUnifyResult<RESTfulResultProvider>()
               .AddFriendlyException()
               .AddDataValidation()
               ;

            //JSON序列化
            services
                .AddMvc(options => { options.EnableEndpointRouting = false; })
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                });

            services.AddDatabaseAccessor(options => {
                options.AddDbPool<MyDbContext>(providerName: default, optionBuilder: opt => {
                    opt.UseBatchEF_MySQLPomelo();
                });
            });

            services.AddVirtualFileServer();

            services.AddMvcFilter<RequestLogFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseUnifyResultStatusCodes();

            app.UseSpecificationDocuments();


            //app.UseHttpsRedirection();
            //app.UseMvc();
            app.UseRouting();
            //app.UseCorsAccessor();
            app.UseCors("ABC");

            // Token认证
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            //app.UseStaticFiles(new StaticFileOptions() {
            //    FileProvider = new PhysicalFileProvider(StaticResourcesController.BasePath),
            //    RequestPath = "/static-resources"

            //});
        }
    }
}
