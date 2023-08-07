using SilhouetteGOose.ViewModel.Game;
using SkiaSharp;
using System;
using SkiaSharp.Views.Maui;
using System.Diagnostics;

namespace SilhouetteGOose.View.Game;

public partial class GamePage : ContentPage
{
    private readonly GamePageViewModel viewModel;

    public GamePage(GamePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    protected override void OnAppearing()
    {
        Debug.WriteLine("ONAPPEARINGONAPPEARINGONAPPEARINGONAPPEARINGONAPPEARINGONAPPEARING");
        viewModel.TickEvent += (_, _) => canvasView?.InvalidateSurface();

        viewModel.StartGame();
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;

        canvas.Clear(SKColors.White);

        // Set instance of canvas in ViewModel
        viewModel.SetCanvas(canvas);

        viewModel.DrawGame();
    }
}


