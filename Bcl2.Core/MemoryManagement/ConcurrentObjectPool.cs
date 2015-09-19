using System.Collections.Concurrent;

namespace Bcl2.MemoryManagement
{
  public static class ConcurrentObjectPool<T>
    where T : class, new()
  {
    private static readonly ConcurrentBag<T> _pool = new ConcurrentBag<T>();

    public static int Count { get { return _pool.Count; } }

    public static T Get()
    {
      T obj;
      if (_pool.TryTake(out obj))
      {
        return obj;
      }
      return new T();
    }

    public static void Put(T obj)
    {
      _pool.Add(obj);
    }
  }
}
