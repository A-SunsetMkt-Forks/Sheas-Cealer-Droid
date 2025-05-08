using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Anims;
using Sheas_Cealer_Droid.Consts;
using Sheas_Cealer_Droid.Preses;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sheas_Cealer_Droid.Pages;

public partial class FlagPage : ContentPage
{
    private readonly FlagPres FlagPres;

    private bool IsFirstLoaded = true;
    private bool IsNextNavigating = false;

    public FlagPage()
    {
        InitializeComponent();

        BindingContext = FlagPres = new();
    }
    private void FlagPage_Loaded(object sender, EventArgs e)
    {
        if (!IsFirstLoaded)
            return;

        new HeroImageSwitchAnim(PrimaryHeroImage, SecondaryHeroImage).Commit(this, nameof(PrimaryHeroImage) + nameof(SecondaryHeroImage) + nameof(HeroImageSwitchAnim), 8, 10000, repeat: () => true);

        if (!FlagPres.IsFlagCopied)
            new ViewFloatAnim(LinkButton, ViewFloatAnim.FloatOrientation.Y, -5).Commit(this, nameof(LinkButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        else
        {
            IsNextNavigating = true;

            new ViewFloatAnim(NextButton, ViewFloatAnim.FloatOrientation.X, -5).Commit(this, nameof(NextButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
        }

        IsFirstLoaded = false;
    }
    private void FlagPage_Appearing(object sender, EventArgs e)
    {
        BindingContext = null;
        BindingContext = FlagPres;
    }
    private async void FlagPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        Page nextPage = (Page)typeof(NavigatedFromEventArgs).GetProperty("DestinationPage", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(e)!;

        if (!FlagPres.IsFlagCopied && (nextPage.GetType() == typeof(AdbPage) || nextPage.GetType() == typeof(ReadyPage)))
        {
            await Toast.Make(GlobalConst.SkipWarningArray[new Random().Next(4)]).Show();
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"//{nameof(FlagPage)}");
        }
        else if (!FlagPres.IsCommandLineExist && nextPage.GetType() == typeof(ReadyPage))
        {
            await Toast.Make(GlobalConst.SkipWarningArray[new Random().Next(4)]).Show();
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"//{nameof(AdbPage)}");
        }
    }
    private void FlagPage_NavigatingFrom(object sender, NavigatingFromEventArgs e) => new PageSwitchAnim(this, IsNextNavigating ? PageSwitchAnim.SwitchDirection.Left : PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.Out).Commit(this, nameof(FlagPage) + nameof(PageSwitchAnim), 8, 100);
    private void FlagPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        Page previousPage = (Page)typeof(NavigatedToEventArgs).GetProperty("PreviousPage", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(e)!;

        new PageSwitchAnim(this, previousPage.GetType() == typeof(BrowserPage) ? PageSwitchAnim.SwitchDirection.Left : PageSwitchAnim.SwitchDirection.Right, PageSwitchAnim.SwitchType.In).Commit(this, nameof(FlagPage) + nameof(PageSwitchAnim), 8, 100);
    }

    private async void LinkButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(GlobalConst.FlagUrl);
        await Toast.Make(GlobalConst._LinkCopiedToastMsg).Show();

        if (FlagPres.IsFlagCopied)
            return;

        IsNextNavigating = true;
        FlagPres.IsFlagCopied = true;

        this.AbortAnimation(nameof(LinkButton) + nameof(ViewFloatAnim));
        new ViewFloatAnim(NextButton, ViewFloatAnim.FloatOrientation.X, -5).Commit(this, nameof(NextButton) + nameof(ViewFloatAnim), 8, 3000, repeat: () => true);
    }

    private async void PrevButton_Clicked(object sender, EventArgs e)
    {
        IsNextNavigating = false;

        await Shell.Current.GoToAsync($"//{nameof(BrowserPage)}");
    }
    private async void NextButton_Clicked(object sender, EventArgs e)
    {
        IsNextNavigating = true;

        await Shell.Current.GoToAsync($"//{nameof(AdbPage)}");
    }

    private void PrevSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => PrevButton_Clicked(null!, null!);
    private void NextSwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e) => NextButton_Clicked(null!, null!);
}