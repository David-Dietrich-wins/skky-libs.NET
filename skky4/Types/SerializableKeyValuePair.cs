using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.ObjectModel;

namespace skky.Types
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SerializableKeyValuePair<TKey, TValue>
    {
        private TKey key;
        private TValue value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public override string ToString()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append('[');

            if (this.Key != null)
            {
                builder1.Append(this.Key.ToString());
            }
            builder1.Append(", ");
            if (this.Value != null)
            {
                builder1.Append(this.Value.ToString());
            }
            builder1.Append(']');

            return builder1.ToString();
        }

        public TValue Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public TKey Key
        {
            get { return this.key; }
            set { this.key = value; }
        }
    }

    [Serializable()]
    public class KeyValuePairCollection<TKey, TValue> : Collection<SerializableKeyValuePair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            this.Add(new SerializableKeyValuePair<TKey, TValue>(key, value));
        }
    }

    [Serializable()]
    public class KeyedKeyValuePairCollection<TKey, TValue> : KeyedCollection<TKey, SerializableKeyValuePair<TKey, TValue>>
    {
        protected override TKey GetKeyForItem(SerializableKeyValuePair<TKey, TValue> item)
        {
            return item.Key;
        }

        public void Add(TKey key, TValue value)
        {
            this.Add(new SerializableKeyValuePair<TKey, TValue>(key, value));
        }
    }
}

