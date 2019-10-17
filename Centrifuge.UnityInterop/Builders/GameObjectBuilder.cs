using System;
using System.Linq;

namespace Centrifuge.UnityInterop.Builders
{
    public class GameObjectBuilder
    {
        private static Type _gameObjectType;

        static GameObjectBuilder()
        {
        }

        private static Type FindTypeByName(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                      .Where(asm => asm.GetName().FullName.StartsWith("UnityEngine"));


        }
    }
}
