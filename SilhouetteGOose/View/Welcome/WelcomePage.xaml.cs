namespace SilhouetteGOose.View.Welcome;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}

    private async void StartNewGameBtn_Clicked(object sender, EventArgs e)
    {
        await AppShell.Current.GoToAsync("///PlayerSelectPage");

    }
}

