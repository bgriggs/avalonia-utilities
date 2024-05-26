namespace BigMission.Avalonia.Utilities.Settings;

public interface ISettingsProvider
{
    T? GetUserValue<T>(string key);
    void SaveUserValue(string key, object value);
    void DeleteUserValue(string key);
    T? GetAppValue<T>(string key);
}
