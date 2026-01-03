namespace Widgets.CustomExtensions;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        public void AddRange(IEnumerable<(TKey key, TValue value)> kvTuples)
        {
            foreach (var (key, value) in kvTuples)
            {
                dictionary.Add(key, value);
            }
        }
    }
}