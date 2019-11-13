using LitJson;
using Reactor.API.Configuration;

namespace Reactor.API
{
    internal static class ModuleInitializer
    {
        public static void Initialize()
        {
            RegisterReactorSectionImporter();
            RegisterFloatExporter();
        }

        private static void RegisterReactorSectionImporter()
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

        private static void RegisterFloatExporter()
        {
            // Workaround for LitJSON's float exporting disabilities.
            // I still love you, even though you're retarded. :)

            JsonMapper.RegisterExporter<float>((input, writer) =>
            {
                var d = (double)input;
                writer.Write(d);
            });
        }
    }
}
