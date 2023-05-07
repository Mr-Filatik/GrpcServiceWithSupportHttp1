using Google.Api;
using GrpcServiceWithSupportHttp1.Database;
using GrpcServiceWithSupportHttp1.Database.Services;
using GrpcServiceWithSupportHttp1.Filters;
using GrpcServiceWithSupportHttp1.Interceptors;
using GrpcServiceWithSupportHttp1.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GrpcServiceWithSupportHttp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Builder and services

            var builder = WebApplication.CreateBuilder(args);

            #region Swagger

            string? connection = builder.Configuration.GetConnectionString("DefaultConnection"); //change delete 1
            builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connection));
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAttemptService, AttemptService>();

            #endregion

            builder.Services.AddGrpc().AddJsonTranscoding()
                .AddServiceOptions<UserGrpcService>(options => options.Interceptors.Add<AuthorizationInterceptor>())
                .AddServiceOptions<AttemptGrpcService>(options => options.Interceptors.Add<AuthorizationInterceptor>());

            #region Swagger

            builder.Services.AddGrpcSwagger();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
                c.OperationFilter<AddRequiredHeaderParameter>();
            });

            #endregion

            #region CORS

            builder.Services.AddCors(options => 
                options.AddPolicy("All", 
                corsBuilder => corsBuilder.AllowAnyOrigin()
                                          .AllowAnyMethod()
                                          .AllowAnyHeader()));

            #endregion

            #endregion

            #region Application

            var app = builder.Build();

            app.UseCors("All");

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            app.MapGrpcService<TestService>();
            app.MapGrpcService<AuthGrpcService>();
            app.MapGrpcService<UserGrpcService>();
            app.MapGrpcService<AttemptGrpcService>();

            app.Run();

            #endregion
        }
    }
}