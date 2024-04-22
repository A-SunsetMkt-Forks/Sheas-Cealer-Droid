using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Sheas_Cealer_Droid.Anims;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;

namespace Sheas_Cealer_Droid.Pages;

public partial class ReadyPage : ContentPage
{
    private readonly ReadyPres ReadyPres;

    private bool IsFirstLoaded = true;
    private bool IsNextNavigating = false;

    public ReadyPage()
    {
        InitializeComponent();

        BindingContext = ReadyPres = new();
    }
    private void ReadyPage_Loaded(object sender, EventArgs e)
    {
        if (!IsFirstLoaded)
            return;

        new ViewFloatAnim(StartButton, ViewFloatAnim.FloatOrientation.Y, 5).Commit(this, nameof(StartButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);

        IsFirstLoaded = false;
    }
    private void ReadyPage_Appearing(object sender, EventArgs e)
    {
        BindingContext = null;
        BindingContext = ReadyPres;
    }
    private void ReadyPage_NavigatingFrom(object sender, NavigatingFromEventArgs e)
    {
        if (IsNextNavigating)
            new ViewFadeAnim(ReadyGrid, ViewFadeAnim.FadeType.Out).Commit(this, nameof(ReadyGrid) + nameof(ViewFadeAnim), 8, 1000);
        else
            new PageSwitchAnim(this, PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.Out).Commit(this, nameof(ReadyPage) + nameof(PageSwitchAnim), 8, 100);
    }
    private void ReadyPage_NavigatedTo(object sender, NavigatedToEventArgs e) => new PageSwitchAnim(this, PageSwitchAnim.SwitchDirection.Left, PageSwitchAnim.SwitchType.In).Commit(this, nameof(ReadyPage) + nameof(PageSwitchAnim), 8, 100);

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("IsFirstRunning", false);

        IsNextNavigating = true;

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    private async void PrevButton_Clicked(object sender, EventArgs e)
    {
        IsNextNavigating = false;

        await Shell.Current.GoToAsync($"//{nameof(FlagPage)}");
    }

    private void PrevSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => PrevButton_Clicked(null!, null!);
    private async void NextSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => await Toast.Make(ReadyConst._EasterEggMsg).Show();
}