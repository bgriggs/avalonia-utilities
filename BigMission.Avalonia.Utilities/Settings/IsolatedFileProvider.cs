using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.IO.IsolatedStorage;

namespace BigMission.Avalonia.Utilities.Settings;

/// <summary>
/// Provides file system access to isolated storage for configuration files.
/// Implements <see cref="IFileProvider"/> to enable configuration providers to access isolated storage.
/// </summary>
public class IsolatedFileProvider : IFileProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IsolatedFileProvider"/> class.
    /// </summary>
    public IsolatedFileProvider()
    {
        
    }

    /// <summary>
    /// Gets the directory contents at the specified subpath in isolated storage.
    /// </summary>
    /// <param name="subpath">The relative path to the directory.</param>
    /// <returns>The directory contents, or <see cref="NotFoundDirectoryContents.Singleton"/> if the path doesn't exist.</returns>
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

    /// <summary>
    /// Gets file information for the file at the specified path in isolated storage.
    /// </summary>
 /// <param name="subpath">The relative path to the file.</param>
    /// <returns>The file information.</returns>
    public IFileInfo GetFileInfo(string subpath)
    {
        return new IsoFileInfo(subpath, false);
    }

    /// <summary>
    /// Creates a change token for the specified filter. Currently returns a token that never triggers changes.
    /// </summary>
    /// <param name="filter">The filter pattern to watch.</param>
    /// <returns>A change token that never signals changes.</returns>
  public IChangeToken Watch(string filter)
    {
        return new IsoChangeToken();
    }
}

/// <summary>
/// Represents directory contents in isolated storage.
/// </summary>
public class IsoDirectoryContents : IDirectoryContents
{
    private List<IsoFileInfo> _entries = [];

    /// <summary>
    /// Gets a value indicating whether the directory exists.
    /// </summary>
    public bool Exists { get; private set; }

    /// <summary>
    /// Returns an enumerator that iterates through the directory contents.
    /// </summary>
    /// <returns>An enumerator for the file information entries.</returns>
    public IEnumerator<IFileInfo> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

  /// <summary>
    /// Initializes a new instance of the <see cref="IsoDirectoryContents"/> class.
    /// </summary>
    /// <param name="exists">A value indicating whether the directory exists.</param>
    public IsoDirectoryContents(bool exists)
    {
        Exists = exists;
    }

    /// <summary>
  /// Adds a file entry to the directory contents.
    /// </summary>
    /// <param name="file">The file information to add.</param>
    public void Add(IsoFileInfo file)
    {
        _entries.Add(file);
    }
}

/// <summary>
/// Represents file information for a file in isolated storage.
/// </summary>
public class IsoFileInfo : IFileInfo
{
    /// <summary>
    /// Gets a value indicating whether the file exists. Always returns true.
    /// </summary>
 public bool Exists => true;

/// <summary>
    /// Gets the length of the file in bytes. Always returns 1.
    /// </summary>
    public long Length => 1;

    /// <summary>
    /// Gets the physical path to the file.
    /// </summary>
    public string? PhysicalPath => Name;

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the last modified timestamp. Always returns the default DateTime.
    /// </summary>
    public DateTimeOffset LastModified => default(DateTime);

    /// <summary>
    /// Gets a value indicating whether this entry represents a directory.
    /// </summary>
    public bool IsDirectory { get; private set; }

    /// <summary>
    /// Creates a read stream for the file. Not implemented.
    /// </summary>
    /// <returns>A read stream.</returns>
    /// <exception cref="NotImplementedException">This method is not implemented.</exception>
    public Stream CreateReadStream()
  {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsoFileInfo"/> class.
    /// </summary>
    /// <param name="name">The name of the file or directory.</param>
  /// <param name="isDirectory">A value indicating whether this represents a directory.</param>
    public IsoFileInfo(string name, bool isDirectory)
{
    Name = name;
        IsDirectory = isDirectory;
    }
}

/// <summary>
/// Represents a change token for isolated storage that never signals changes.
/// </summary>
public class IsoChangeToken : IChangeToken
{
    /// <summary>
    /// Gets a value indicating whether a change has occurred. Always returns false.
    /// </summary>
    public bool HasChanged => false;

    /// <summary>
    /// Gets a value indicating whether this token actively raises callbacks. Always returns false.
    /// </summary>
    public bool ActiveChangeCallbacks => false;

    /// <summary>
  /// Registers a callback that will be invoked when the change token is triggered.
    /// </summary>
    /// <param name="callback">The callback to invoke.</param>
    /// <param name="state">The state to pass to the callback.</param>
    /// <returns>A disposable object used to unregister the callback.</returns>
    public IDisposable RegisterChangeCallback(Action<object> callback, object? state)
    {
   return new EmptyDisposable();
    }
}

/// <summary>
/// A disposable object that does nothing when disposed.
/// </summary>
public class EmptyDisposable : IDisposable
{
  /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() { }
}
