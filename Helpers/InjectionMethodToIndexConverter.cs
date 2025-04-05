using System;
using System.Globalization;
using System.Windows.Data;

namespace StarZInjector.Helpers;

internal sealed class InjectionMethodToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string method)
            return 0;

        return method switch
        {
            "LoadLibraryA" => 0,
            "LoadLibraryW" => 1,
            "LoadLibraryExA" => 2,
            "LoadLibraryExW" => 3,
            _ => 0
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            0 => "LoadLibraryA",
            1 => "LoadLibraryW",
            2 => "LoadLibraryExA",
            3 => "LoadLibraryExW",
            _ => "LoadLibraryA"
        };
    }
}
