using System;
using System.Collections.Generic;

namespace Reactor.API.DataModel
{
    public class Message
    {
        private readonly Dictionary<string, object> _data;

        public object this[string key]
        {
            get
            {
                if (_data.ContainsKey(key))
                    return _data[key];

                return null;
            }

            set
            {
                if (!_data.ContainsKey(key))
                    _data.Add(key, value);
                else
                    _data[key] = value;
            }
        }

        public string SourceModID { get; }
        public string TargetModID { get; }
        public string Name { get; }

        public Message(string sourceModId, string targetModId, string name)
        {
            _data = new Dictionary<string, object>();

            SourceModID = sourceModId;
            TargetModID = targetModId;

            Name = name;
        }

        public bool Has(string key)
            => _data.ContainsKey(key);

        public T Get<T>(string key)
        {
            try
            {
                return (T)Convert.ChangeType(_data[key], typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
