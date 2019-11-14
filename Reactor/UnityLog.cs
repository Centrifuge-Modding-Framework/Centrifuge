using Reactor.API.Logging;

namespace Reactor
{
    public class UnityLog
    {
        private Log Log => LogManager.GetForInternalAssembly();

        public void LogUnityEngineMessage(string condition, string stackTrace, int logType)
        {
            if (!Global.InterceptUnityLogs)
                return;

            var msg = $"{condition}";

            if (!string.IsNullOrEmpty(stackTrace))
            {
                msg += $"\n{stackTrace}";
            }

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
