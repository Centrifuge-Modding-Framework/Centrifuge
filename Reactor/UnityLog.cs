using Reactor.API.Logging;
using System.Linq;
using System.Text;

namespace Reactor
{
    public class UnityLog
    {
        private Log Log { get; } = LogManager.GetForInternalAssembly();

        private bool Enabled { get; } = Manager.Settings.GetItem<bool>(Resources.InterceptUnityLogsSettingsKey);

        public void LogUnityEngineMessage(string condition, string stackTrace, int logType)
        {
            if (!Enabled)
                return;

            var sb = new StringBuilder().Append(condition);

            if (!string.IsNullOrEmpty(stackTrace))
            {
                var lines = stackTrace.Split('\n')
                                      .Select(s => s.Trim())
                                      .Where(s => !string.IsNullOrEmpty(s));

                foreach (var line in lines)
                {
                    sb.AppendLine($"  {line}");
                }
            }

            var msg = sb.ToString();

            switch (logType)
            {
                case 0: // LogType.Error
                case 1: // LogType.Assert
                case 4: // LogType.Exception
                    Log.Error(msg);
                    break;

                case 2:
                    Log.Warning(msg);
                    break;

                case 3:
                    Log.Info(msg);
                    break;
            }
        }
    }
}
