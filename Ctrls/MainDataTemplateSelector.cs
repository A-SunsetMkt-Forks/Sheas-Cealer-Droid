using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Models;

namespace Sheas_Cealer_Droid.Ctrls;

internal class MainDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate NormalTemplate { private get; init; } = null!;
    public DataTemplate NullTemplate { private get; init; } = null!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => item is CealHostRule ? NormalTemplate : NullTemplate;
}