using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BigMission.Avalonia.Utilities.Settings;

// https://stackoverflow.com/questions/57978535/save-changes-of-iconfigurationroot-sections-to-its-json-file-in-net-core-2-2/57990271#57990271
public class WritableJsonConfigurationSource : JsonConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new WritableJsonConfigurationProvider(this);
    }
}
