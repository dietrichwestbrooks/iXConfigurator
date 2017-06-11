using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Wayne.Payment.Products.iXConfigurator.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            Exit += OnExit;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new WindowsBootstrapper();
            bootstrapper.Run();
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();

            var mainWindow = container.Resolve<Window>();

            MessageBox.Show($"Fatal Error: {e.Exception.Message}", mainWindow?.Title, MessageBoxButton.OK, MessageBoxImage.Stop);

            mainWindow?.Close();

            //var mainWindow = container.GetExportedValue<IShellView>() as MetroWindow;
            //
            //mainWindow?.ShowMessageAsync("Application Error", e.Exception.Message, MessageDialogStyle.Affirmative,
            //    mainWindow.MetroDialogOptions);
        }
    }
}
