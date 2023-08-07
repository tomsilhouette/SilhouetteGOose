namespace SilhouetteGOose.View.PlayerSelect;

public partial class PlayerSelectPage : ContentPage
{
	public PlayerSelectPage()
	{
		InitializeComponent();
	}

    private async void StartGameBtn_Clicked(object sender, EventArgs e)
    {
        await AppShell.Current.GoToAsync("///GamePage");
    }
}