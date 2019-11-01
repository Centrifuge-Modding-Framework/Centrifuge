using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Reactor.API.Input
{
    public class Hotkey
    {
        private List<string> Keys { get; }
        private bool WasFired { get; set; }
        private bool IsOneTime { get; } = true;

        public int Specificity => Keys.Count;

        public bool IsPressed
        {
            get
            {
                var pressed = false;
                foreach (var key in Keys)
                {
                    if (Keyboard.IsKeyPressed(key))
                    {
                        pressed = true;
                        continue;
                    }

                    pressed = false;
                    WasFired = false;
                    break;
                }

                if (pressed)
                {
                    if (!WasFired || !IsOneTime)
                    {
                        WasFired = true;
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// Creates a hotkey given a key combinaton.
        /// </summary>
        /// <param name="hotkeyString">Unity key names separated with '+'.</param>
        public Hotkey(string hotkeyString)
        {
            Keys = Parse(hotkeyString);
        }

        /// <summary>
        /// Creates a hotkey given a key combination.
        /// </summary>
        /// <param name="hotkeyString">Unity key names separated with '+'.</param>
        /// <param name="isOneTime">Whether to register the keypress every frame, or just once.</param>
        public Hotkey(string hotkeyString, bool isOneTime) : this(hotkeyString)
        {
            IsOneTime = isOneTime;
        }

        private List<string> Parse(string hotkey)
        {
            var list = new List<string>();

            string modifiedHotkeyString = hotkey;
            if (Regex.IsMatch(modifiedHotkeyString, @"^[A-z+]+"))
            {
                // e.g. X+
                if (modifiedHotkeyString.EndsWith("+"))
                {
                    // X+ == X
                    modifiedHotkeyString = modifiedHotkeyString.Substring(0, modifiedHotkeyString.Length - 1);
                }

                if (modifiedHotkeyString.Contains("+"))
                {
                    var split = hotkey.Split('+');

                    foreach (var s in split)
                        list.Add(s);
                }
                else
                {
                    list.Add(modifiedHotkeyString);
                }
            }

            var noDuplicates = new List<string>(new HashSet<string>(list));
            return noDuplicates;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var s in Keys)
                sb.Append($"{s}+");

            return sb.ToString().Trim('+');
        }
    }
}
