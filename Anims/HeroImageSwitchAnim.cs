using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Sheas_Cealer_Droid.Anims;

internal class HeroImageSwitchAnim : Animation
{
    internal HeroImageSwitchAnim(Image primaryImage, Image secondaryImage)
    {
        Add(0.30, 0.37, new(v => primaryImage.Scale = v, 1, 0.9, Easing.CubicInOut));
        Add(0.33, 0.37, new(v => primaryImage.Opacity = v, 1, 0, Easing.CubicInOut));
        Add(0.33, 0.37, new(v => secondaryImage.Opacity = v, 0, 1, Easing.CubicInOut));
        Add(0.33, 0.40, new(v => secondaryImage.Scale = v, 0.9, 1, Easing.CubicInOut));
        Add(0.80, 0.87, new(v => secondaryImage.Scale = v, 1, 0.9, Easing.CubicInOut));
        Add(0.83, 0.87, new(v => secondaryImage.Opacity = v, 1, 0, Easing.CubicInOut));
        Add(0.83, 0.87, new(v => primaryImage.Opacity = v, 0, 1, Easing.CubicInOut));
        Add(0.83, 0.90, new(v => primaryImage.Scale = v, 0.9, 1, Easing.CubicInOut));
    }
}