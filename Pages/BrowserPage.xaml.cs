using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Anims;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Pages;

public partial class BrowserPage : ContentPage
{
    private readonly BrowserPres BrowserPres;

    private bool IsFirstLoaded = true;

    public BrowserPage()
    {
        InitializeComponent();

        BindingContext = BrowserPres = new();
    }
    private void BrowserPage_Loaded(object sender, EventArgs e)
    {
        if (!IsFirstLoaded)
            return;

        new ViewFadeAnim(BrowserGrid, ViewFadeAnim.FadeType.In).Commit(this, nameof(BrowserGrid) + nameof(ViewFadeAnim), 8, 1000);
        new HeroImageSwitchAnim(PrimaryHeroImage, SecondaryHeroImage).Commit(this, nameof(PrimaryHeroImage) + nameof(SecondaryHeroImage) + nameof(HeroImageSwitchAnim), 8, 10000, repeat: () => true);

        if (string.IsNullOrEmpty(BrowserPres.BrowserName))
            new ViewFloatAnim(BrowserPicker, ViewFloatAnim.FloatOrientation.Y, -5).Commit(this, nameof(BrowserPicker) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        else
            new ViewFloatAnim(NextButton, ViewFloatAnim.FloatOrientation.X, -5).Commit(this, nameof(NextButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);

        IsFirstLoaded = false;
    }
    private async void BrowserPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        Page nextPage = (Page)typeof(NavigatedFromEventArgs).GetProperty("DestinationPage", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(e)!;

        if (string.IsNullOrEmpty(BrowserPres.BrowserName))
        {
            await Toast.Make(GlobalConst.SkipWarningArray[new Random().Next(4)]).Show();
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"//{nameof(BrowserPage)}");
        }
        else if (!BrowserPres.IsFlagCopied && (nextPage.GetType() == typeof(AdbPage) || nextPage.GetType() == typeof(ReadyPage)))
        {
            await Toast.Make(GlobalConst.SkipWarningArray[new Random().Next(4)]).Show();
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"//{nameof(FlagPage)}");
        }
        else if (!BrowserPres.IsCommandLineExist && nextPage.GetType() == typeof(ReadyPage))
        {
            await Toast.Make(GlobalConst.SkipWarningArray[new Random().Next(4)]).Show();
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"//{nameof(AdbPage)}");
        }
    }
    private void BrowserPage_NavigatingFrom(object sender, NavigatingFromEventArgs e) => new PageSwitchAnim(this, PageSwitchAnim.SwitchDirection.Left, PageSwitchAnim.SwitchType.Out).Commit(this, nameof(BrowserPage) + nameof(PageSwitchAnim), 8, 100);
    private void BrowserPage_NavigatedTo(object sender, NavigatedToEventArgs e) => new PageSwitchAnim(this, PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.In).Commit(this, nameof(BrowserPage) + nameof(PageSwitchAnim), 8, 100);

    private void BrowserPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(BrowserPres.BrowserName))
        {
            if (this.AnimationIsRunning(nameof(BrowserPicker) + nameof(ViewFloatAnim)))
            {
                this.AbortAnimation(nameof(BrowserPicker) + nameof(ViewFloatAnim));
                new ViewFloatAnim(NextButton, ViewFloatAnim.FloatOrientation.X, -5).Commit(this, nameof(NextButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
            }
        }
        else if (this.AnimationIsRunning(nameof(NextButton) + nameof(ViewFloatAnim)))
        {
            this.AbortAnimation(nameof(NextButton) + nameof(ViewFloatAnim));
            new ViewFloatAnim(BrowserPicker, ViewFloatAnim.FloatOrientation.Y, -5).Commit(this, nameof(BrowserPicker) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        }
    }

    private async void NextButton_Clicked(object sender, EventArgs e) => await Shell.Current.GoToAsync($"//{nameof(FlagPage)}");

    private async void PrevSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => await Toast.Make(BrowserConst._EasterEggMsg).Show();
    private void NextSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => NextButton_Clicked(null!, null!);
}