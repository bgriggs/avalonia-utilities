using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BigMission.Avalonia.Utilities.Settings;

// https://stackoverflow.com/questions/57978535/save-changes-of-iconfigurationroot-sections-to-its-json-file-in-net-core-2-2/57990271#57990271
internal class WritableJsonConfigurationProvider(JsonConfigurationSource source) : JsonConfigurationProvider(source)
{
    public override void Set(string key, string? value)
    {
        base.Set(key, value);

        if (string.IsNullOrEmpty(Source.Path))
        {
            throw new InvalidOperationException("The configuration source must have a non-empty path.");
        }

        EnsureSettingsFile(Source.Path);

        var store = IsolatedStorageFile.GetUserStoreForAssembly();
        using IsolatedStorageFileStream isoReadStream = new(Source.Path, FileMode.Open, store);
        using StreamReader reader = new(isoReadStream);
        var json = reader.ReadToEnd();
        var jsonObj = JsonNode.Parse(json) ?? JsonNode.Parse("{ }")!;
        var result = SetValue(jsonObj.AsObject(), key, value);

        //dynamic jsonObj = JsonConvert.DeserializeObject(json) ?? new Dictionary<string, object>();
        //jsonObj[key] = value;
        //string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

        reader.Dispose();

        // Write back to file
        if (result)
        {
            using IsolatedStorageFileStream isoWriteStream = new(Source.Path, FileMode.Open, store);
            using StreamWriter writer = new(isoWriteStream);
            var jo = new JsonSerializerOptions(JsonSerializerDefaults.General) { WriteIndented = true };
            writer.Write(jsonObj.ToJsonString(jo));
        }
        else
        {
            throw new InvalidOperationException("Failed to set value. If this is a nested setting, ensure it is an open node without a value.");
        }
    }

    public static void EnsureSettingsFile(string name)
    {
        var store = IsolatedStorageFile.GetUserStoreForAssembly();
        if (!store.FileExists(name))
        {
            using IsolatedStorageFileStream isoCreateStream = new(name, FileMode.CreateNew, store);
            using StreamWriter create = new(isoCreateStream);
            create.Write("{ }");
        }
    }

    public override void Load()
    {
        IFileInfo? file = Source.FileProvider?.GetFileInfo(Source.Path ?? string.Empty);
        if (file == null || !file.Exists)
        {
            if (Source.Optional) // Always optional on reload
            {
                Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                throw new InvalidOperationException($"Failed to open settings file {Source.Path}");
            }
        }
        else
        {
            static Stream OpenRead(IFileInfo fileInfo)
            {
                var store = IsolatedStorageFile.GetUserStoreForAssembly();
                return new IsolatedStorageFileStream(fileInfo.Name, FileMode.Open, store);
            }

            using Stream stream = OpenRead(file);
            try
            {
                Load(stream);
            }
            catch
            {

            }
        }
        // REVIEW: Should we raise this in the base as well / instead?
        OnReload();
    }

    private static bool SetValue(JsonObject obj, string key, JsonNode? value, string[]? delimiters = null)
    {
        var retVal = false;

        if (obj is not null)
        {
            // These are the default delimiters supported by the .Net Configuration
            delimiters ??= [":", "__"];

            var keyParts = key.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            JsonObject contextObj = obj;

            for (var i = 0; i < keyParts.Length - 1; i++)
            {
                try
                {
                    JsonNode? nextContextObj = contextObj[keyParts[i]];
                    nextContextObj ??= contextObj[keyParts[i]] = new JsonObject();
                    contextObj = nextContextObj.AsObject();
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }

            if (contextObj is not null)
            {
                contextObj[keyParts[^1]] = value;
                retVal = true;
            }
        }

        return retVal;
    }
}