namespace NHASoftware.Services.CacheGoblin
{
    public interface ICacheGoblin<T>
    {
        void Add(T item);
        void Clear();
        bool Exists(T item);
    }
}