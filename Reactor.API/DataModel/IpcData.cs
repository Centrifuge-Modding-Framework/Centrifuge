using System;
using System.Collections.Generic;

namespace Reactor.API.DataModel
{
    public class IpcData : Dictionary<string, object>
    {
        public new object this[string key]
        {
            get
            {
                if (ContainsKey(key))
                    return base[key];

                return null;
            }

            set
            {
                if (!ContainsKey(key))
                    Add(key, value);
                else
                    base[key] = value;
            }
        }

        public string SourceIdentifier { get; }

        public IpcData(string sourceIdentifier)
        {
            SourceIdentifier = sourceIdentifier;
        }

        public T Get<T>(string key)
        {
            try
            {
                return (T)Convert.ChangeType(base[key], typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
