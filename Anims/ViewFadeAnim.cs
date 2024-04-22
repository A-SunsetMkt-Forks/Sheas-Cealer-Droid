using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Sheas_Cealer_Droid.Anims;

internal class ViewFadeAnim : Animation
{
    internal enum FadeType { In, Out }

    internal ViewFadeAnim(View fadeInView, FadeType fadeType)
    {
        if (fadeType == FadeType.In)
        {
            Add(0, 1, new Animation(v => fadeInView.Opacity = v, 0, 1, Easing.CubicInOut));
            Add(0, 1, new Animation(v => fadeInView.Scale = v, 0.9, 1, Easing.CubicInOut));
        }
        else
        {
            Add(0, 1, new Animation(v => fadeInView.Opacity = v, 1, 0, Easing.CubicInOut));
            Add(0, 1, new Animation(v => fadeInView.Scale = v, 1, 0.9, Easing.CubicInOut));
        }
    }
}