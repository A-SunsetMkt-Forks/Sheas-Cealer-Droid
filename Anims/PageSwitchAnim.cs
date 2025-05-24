using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Sheas_Cealer_Droid.Anims;

internal class PageSwitchAnim : Animation
{
    internal enum SwitchDirection { Left, Right }
    internal enum SwitchType { In, Out }

    internal PageSwitchAnim(Page switchPage, SwitchDirection switchDirection, SwitchType switchType)
    {
        if (switchType == SwitchType.In)
            Add(0, 1, new(v => switchPage.TranslationX = v, switchDirection == SwitchDirection.Left ? switchPage.Width : -switchPage.Width, 0, Easing.CubicIn));
        else
            Add(0, 1, new(v => switchPage.TranslationX = v, 0, switchDirection == SwitchDirection.Left ? -switchPage.Width : switchPage.Width, Easing.CubicOut));
    }
}