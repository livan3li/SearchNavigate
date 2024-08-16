using Confluent.Kafka;
using SearchNavigate.Infrastructure.Persistence.Extensions;
using SearchNavigate.Infrastructure.Persistence.Repositories;
using StreamReader;
using System.Reflection;

internal class Program
{
   private static void Main(string[] args)
   {
      var builder = Host.CreateApplicationBuilder(args);

      var producerConfig = new ConsumerConfig
      {
         BootstrapServers = $"localhost:9092",
         GroupId = "SearchNavigate-Consumer",
         AutoOffsetReset = AutoOffsetReset.Earliest
      };
      builder.Services.AddSingleton(
          new ConsumerBuilder<Null, string>(producerConfig).Build());

      var optBuilder = new DbContextOptBuilder(
         @"Host=localhost; Port=5432; Database=SearchNavigate; 
           User Id=postgres;Password=132652;");
      builder.Services
         .AddSingleton(new UserViewHistoryRepository(optBuilder));

      builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
      builder.Services.AddHostedService<Worker>();

      var host = builder.Build();
      host.Run();
      //   var builder = Host.CreateApplicationBuilder(args);
      //   builder.Services.AddHostedService<Worker>();

      //   var host = builder.Build();
      //   host.Run();
   }
}
