using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Sheas_Cealer_Droid.Anims;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Pages;

public partial class AdbPage : ContentPage
{
    private readonly AdbPres AdbPres;
    private readonly IDispatcherTimer CommandLineTimer = Application.Current!.Dispatcher.CreateTimer();

    private bool IsFirstLoaded = true;
    private bool IsNextNavigating = false;

    public AdbPage()
    {
        InitializeComponent();

        BindingContext = AdbPres = new();
    }
    private void AdbPage_Loaded(object sender, EventArgs e)
    {
        if (!IsFirstLoaded)
            return;

        CommandLineTimer.Interval = TimeSpan.FromSeconds(0.1);
        CommandLineTimer.Tick += CommandLineTimer_Tick;
        CommandLineTimer.Start();

        new HeroImageSwitchAnim(PrimaryHeroImage, SecondaryHeroImage).Commit(this, nameof(PrimaryHeroImage) + nameof(SecondaryHeroImage) + nameof(HeroImageSwitchAnim), 8, 10000, repeat: () => true);
        new ViewFloatAnim(CommandButton, ViewFloatAnim.FloatOrientation.Y, 5).Commit(this, nameof(CommandButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);

        IsFirstLoaded = false;
    }
    private void AdbPage_Appearing(object sender, EventArgs e)
    {
        BindingContext = null;
        BindingContext = AdbPres;
    }
    private async void AdbPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        Page nextPage = (Page)typeof(NavigatedFromEventArgs).GetProperty("DestinationPage", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(e)!;

        if (!AdbPres.IsCommandLineExist && nextPage.GetType() == typeof(ReadyPage))
        {
            await Toast.Make(GlobalConst.SkipWarningArray[new Random().Next(4)]).Show();
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"//{nameof(AdbPage)}");
        }
    }
    private void AdbPage_NavigatingFrom(object sender, NavigatingFromEventArgs e) => new PageSwitchAnim(this, IsNextNavigating ? PageSwitchAnim.SwitchDirection.Left : PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.Out).Commit(this, nameof(AdbPage) + nameof(PageSwitchAnim), 8, 100);
    private void AdbPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        Page previousPage = (Page)typeof(NavigatedToEventArgs).GetProperty("PreviousPage", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(e)!;

        new PageSwitchAnim(this, previousPage.GetType() == typeof(FlagPage) ? PageSwitchAnim.SwitchDirection.Left : PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.In).Commit(this, nameof(AdbPage) + nameof(PageSwitchAnim), 8, 100);
    }

    private async void CommandButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(AdbConst.AdbCommand);
        await Toast.Make(AdbConst._CommandCopiedToastMsg).Show();

        if (AdbPres.IsCommandLineExist)
            return;

        IsNextNavigating = true;

        this.AbortAnimation(nameof(CommandButton) + nameof(ViewFloatAnim));
        new ViewFloatAnim(NextButton, ViewFloatAnim.FloatOrientation.X, 5).Commit(this, nameof(NextButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
    }

    private async void PrevButton_Clicked(object sender, EventArgs e)
    {
        IsNextNavigating = false;

        await Shell.Current.GoToAsync($"//{nameof(FlagPage)}");
    }
    private async void NextButton_Clicked(object sender, EventArgs e)
    {
        IsNextNavigating = true;

        await Shell.Current.GoToAsync($"//{nameof(ReadyPage)}");
    }

    private void CommandLineTimer_Tick(object? sender, EventArgs e)
    {
        if (File.Exists(MainConst.CommandLinePath))
            AdbPres.IsCommandLineExist = true;
    }

    private void PrevSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => PrevButton_Clicked(null!, null!);
    private void NextSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => NextButton_Clicked(null!, null!);
}