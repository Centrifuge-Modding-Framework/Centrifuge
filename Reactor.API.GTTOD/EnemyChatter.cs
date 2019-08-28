using System.Collections.Generic;
using System.Linq;

namespace Reactor.API.GTTOD
{
    public static class EnemyChatter
    {
        public static List<string> AttackMessages { get; private set; }
        public static List<string> AngryMessages { get; private set; }
        public static List<string> DeathMessages { get; private set; }
        public static List<string> EquipmentMessages { get; private set; }
        public static List<string> JokeMessages { get; private set; }
        public static List<string> ReactionMessages { get; private set; }
        public static List<string> ReloadMessages { get; private set; }

        static EnemyChatter()
        {
            AttackMessages = new List<string>();
            AngryMessages = new List<string>();
            DeathMessages = new List<string>();
            EquipmentMessages = new List<string>();
            JokeMessages = new List<string>();
            ReactionMessages = new List<string>();
            ReloadMessages = new List<string>();
        }

        internal static void InitializeAdditionalTalkerMessages(AITalker talker)
        {
            talker.AttackMessages.AddRange(
                AttackMessages.Select((message) => new TyperMessage { Text = message })
            );

            talker.AngryMessages.AddRange(
                AngryMessages.Select((message) => new TyperMessage { Text = message })
            );

            talker.DeathMessages.AddRange(
                DeathMessages.Select((message) => new TyperMessage { Text = message })
            );

            talker.EquipmentMessages.AddRange(
                EquipmentMessages.Select((message) => new TyperMessage { Text = message })
            );

            talker.JokeMessages.AddRange(
                JokeMessages.Select((message) => new TyperMessage { Text = message })
            );

            talker.ReactionMessages.AddRange(
                ReactionMessages.Select((message) => new TyperMessage { Text = message })
            );

            talker.ReloadMessages.AddRange(
                ReloadMessages.Select((message) => new TyperMessage { Text = message })
            );
        }
    }
}
