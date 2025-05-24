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

        if (!AdbPres.IsFirstRunning)
        {
            IsNextNavigating = true;

            new ViewFadeAnim(AdbGrid, ViewFadeAnim.FadeType.In).Commit(this, nameof(AdbGrid) + nameof(ViewFadeAnim), 8, 1000);
        }

        new HeroImageSwitchAnim(PrimaryHeroImage, SecondaryHeroImage).Commit(this, nameof(PrimaryHeroImage) + nameof(SecondaryHeroImage) + nameof(HeroImageSwitchAnim), 8, 10000, repeat: () => true);

        if (!AdbPres.IsCommandLineExist)
            new ViewFloatAnim(CommandButton, ViewFloatAnim.FloatOrientation.Y, -5).Commit(this, nameof(CommandButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        else
        {
            IsNextNavigating = true;

            new ViewFloatAnim(NextImageButton, ViewFloatAnim.FloatOrientation.X, -5).Commit(this, nameof(NextImageButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        }

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

        if (AdbPres.IsCommandLineExist || nextPage.GetType() != typeof(ReadyPage))
            return;

        await Toast.Make(GlobalConst.SkipWarningArray[new Random().Next(4)]).Show();
        await Task.Delay(300);
        await Shell.Current.GoToAsync($"//{nameof(AdbPage)}");
    }
    private void AdbPage_NavigatingFrom(object sender, NavigatingFromEventArgs e) => new PageSwitchAnim(this, IsNextNavigating ? PageSwitchAnim.SwitchDirection.Left : PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.Out).Commit(this, nameof(AdbPage) + nameof(PageSwitchAnim), 8, 100);
    private void AdbPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        Page? previousPage = typeof(NavigatedToEventArgs).GetProperty("PreviousPage", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(e) as Page;

        if (previousPage == null)
            return;

        new PageSwitchAnim(this, previousPage.GetType() == typeof(FlagPage) ? PageSwitchAnim.SwitchDirection.Left : PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.In).Commit(this, nameof(AdbPage) + nameof(PageSwitchAnim), 8, 100);
    }

    private async void CommandButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(GlobalConst.AdbCommand);
        await Toast.Make(GlobalConst._CommandCopiedToastMsg).Show();
    }

    private async void PrevImageButton_Clicked(object sender, EventArgs e)
    {
        if (!AdbPres.IsFirstRunning)
            return;

        IsNextNavigating = false;

        await Shell.Current.GoToAsync($"//{nameof(FlagPage)}");
    }
    private async void NextImageButton_Clicked(object sender, EventArgs e)
    {
        IsNextNavigating = true;

        await Shell.Current.GoToAsync($"//{nameof(ReadyPage)}");
    }

    private void CommandLineTimer_Tick(object? sender, EventArgs e)
    {
        if (!AdbPres.IsFlagCopied)
            return;

        if (File.Exists(GlobalConst.CommandLinePath))
        {
            if (AdbPres.IsCommandLineExist)
                return;

            IsNextNavigating = true;
            AdbPres.IsCommandLineExist = true;

            this.AbortAnimation(nameof(CommandButton) + nameof(ViewFloatAnim));
            new ViewFloatAnim(NextImageButton, ViewFloatAnim.FloatOrientation.X, -5).Commit(this, nameof(NextImageButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        }
        else if (AdbPres.IsCommandLineExist)
        {
            IsNextNavigating = false;
            AdbPres.IsCommandLineExist = false;

            this.AbortAnimation(nameof(NextImageButton) + nameof(ViewFloatAnim));
            new ViewFloatAnim(CommandButton, ViewFloatAnim.FloatOrientation.Y, -5).Commit(this, nameof(CommandButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        }
    }

    private void PrevSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => PrevImageButton_Clicked(null!, null!);
    private void NextSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => NextImageButton_Clicked(null!, null!);
}