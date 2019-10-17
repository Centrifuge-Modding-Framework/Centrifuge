using System;

namespace Reactor.API.Input
{
    public class Keyboard
    {
        public static bool IsKeyPressed(string key)
        {
            throw new NotImplementedException("Reimplementation in progress.");

            try
            {
                // return UnityEngine.Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            catch
            {
                return false;
            }
        }

        public static bool IsKeyDown(string key)
        {
            throw new NotImplementedException("Reimplementation in progress.");

            try
            {
                // return UnityEngine.Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            catch
            {
                return false;
            }
        }

        public static bool IsKeyUp(string key)
        {
            throw new NotImplementedException("Reimplementation in progress.");

            try
            {
               // return UnityEngine.Input.GetKeyUp((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            catch
            {
                return false;
            }
        }
    }
}
