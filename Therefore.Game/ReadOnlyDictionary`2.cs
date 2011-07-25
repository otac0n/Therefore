namespace Therefore.Game
{
    using System;
    using System.Collections.Generic;

    internal class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> storage;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            this.storage = dictionary;
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        public bool ContainsKey(TKey key)
        {
            return this.storage.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return this.storage.Keys; }
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.storage.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return this.storage.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.storage[key];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.storage.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.storage.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.storage.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.storage.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.storage.GetEnumerator();
        }
    }
}
