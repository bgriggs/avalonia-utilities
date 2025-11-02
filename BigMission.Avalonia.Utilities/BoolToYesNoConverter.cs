using Avalonia.Data.Converters;
using System.Globalization;

namespace BigMission.Avalonia.Utilities;

/// <summary>
/// Converts boolean values to "Yes" or "No" strings for display in Avalonia UI.
/// </summary>
public class BoolToYesNoConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to "Yes" or "No".
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>"Yes" if the value is true, otherwise "No".</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool v)
        {
            if (v) return "Yes";
        }
        return "No";
    }

    /// <summary>
    /// Converts a value back from "Yes"/"No" to boolean. Not implemented.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Always returns null.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
