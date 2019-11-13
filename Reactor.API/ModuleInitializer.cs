using LitJson;
using Reactor.API.Configuration;

namespace Reactor.API
{
    public static class ModuleInitializer
    {
        public static void Initialize()
        {
            JsonMapper.RegisterImporter<object, Section>((o) =>
            {
                var sec = new Section();

                if (o is JsonData jsonData)
                {
                    if (jsonData.IsObject)
                    {
                        foreach (var k in jsonData.Keys)
                        {
                            sec[k] = jsonData[k];
                        }
                    }
                }

                return sec;
            });
        }
    }
}
