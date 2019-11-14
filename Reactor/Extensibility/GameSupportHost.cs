using System.Reflection;

namespace Reactor.Extensibility
{
    internal class GameSupportHost
    {
        public Assembly Assembly { get; set; }

        public string ID { get; set; }

        public object UnityGameObject { get; set; }
        public object ComponentInstance { get; set; }
    }
}
