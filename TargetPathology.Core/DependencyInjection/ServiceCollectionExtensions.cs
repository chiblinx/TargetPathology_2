using Microsoft.Extensions.DependencyInjection;
using TargetPathology.Core.Services;

namespace TargetPathology.Core.DependencyInjection
{
	/// <summary>
	/// Provides extension methods to register services to the dependency injection container.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers the necessary services to enable serial port monitoring and management.
		/// </summary>
		/// <param name="services">The service collection to which the services will be registered.</param>
		/// <returns>The service collection to allow for fluent chaining of registrations.</returns>
		public static IServiceCollection AddSerialPortServices(this IServiceCollection services)
		{
			services.AddSingleton<StatisticsTracker>();
			services.AddTransient<MessageProcessor>();
			services.AddHostedService<SerialPortWatcher>();
			services.AddSingleton<ISerialPortManager, SerialPortManager>();

			return services;
		}
	}
}
