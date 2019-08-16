using Harmony;
using System.Reflection;
using UnityEngine;

namespace Reactor.API.GTTOD
{
    public class GameAPI : MonoBehaviour
    {
        internal HarmonyInstance HarmonyInstance { get; private set; }
        public CommandTerminal.Terminal Terminal => Internal.Terminal.Reference;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeMixins();
        }

        private void InitializeMixins()
        {
            HarmonyInstance = HarmonyInstance.Create(Defaults.ReactorGameApiNamespace);
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
