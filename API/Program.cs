using SearchNavigate.Infrastructure.Persistence.Extensions;
using SearchNavigate.Core.Application.Extensions;

namespace SearchNavigate.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAuthorization();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(
            o => o.AddPolicy(
                    "MyPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                    }
                )
            );

        // Private Services

        string assmPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        await builder.Services.AddPersistanceRegistration(
            builder.Configuration["Db_Connection"],
            assmPath + "/../../../../views.json"); // which is solution root folder.
        builder.Services.AddApplicationRegistration();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
        }

        app.UseHttpsRedirection();
        app.UseCors("MyPolicy");

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
