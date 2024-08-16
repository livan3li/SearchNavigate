using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SearchNavigate.Core.Application.Extensions;
public static class Registration
{
   public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
   {
      var assm = Assembly.GetExecutingAssembly();
      services.AddMediatR(assm);
      services.AddAutoMapper(assm);
      // services.AddValidatorsFromAssembly(assm);
      return services;
   }
}
