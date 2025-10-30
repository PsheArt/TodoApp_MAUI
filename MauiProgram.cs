using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TodoMauiApp.Services;
using Microsoft.Extensions.DependencyInjection;
using TodoMauiApp.Views;

namespace TodoMauiApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<IThemeService, ThemeService>();
            builder.Services.AddSingleton<ITodoService, TodoService>();
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
