namespace BigMission.Avalonia.Utilities.Settings;

/// <summary>
/// Defines a contract for managing application and user-specific settings.
/// </summary>
public interface ISettingsProvider
{
    /// <summary>
    /// Gets a user-specific setting value.
    /// </summary>
    /// <typeparam name="T">The type to convert the setting value to.</typeparam>
    /// <param name="key">The configuration key to retrieve.</param>
    /// <returns>The setting value, or default if not found.</returns>
    T? GetUserValue<T>(string key);

    /// <summary>
    /// Saves a user-specific setting value.
    /// </summary>
    /// <param name="key">The configuration key to save.</param>
    /// <param name="value">The value to save.</param>
    void SaveUserValue(string key, object value);

    /// <summary>
    /// Deletes a user-specific setting.
    /// </summary>
    /// <param name="key">The configuration key to delete.</param>
    void DeleteUserValue(string key);

    /// <summary>
    /// Gets an application-wide setting value.
    /// </summary>
    /// <typeparam name="T">The type to convert the setting value to.</typeparam>
    /// <param name="key">The configuration key to retrieve.</param>
    /// <returns>The setting value, or default if not found.</returns>
    T? GetAppValue<T>(string key);
}
