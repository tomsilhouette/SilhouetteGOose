using Microsoft.Extensions.Logging;
using SilhouetteGOose.Model.GameState;
using SilhouetteGOose.View.Game;
using SilhouetteGOose.View.PlayerSelect;
using SilhouetteGOose.View.Welcome;
using SilhouetteGOose.ViewModel.Game;
using SilhouetteGOose.ViewModel.PlayerSelect;
using SilhouetteGOose.ViewModel.Welcome;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace SilhouetteGOose;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<GameState>();

        builder.Services.AddSingleton<WelcomePageViewModel>();
        builder.Services.AddSingleton<WelcomePage>();

        builder.Services.AddSingleton<GamePageViewModel>();
        builder.Services.AddSingleton<GamePage>();

        builder.Services.AddSingleton<PlayerSelectViewModel>();
        builder.Services.AddSingleton<PlayerSelectPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
