using Confluent.Kafka;
using SearchNavigate.Core.Application.Interfaces.Repositories;
using SearchNavigate.Infrastructure.Persistence.Repositories;
using SearchNavigate.Core.Domain.Models;
using Newtonsoft.Json;
using SearchNavigate.Infrastructure.Persistence.Context;

namespace StreamReader;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<Null, string> _consumer;
    // private readonly DbContext _dbContext;
    private readonly IUserViewHistoryRepository userViewHistRepo;

    public Worker(ILogger<Worker> logger, IConsumer<Null, string> consumer, UserViewHistoryRepository uViewRepo)
    {
        // _logger = logger;
        this._consumer = consumer;
        _consumer.Subscribe("viewHistory_v2-topic");
        userViewHistRepo = uViewRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            var consumeResult = _consumer.Consume(stoppingToken);
            Console.Write($"Kafka Offset: {consumeResult.Offset} - ");

            if (consumeResult is null)
                continue;

            var view = JsonConvert.DeserializeObject<DataSeeding.View>(
                consumeResult.Message.Value);

            var viewHistory = new UserViewHistory
            {
                ProductId = view.properties.ProductId,
                UserId = view.user.Id,
                ViewSourse = (ViewSourse)view.context.Source,
                ViewDate = DateTime.Now,
            };
            await userViewHistRepo.AddAsync(viewHistory);
            Console.WriteLine($" User {view.user.UserName} viewed \"{view.properties.ProductName}\" product!");
        }
    }
}
