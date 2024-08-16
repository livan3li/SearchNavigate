using Confluent.Kafka;

namespace ViewProducer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IProducer<Null, string> _producer;

    private string viewJsonFilePath = "../../views.json"; // which is solution root directory.

    private StreamReader streamReader;
    public Worker(ILogger<Worker> logger, IProducer<Null, string> producer)
    {
        _logger = logger;
        _producer = producer;
        try
        {
            streamReader = new StreamReader(viewJsonFilePath);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var line = streamReader.ReadLine();
            await _producer.ProduceAsync(
                "viewHistory_v2-topic",
                new Message<Null, string>
                {
                    Value = line
                });
            await Task.Delay(10000, stoppingToken);
        }
    }
}
