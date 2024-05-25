using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BigMission.Avalonia.Utilities;

/// <summary>
/// Provides optimizations to observable collection for working with
/// large number of adds and removes. You can add/remove many items 
/// and only raise one update event.
/// </summary>
public class LargeObservableCollection<T> : ObservableCollection<T>
{
    /// <summary>
    /// Notification of collection changes on an item per item basis, which ensures each and
    /// every item being added or removed is tracked explicitly (i.e. not "Reset").
    /// </summary>
    public event EventHandler<LargeObservableCollectionChangesEventArgs<T>>? CollectionItemChanged;

    private bool startingBulkTrackingState;
    private bool TrackChanges
    {
        get { return CollectionItemChanged != null; }
    }

    private T[]? startingItems;

    private bool raiseUpdate = true;

    /// <summary>
    /// Disables events so that a large number of rows can be added without individual events.
    /// </summary>
    public void BeginBulkOperation()
    {
        raiseUpdate = false;

        if (TrackChanges)
        {
            startingItems = this.ToArray();
        }

        // Note the state in case event is subscribed to during the operation
        startingBulkTrackingState = TrackChanges;
    }

    /// <summary>
    /// Finishes the bulk and raises change event.
    /// </summary>
    public void EndBulkOperation(bool raiseUpdate = true)
    {
        this.raiseUpdate = raiseUpdate;

        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        OnCollectionChanged(args);

        // Reset if disabled
        this.raiseUpdate = true;

        if (!startingBulkTrackingState && TrackChanges)
        {
            throw new InvalidOperationException("Cannot subscribe to CollectionItemChanged during a bulk operation");
        }
    }

    public void AddRange(IEnumerable<T> items)
    {
        bool clearBulkOperation = false;
        if (!raiseUpdate)
        {
            clearBulkOperation = true;
            BeginBulkOperation();
        }
        try
        {
            foreach (var item in items)
                Add(item);
        }
        finally
        {
            if (clearBulkOperation)
            {
                EndBulkOperation();
            }
        }
    }

    /// <summary>
    /// Clears collection and adds all items.
    /// </summary>
    /// <param name="items"></param>
    public void SetRange(IEnumerable<T> items)
    {
        bool clearBulkOperation = false;
        if (!raiseUpdate)
        {
            clearBulkOperation = true;
            BeginBulkOperation();
        }
        try
        {
            Clear();
            foreach (var item in items)
                Add(item);
        }
        finally
        {
            if (clearBulkOperation)
            {
                EndBulkOperation();
            }
        }
    }

    protected override void ClearItems()
    {
        if (raiseUpdate && TrackChanges)
        {
            startingItems = this.ToArray();
        }

        base.ClearItems();
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (raiseUpdate)
        {
            base.OnCollectionChanged(e);
            CheckTrackingEvent(e);
        }
    }

    private void CheckTrackingEvent(NotifyCollectionChangedEventArgs notifyChangedArgs)
    {
        if (TrackChanges)
        {
            T[] added = [];
            T[] removed = [];

            // Build operation, add range, clear
            if (startingItems != null)
            {
                added = this.Except(startingItems).ToArray();
                removed = startingItems.Except(this).ToArray();
            }
            // Single operation - add, remove, replace
            else if (notifyChangedArgs.Action == NotifyCollectionChangedAction.Add ||
                notifyChangedArgs.Action == NotifyCollectionChangedAction.Remove ||
                notifyChangedArgs.Action == NotifyCollectionChangedAction.Replace)
            {
                if (notifyChangedArgs.NewItems != null)
                {
                    added = notifyChangedArgs.NewItems.Cast<T>().ToArray();
                }

                if (notifyChangedArgs.OldItems != null)
                {
                    removed = notifyChangedArgs.OldItems.Cast<T>().ToArray();
                }
            }

            var args = new LargeObservableCollectionChangesEventArgs<T>(added, removed);
            CollectionItemChanged?.Invoke(this, args);
        }

        startingItems = null;
    }
}

public class LargeObservableCollectionChangesEventArgs<T>(T[] added, T[] removed) : EventArgs
{
    /// <summary>
    /// Added items, never null;
    /// </summary>
    public T[] Added { get; private set; } = added ?? [];

    /// <summary>
    /// Removed items, never null;
    /// </summary>
    public T[] Removed { get; private set; } = removed ?? [];
}
