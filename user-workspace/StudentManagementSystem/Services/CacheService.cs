using Microsoft.Extensions.Caching.Memory;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expirationTime = null);
        void Remove(string key);
        bool TryGetValue<T>(string key, out T? value);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null);
        void RemoveByPattern(string pattern);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly MemoryCacheEntryOptions _defaultCacheOptions;

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _defaultCacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
        }

        public T? Get<T>(string key)
        {
            try
            {
                return _cache.Get<T>(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving value from cache for key: {Key}", key);
                return default;
            }
        }

        public void Set<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions();

                if (expirationTime.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expirationTime;
                    options.SlidingExpiration = TimeSpan.FromMinutes(Math.Min(expirationTime.Value.TotalMinutes / 2, 30));
                }
                else
                {
                    options = _defaultCacheOptions;
                }

                options.RegisterPostEvictionCallback(OnPostEviction);

                _cache.Set(key, value, options);
                _logger.LogDebug("Value cached successfully for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
            }
        }

        public void Remove(string key)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogDebug("Cache entry removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing value from cache for key: {Key}", key);
            }
        }

        public bool TryGetValue<T>(string key, out T? value)
        {
            try
            {
                return _cache.TryGetValue(key, out value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to get value from cache for key: {Key}", key);
                value = default;
                return false;
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null)
        {
            try
            {
                return await _cache.GetOrCreateAsync(key, async entry =>
                {
                    if (expirationTime.HasValue)
                    {
                        entry.AbsoluteExpirationRelativeToNow = expirationTime;
                        entry.SlidingExpiration = TimeSpan.FromMinutes(Math.Min(expirationTime.Value.TotalMinutes / 2, 30));
                    }
                    else
                    {
                        entry.AbsoluteExpirationRelativeToNow = _defaultCacheOptions.AbsoluteExpirationRelativeToNow;
                        entry.SlidingExpiration = _defaultCacheOptions.SlidingExpiration;
                    }

                    entry.RegisterPostEvictionCallback(OnPostEviction);

                    var value = await factory();
                    _logger.LogDebug("Value created and cached for key: {Key}", key);
                    return value;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or creating cache entry for key: {Key}", key);
                throw;
            }
        }

        public void RemoveByPattern(string pattern)
        {
            try
            {
                var field = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var collection = field?.GetValue(_cache) as dynamic;

                var keys = new List<string>();
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        var methodInfo = item.GetType().GetProperty("Key");
                        var key = methodInfo?.GetValue(item) as string;
                        if (key != null && key.Contains(pattern))
                        {
                            keys.Add(key);
                        }
                    }
                }

                foreach (var key in keys)
                {
                    Remove(key);
                }

                _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", keys.Count, pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache entries by pattern: {Pattern}", pattern);
            }
        }

        private void OnPostEviction(object key, object? value, EvictionReason reason, object? state)
        {
            _logger.LogDebug(
                "Cache entry {Key} was evicted. Reason: {Reason}",
                key, reason);
        }

        // Helper methods for generating cache keys
        public static class Keys
        {
            public static string Student(string studentId) => $"student:{studentId}";
            public static string Course(int courseId) => $"course:{courseId}";
            public static string CourseList() => "courses:all";
            public static string StudentEnrollments(string studentId) => $"enrollments:student:{studentId}";
            public static string CourseEnrollments(int courseId) => $"enrollments:course:{courseId}";
            public static string StudentPayments(string studentId) => $"payments:student:{studentId}";
            public static string CourseSchedule(int courseId) => $"schedule:course:{courseId}";
            public static string CourseStatistics(int courseId) => $"stats:course:{courseId}";
            public static string DashboardInfo(string studentId) => $"dashboard:student:{studentId}";
        }
    }
}