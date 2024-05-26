using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.IO.IsolatedStorage;

namespace BigMission.Avalonia.Utilities.Settings;

public class IsolatedFileProvider : IFileProvider
{
    public IsolatedFileProvider()
    {
        
    }
    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var store = IsolatedStorageFile.GetUserStoreForAssembly();
        if (!store.FileExists(subpath))
        {
            return NotFoundDirectoryContents.Singleton;
        }

        var contents = new IsoDirectoryContents(true);

        foreach (var dir in store.GetDirectoryNames())
        {
            contents.Add(new IsoFileInfo(dir, true));
        }

        foreach (var file in store.GetFileNames())
        {
            contents.Add(new IsoFileInfo(file, false));
        }

        return contents;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        return new IsoFileInfo(subpath, false);
    }

    public IChangeToken Watch(string filter)
    {
        return new IsoChangeToken();
    }
}

public class IsoDirectoryContents : IDirectoryContents
{
    private List<IsoFileInfo> _entries = [];

    public bool Exists { get; private set; }

    public IEnumerator<IFileInfo> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    public IsoDirectoryContents(bool exists)
    {
        Exists = exists;
    }

    public void Add(IsoFileInfo file)
    {
        _entries.Add(file);
    }
}

public class IsoFileInfo : IFileInfo
{
    public bool Exists => true;

    public long Length => 1;

    public string? PhysicalPath => Name;

    public string Name { get; private set; }

    public DateTimeOffset LastModified => default(DateTime);

    public bool IsDirectory { get; private set; }

    public Stream CreateReadStream()
    {
        throw new NotImplementedException();
    }

    public IsoFileInfo(string name, bool isDirectory)
    {
        Name = name;
        IsDirectory = isDirectory;
    }
}

public class IsoChangeToken : IChangeToken
{
    public bool HasChanged => false;

    public bool ActiveChangeCallbacks => false;

    public IDisposable RegisterChangeCallback(Action<object> callback, object? state)
    {
        return new EmptyDisposable();
    }
}

public class EmptyDisposable : IDisposable
{
    public void Dispose() { }
}
