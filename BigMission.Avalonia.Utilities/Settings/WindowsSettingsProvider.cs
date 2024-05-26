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

    public WindowsSettingsProvider()
    {
        userSettingsFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}-settings.json";

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

    public T? GetUserValue<T>(string key)
    {
        return GetValue<T>(userSettings, key);
    }

    public void SaveUserValue(string key, object value)
    {
        if (userSettings is null)
        {
            throw new InvalidOperationException("User settings not loaded");
        }
        userSettings.GetSection(key).Value = value.ToString() ?? string.Empty;
    }

    public void DeleteUserValue(string key)
    {
        SaveUserValue(key, string.Empty);
    }

    public T? GetAppValue<T>(string key)
    {
        return GetValue<T>(userSettings, key);
    }


    public static string GetCsv(int[] values) => string.Join(",", values);

    public static int[] GetInts(string csv) => csv.Split(',').Select(int.Parse).ToArray();
    public static double[] GetDoubles(string csv) => csv.Split(',').Select(double.Parse).ToArray();
    public static float[] GetFloats(string csv) => csv.Split(',').Select(float.Parse).ToArray();
    public static string[] GetStrings(string csv) => [.. csv.Split(',')];
}
