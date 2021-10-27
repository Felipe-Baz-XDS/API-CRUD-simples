using System.Collections.Generic;
using MeuTodo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace MeuTodo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<AppDbContext>();

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Usuario/Login";
            });

            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1",new OpenApiInfo {
                    Title = "Empresa X",
                    Version = "v1"
                });
                c.EnableAnnotations();
                //Primeiro define o esquema de segurança
                c.AddSecurityDefinition("Bearer", //Nome do esquema de segurança
                    new OpenApiSecurityScheme{
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http, //Foi setado o esquema como HTTP desde que estamos usando a autenticação bearer
                    Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                { 
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Default",
                    pattern:"{controller=Home}/{action=Index}/{Id?}");
            });

            

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //quando abrir api swagger é apresentado se string.Empty
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }
    }
}
