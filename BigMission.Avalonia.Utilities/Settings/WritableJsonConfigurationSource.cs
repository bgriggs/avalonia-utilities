using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BigMission.Avalonia.Utilities.Settings;

// https://stackoverflow.com/questions/57978535/save-changes-of-iconfigurationroot-sections-to-its-json-file-in-net-core-2-2/57990271#57990271
/// <summary>
/// Configuration source for writable JSON configuration files.
/// Extends <see cref="JsonConfigurationSource"/> to support writing configuration changes back to the JSON file.
/// </summary>
public class WritableJsonConfigurationSource : JsonConfigurationSource
{
    /// <summary>
    /// Builds the configuration provider for this source.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <returns>A <see cref="WritableJsonConfigurationProvider"/> instance.</returns>
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new WritableJsonConfigurationProvider(this);
    }
}
