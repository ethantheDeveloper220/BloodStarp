using System.Windows;
using System.Windows.Controls;
using Voidstrap.UI.ViewModels.Settings;

namespace Voidstrap.UI.Converters
{
    public class SettingsControlTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is SettingsControl control && container is FrameworkElement element)
            {
                return control.Type switch
                {
                    ControlType.TextBox => element.FindResource("TextBoxTemplate") as DataTemplate,
                    ControlType.Slider => element.FindResource("SliderTemplate") as DataTemplate,
                    ControlType.ToggleSwitch => element.FindResource("ToggleSwitchTemplate") as DataTemplate,
                    ControlType.ComboBox => element.FindResource("ComboBoxTemplate") as DataTemplate,
                    ControlType.Vector2 => element.FindResource("Vector2Template") as DataTemplate,
                    _ => null
                };
            }

            return null;
        }
    }
}
