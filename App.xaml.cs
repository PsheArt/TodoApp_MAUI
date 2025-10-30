using Microsoft.Extensions.DependencyInjection;
using TodoMauiApp.Views;

namespace TodoMauiApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
          
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}