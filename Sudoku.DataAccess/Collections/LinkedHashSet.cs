namespace Sudoku.DataAccess;

public class LinkedHashSet<T> : IEnumerable<T> where T : notnull
{
    private readonly LinkedList<T> _list = [];
    private readonly Dictionary<T, LinkedListNode<T>> _map = new();
    
    public bool Add(T item) {
        if (_map.ContainsKey(item))
            return false;
        
        var node = _list.AddLast(item);
        _map[item] = node;
        
        return true;
    }

    public bool Remove(T item) {
        if (!_map.TryGetValue(item, out var node))
            return false;

        _list.Remove(node);
        _map.Remove(item);

        return true;
    }
    
    public bool Contains(T item) => _map.ContainsKey(item);

    public void Clear() {
        _list.Clear();
        _map.Clear();
    }
    
    public int Count => _map.Count;
    
    public List<T> ToList() {
        return [.._list];
    }
    
    public T GetLastSelected()
    {
        if (_list.Count == 0)
            throw new InvalidOperationException("The set is empty.");

        return _list.Last.Value;
    }

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}