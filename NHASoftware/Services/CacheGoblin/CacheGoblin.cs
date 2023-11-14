namespace NHA.Website.Software.Services.CacheGoblin
{
    public class CacheGoblin<T> : ICacheGoblin<T>
    {
        public int cacheResetThreshold { get; set; } = 10000;
        public HashSet<T> cacheitems = new HashSet<T>();

        public CacheGoblin()
        {

        }

        public void Add(T item)
        {
            if (cacheitems.Count > 50000)
            {
                Clear();
            }

            if (!Exists(item))
            {
                cacheitems.Add(item);
            }
        }

        public void Clear()
        {
            cacheitems.Clear();
        }

        public bool Exists(T item)
        {
            foreach (var cItem in cacheitems)
            {
                if (cItem == null)
                {
                    return false;
                }

                if (cItem.Equals(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
