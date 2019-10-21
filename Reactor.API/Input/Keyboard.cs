using Centrifuge.UnityInterop.Bridges;

namespace Reactor.API.Input
{
    public class Keyboard
    {
        public static bool IsKeyPressed(string key)
        {
            try
            {
                return InputBridge.IsKeyPressed(key);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsKeyDown(string key)
        {
            try
            {
                return InputBridge.IsKeyDown(key);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsKeyUp(string key)
        {
            try
            {
                return IsKeyUp(key);
            }
            catch
            {
                return false;
            }
        }
    }
}
