using UnityEngine;
using GttodTerminal = CommandTerminal.Terminal;

namespace Reactor.API.GTTOD.Internal
{
    internal class Terminal
    {
        private static GttodTerminal _reference;
        public static GttodTerminal Reference
        {
            get
            {
                EnsureReferenceValid();
                return _reference;
            }
        }
        
        private static void EnsureReferenceValid()
        {
            _reference = GameObject.FindObjectOfType<GttodTerminal>();

            if (!_reference)
            {
                var termGameObject = new GameObject("com.github.ciastex/ReactorGameAPI.ReplacementGameTerminal");
                _reference = termGameObject.AddComponent<GttodTerminal>();
            }
        }
    }
}
