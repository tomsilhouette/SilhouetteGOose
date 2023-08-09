using SilhouetteGOose.ViewModel.PlayerSelect;

namespace SilhouetteGOose.View.PlayerSelect;

public partial class PlayerSelectPage : ContentPage
{
    public PlayerSelectViewModel viewModel;
	public PlayerSelectPage(PlayerSelectViewModel vm)
	{
		InitializeComponent();
        BindingContext = viewModel = vm;
	}

    private async void StartGameBtn_Clicked(object sender, EventArgs e)
    {
        await AppShell.Current.GoToAsync("///GamePage");
    }
}