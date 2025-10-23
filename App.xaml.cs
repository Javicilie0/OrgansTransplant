using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OrgnTransplant.Data;
using OrgnTransplant.ViewModels;
using OrgnTransplant.Views;
using OrgnTransplant.Utilities;

namespace OrgnTransplant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure Dependency Injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Show MainWindow with DI
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Get connection string
            string connectionString = ConfigurationHelper.GetConnectionString();

            // Register Repositories
            services.AddSingleton<IDonorRepository>(provider => new DonorRepository(connectionString));
            services.AddSingleton<IMessageRepository>(provider => new MessageRepository(connectionString));

            // Register Services
            services.AddSingleton<IDonorService, DonorService>();
            services.AddSingleton<IMessageService, MessageService>();

            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<MessagesViewModel>();

            // Register Windows
            services.AddTransient<MainWindow>();
            services.AddTransient<MessagesWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }

}
