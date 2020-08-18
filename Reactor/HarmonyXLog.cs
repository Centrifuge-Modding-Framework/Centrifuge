using HarmonyLib.Tools;
using Reactor.API.Logging;
using System.IO;
using System.Text;

namespace Reactor
{
    public class HarmonyXLog : TextWriter
    {
        private Log Log { get; } = LogManager.GetForCurrentAssembly();

        public bool IsEnabled => Manager.Settings.GetItem<bool>(Resources.InterceptHarmonyXLogsSettingsKey);
        public override Encoding Encoding => Encoding.UTF8;

        internal HarmonyXLog()
        {
            Logger.ChannelFilter = Logger.LogChannel.All;

            HarmonyFileLog.Writer = this;
            Logger.MessageReceived += Logger_MessageReceived;
        }

        private void Logger_MessageReceived(object sender, Logger.LogEventArgs e)
        {
            if (!IsEnabled)
                return;

            switch (e.LogChannel)
            {
                case Logger.LogChannel.Warn:
                    Log.Warning(e.Message);
                    break;
                case Logger.LogChannel.IL:
                    Log.Debug(e.Message);
                    break;
                case Logger.LogChannel.Error:
                    Log.Error(e.Message);
                    break;
                default:
                    Log.Info(e.Message);
                    break;
            }
        }
    }
}
