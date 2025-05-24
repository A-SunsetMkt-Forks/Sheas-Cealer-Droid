using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Sheas_Cealer_Droid.Anims;

internal class AddImageButtonSlideAnim : Animation
{
    internal enum SlideType { In, Out }

    internal AddImageButtonSlideAnim(ImageButton addImageButton, SlideType slideType)
    {
        if (slideType == SlideType.In)
        {
            Add(0, 1, new(v => addImageButton.TranslationY = v, 60, 0, Easing.CubicOut));
            Add(0, 1, new(v => addImageButton.Rotation = v, -45, 0, Easing.CubicOut));
        }
        else
        {
            Add(0, 1, new(v => addImageButton.TranslationY = v, 0, 60, Easing.CubicIn));
            Add(0, 1, new(v => addImageButton.Rotation = v, 0, 45, Easing.CubicIn));
        }
    }
}