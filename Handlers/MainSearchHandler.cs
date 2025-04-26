using Microsoft.Maui.Controls;
using Sheas_Cealer_Droid.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sheas_Cealer_Droid.Handlers;

internal class MainSearchHandler : SearchHandler
{
    internal static readonly BindableProperty SearchCollectionProperty = BindableProperty.Create(nameof(SearchCollection), typeof(ObservableCollection<CealHostRule>), typeof(MainSearchHandler));
    internal ObservableCollection<CealHostRule> SearchCollection
    {
        private get => (ObservableCollection<CealHostRule>)GetValue(SearchCollectionProperty);
        init => SetValue(SearchCollectionProperty, value);
    }

    internal event EventHandler<CealHostRule>? ItemSelected;

    protected override void OnQueryChanged(string oldValue, string newValue)
    {
        base.OnQueryChanged(oldValue, newValue);

        ItemsSource = SearchCollection.Where(cealHostRule => cealHostRule.Domain.Contains(newValue) || (cealHostRule.Sni?.Contains(newValue) ?? false) || cealHostRule.Ip.Contains(newValue));
    }

    protected override void OnItemSelected(object item) => ItemSelected?.Invoke(this, (CealHostRule)item);
}