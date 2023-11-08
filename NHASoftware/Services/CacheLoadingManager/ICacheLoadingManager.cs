namespace NHA.Website.Software.Services.CacheLoadingManager
{
    public interface ICacheLoadingManager
    {
        /// <summary>
        /// Increments cacheChangeCounter. Uses default cacheLoadingManagerOptions to reload after 1 change. 
        /// </summary>
        /// <param name="cacheKey">cacheKey to increment</param>
        /// <param name="incrementAmount">amount to increment cacheChangeCounter by</param>
        public void IncrementCacheChangeCounter(string cacheKey, int incrementAmount = 1);

        /// <summary>
        /// Increments cache change counter. Sets up the cacheLoadingManagerOptions if the cache doesn't already exists. 
        /// </summary>
        /// <param name="cacheKey">cacheKey to increment</param>
        /// <param name="suppliedCacheReloadOptions">cacheLoadingManagerOptions determines when reload should happen</param>
        /// <param name="incrementAmount">amount to increment the cacheChangeCounter by</param>
        public void IncrementCacheChangeCounter(string cacheKey, CacheLoadingManagerOptions suppliedCacheReloadOptions, int incrementAmount);

        /// <summary>
        /// Checks whether the specified cache needs to be reloaded. Meaning counter > minimumReloadValue
        /// </summary>
        /// <param name="cacheKey">string cacheKey to check</param>
        /// <returns>bool whether cache needs to be reloaded or not</returns>
        public bool ShouldCacheReload(string cacheKey);
    }
}
