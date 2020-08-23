using Reactor.API.Events;
using Reactor.API.Exceptions;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Reactor.API.Configuration
{
    public class Section : Dictionary<string, object>
    {
        public bool Dirty { get; protected set; }

        public new object this[string key]
        {
            get
            {
                if (!ContainsKey(key))
                    return null;

                return base[key];
            }

            set
            {
                if (!ContainsKey(key))
                {
                    Add(key, value);
                    ValueChanged?.Invoke(this, new SettingsChangedEventArgs(key, null, base[key]));
                }
                else
                {
                    var oldValue = base[key];

                    base[key] = value;
                    ValueChanged?.Invoke(this, new SettingsChangedEventArgs(key, oldValue, base[key]));
                }

                Dirty = true;
            }
        }

        public event EventHandler<SettingsChangedEventArgs> ValueChanged;

        public T GetOrCreate<T>(string key) where T : new()
        {
            if (!ContainsKey(key))
            {
                this[key] = new T();
                ValueChanged?.Invoke(this, new SettingsChangedEventArgs(key, null, this[key]));
            }

            return GetItem<T>(key);
        }

        public T GetOrCreate<T>(string key, T defaultValue)
        {
            if (!ContainsKey(key))
            {
                this[key] = defaultValue;
                ValueChanged?.Invoke(this, new SettingsChangedEventArgs(key, null, this[key]));
            }

            return GetItem<T>(key);
        }

        public T GetItem<T>(string key)
        {
            if (!ContainsKey(key))
                throw new KeyNotFoundException($"The key requested doesn't exist in store: '{key}'.");

            try
            {
                if (this[key] is JToken jToken)
                    return jToken.ToObject<T>();

                return (T)Convert.ChangeType(this[key], typeof(T));
            }
            catch (JsonException je)
            {
                throw new SettingsException("Failed to convert a JSON token to the requested type.", key, true, je);
            }
            catch (Exception e)
            {
                try
                {
                    return (T)Enum.ToObject(typeof(T), (long)this[key]);
                }
                catch
                {
                    throw new SettingsException($".NET type conversion exception has been thrown.", key, false, e);
                }
            }
        }

        public bool ContainsKey<T>(string key)
        {
            try
            {
                GetItem<T>(key);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}