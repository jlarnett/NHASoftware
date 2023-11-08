namespace NHA.Website.Software.Services.CacheLoadingManager
{
    public class CacheLoadingManager : ICacheLoadingManager
    {
        private readonly Dictionary<string, CacheLoadingManagerOptions> _cacheLoadingOptions = new Dictionary<string, CacheLoadingManagerOptions>();
        private readonly Dictionary<string, CacheReloadingEntry> _cacheReloadEntries = new Dictionary<string, CacheReloadingEntry>();

        private readonly ILogger _logger;

        public CacheLoadingManager(ILogger<CacheLoadingManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Increments cacheChangeCounter. Uses default cacheLoadingManagerOptions to reload after 1 change. 
        /// </summary>
        /// <param name="cacheKey">cacheKey to increment</param>
        /// <param name="incrementAmount">amount to increment cacheChangeCounter by</param>
        public void IncrementCacheChangeCounter(string cacheKey, int incrementAmount = 1)
        {
            if (_cacheLoadingOptions.TryGetValue(cacheKey, out CacheLoadingManagerOptions? options))
            {
                if (!_cacheReloadEntries.ContainsKey(cacheKey))
                    _cacheReloadEntries.Add(cacheKey, new CacheReloadingEntry());
                else
                {
                    _cacheReloadEntries[cacheKey].CacheChangeCounter += incrementAmount;
                    if (_cacheReloadEntries[cacheKey].CacheChangeCounter >=
                        options.NumberOfModificationsBeforeCacheReload)
                        _cacheReloadEntries[cacheKey].ReloadCache = true;
                }
            }
            else
            {
                _cacheLoadingOptions.Add(cacheKey, new CacheLoadingManagerOptions());
            }
        }

        /// <summary>
        /// Increments cache change counter. Sets up the cacheLoadingManagerOptions if the cache doesn't already exists. 
        /// </summary>
        /// <param name="cacheKey">cacheKey to increment</param>
        /// <param name="suppliedCacheReloadOptions">cacheLoadingManagerOptions determines when reload should happen</param>
        /// <param name="incrementAmount">amount to increment the cacheChangeCounter by</param>
        public void IncrementCacheChangeCounter(string cacheKey, CacheLoadingManagerOptions suppliedCacheReloadOptions, int incrementAmount = 1)
        {
            if (_cacheLoadingOptions.TryGetValue(cacheKey, out CacheLoadingManagerOptions? options))
            {
                if (!_cacheReloadEntries.ContainsKey(cacheKey))
                    _cacheReloadEntries.Add(cacheKey, new CacheReloadingEntry());
                else
                {
                    _cacheReloadEntries[cacheKey].CacheChangeCounter += incrementAmount;
                    if (_cacheReloadEntries[cacheKey].CacheChangeCounter >=
                        options.NumberOfModificationsBeforeCacheReload)
                        _cacheReloadEntries[cacheKey].ReloadCache = true;
                }
            }
            else
            {
                _cacheLoadingOptions.Add(cacheKey, suppliedCacheReloadOptions);
            }
        }
        /// <summary>
        /// Checks whether the specified cache needs to be reloaded. Meaning counter > minimumReloadValue
        /// </summary>
        /// <param name="cacheKey">string cacheKey to check</param>
        /// <returns>bool whether cache needs to be reloaded or not</returns>
        public bool ShouldCacheReload(string cacheKey)
        {
            if (_cacheReloadEntries.ContainsKey(cacheKey))
            {
                var result = _cacheReloadEntries[cacheKey].ReloadCache;

                if (result)
                {
                    _cacheReloadEntries[cacheKey].ReloadCache = false;
                    return result;
                }

                return false;
            }

            _logger.LogError("CacheLoadingManager was unable to locate cacheKey when checking for reload. Adding key to dictionaries.");
            TryAddCacheReloadEntryToDictionary(cacheKey);
            return _cacheReloadEntries[cacheKey].ReloadCache;
        }

        private void TryAddCacheReloadEntryToDictionary(string cacheKey)
        {
            if (!_cacheLoadingOptions.ContainsKey(cacheKey))
            {
                _cacheLoadingOptions.Add(cacheKey, new CacheLoadingManagerOptions());
            }

            if (!_cacheReloadEntries.ContainsKey(cacheKey))
            {
                _cacheReloadEntries.Add(cacheKey, new CacheReloadingEntry());
            }
        }

        private class CacheReloadingEntry
        {
            public int CacheChangeCounter { get; set; } = 0;
            public bool ReloadCache { get; set; } = false;
        }

    }

    public class CacheLoadingManagerOptions
    {
        public int NumberOfModificationsBeforeCacheReload { get; set; } = 1;
    }


}
