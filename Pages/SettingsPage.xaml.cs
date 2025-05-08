using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Preses;

namespace Sheas_Cealer_Droid.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsPres SettingsPres;

    public SettingsPage()
    {
        InitializeComponent();

        BindingContext = SettingsPres = new();
    }
    private void SettingsPage_NavigatedFrom(object sender, NavigatedFromEventArgs e) => BrowserPicker.BindingContext = null;
}