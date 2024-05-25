using Avalonia.Data.Converters;
using System.Globalization;

namespace BigMission.Avalonia.Utilities;

public class BoolToYesNoConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool v)
        {
            if (v) return "Yes";
        }
        return "No";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
