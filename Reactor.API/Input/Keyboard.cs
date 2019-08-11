using System;
using UnityEngine;

namespace Reactor.API.Input
{
    public class Keyboard
    {
        public static bool IsKeyPressed(string key)
        {
            try
            {
                return UnityEngine.Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), key));
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
                return UnityEngine.Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), key));
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
                return UnityEngine.Input.GetKeyUp((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            catch
            {
                return false;
            }
        }
    }
}
