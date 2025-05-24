using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Sheas_Cealer_Droid.Anims;

internal class ViewFloatAnim : Animation
{
    internal enum FloatOrientation { X, Y }

    internal ViewFloatAnim(View floatView, FloatOrientation floatOrientation, double floatOffset)
    {
        if (floatOrientation == FloatOrientation.X)
        {
            Add(0, 0.5, new(v => floatView.TranslationX = v, 0, floatOffset, Easing.SinInOut));
            Add(0.5, 1, new(v => floatView.TranslationX = v, floatOffset, 0, Easing.SinInOut));
        }
        else
        {
            Add(0, 0.5, new(v => floatView.TranslationY = v, 0, floatOffset, Easing.SinInOut));
            Add(0.5, 1, new(v => floatView.TranslationY = v, floatOffset, 0, Easing.SinInOut));
        }
    }
}