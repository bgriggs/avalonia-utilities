using Microsoft.Extensions.Configuration;

namespace BigMission.Avalonia.Utilities.Settings;

/// <summary>
/// Provides settings for Windows using standard appsettings.json for application settings 
/// and a user-specific settings file saved in the user's isolated storage, such as 
/// <SYSTEMDRIVE>\Users\<user>\AppData\Local for non-roaming users.
/// </summary>
public class WindowsSettingsProvider : ISettingsProvider
{
    private readonly string userSettingsFile;
    private readonly IConfiguration? userSettings;

    private readonly IConfiguration? appSettings;
    private readonly string appSettingsFile = "appsettings.json";

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsSettingsProvider"/> class.
    /// Sets up user settings in isolated storage and loads application settings from appsettings.json if available.
    /// </summary>
    public WindowsSettingsProvider()
    {
        userSettingsFile = $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}-settings.json";

        var configurationBuilder = new ConfigurationBuilder();
        userSettings = configurationBuilder.Add((Action<WritableJsonConfigurationSource>)(s =>
        {
            s.FileProvider = new IsolatedFileProvider();
            s.Path = userSettingsFile;
            s.Optional = true;
            s.ReloadOnChange = true;
        })).Build();
        
        var dir = Directory.GetCurrentDirectory();
        var appSettingsPath = Path.Combine(dir, appSettingsFile);
        if (!string.IsNullOrEmpty(appSettingsPath) && File.Exists(appSettingsPath))
        {
            var cb = new ConfigurationBuilder();
            cb.AddJsonFile(appSettingsPath);
            appSettings = cb.Build();
        }
    }

    private static T? GetValue<T>(IConfiguration? config, string key)
    {
        if (config is not null)
        {
            return config.GetValue<T>(key);
        }

        return default;
    }

    /// <summary>
    /// Gets a user-specific setting value from isolated storage.
    /// </summary>
    /// <typeparam name="T">The type to convert the setting value to.</typeparam>
    /// <param name="key">The configuration key to retrieve.</param>
    /// <returns>The setting value, or default if not found.</returns>
    public T? GetUserValue<T>(string key)
    {
        return GetValue<T>(userSettings, key);
    }

    /// <summary>
    /// Saves a user-specific setting value to isolated storage.
    /// </summary>
    /// <param name="key">The configuration key to save.</param>
    /// <param name="value">The value to save.</param>
    /// <exception cref="InvalidOperationException">Thrown when user settings are not loaded.</exception>
    public void SaveUserValue(string key, object value)
    {
        if (userSettings is null)
        {
            throw new InvalidOperationException("User settings not loaded");
        }
        userSettings.GetSection(key).Value = value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Deletes a user-specific setting by setting its value to an empty string.
    /// </summary>
    /// <param name="key">The configuration key to delete.</param>
    public void DeleteUserValue(string key)
    {
        SaveUserValue(key, string.Empty);
    }

    /// <summary>
    /// Gets an application-wide setting value from appsettings.json.
    /// </summary>
    /// <typeparam name="T">The type to convert the setting value to.</typeparam>
    /// <param name="key">The configuration key to retrieve.</param>
    /// <returns>The setting value, or default if not found.</returns>
    public T? GetAppValue<T>(string key)
    {
        return GetValue<T>(userSettings, key);
    }


    /// <summary>
    /// Converts an array of integers to a comma-separated string.
    /// </summary>
    /// <param name="values">The integer values to convert.</param>
    /// <returns>A comma-separated string of the values.</returns>
    public static string GetCsv(int[] values) => string.Join(",", values);

    /// <summary>
    /// Parses a comma-separated string into an array of integers.
    /// </summary>
    /// <param name="csv">The comma-separated string to parse.</param>
    /// <returns>An array of integer values.</returns>
    public static int[] GetInts(string csv) => csv.Split(',').Select(int.Parse).ToArray();

    /// <summary>
    /// Parses a comma-separated string into an array of doubles.
    /// </summary>
    /// <param name="csv">The comma-separated string to parse.</param>
    /// <returns>An array of double values.</returns>
    public static double[] GetDoubles(string csv) => csv.Split(',').Select(double.Parse).ToArray();

    /// <summary>
    /// Parses a comma-separated string into an array of floats.
    /// </summary>
    /// <param name="csv">The comma-separated string to parse.</param>
    /// <returns>An array of float values.</returns>
    public static float[] GetFloats(string csv) => csv.Split(',').Select(float.Parse).ToArray();

    /// <summary>
    /// Parses a comma-separated string into an array of strings.
    /// </summary>
    /// <param name="csv">The comma-separated string to parse.</param>
    /// <returns>An array of string values.</returns>
    public static string[] GetStrings(string csv) => [.. csv.Split(',')];
}
