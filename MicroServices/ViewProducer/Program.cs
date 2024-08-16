using Confluent.Kafka;
using ViewProducer;

internal class Program
{
   private static void Main(string[] args)
   {
      var builder = Host.CreateApplicationBuilder(args);
      var producerConfig = new ProducerConfig
      {
         BootstrapServers = $"localhost:9092",
         ClientId = "SearchNavigate-Producer"
      };
      builder.Services.AddSingleton(
          new ProducerBuilder<Null, string>(producerConfig).Build());
      builder.Services.AddHostedService<Worker>();
      var host = builder.Build();
      host.Run();
   }
}
