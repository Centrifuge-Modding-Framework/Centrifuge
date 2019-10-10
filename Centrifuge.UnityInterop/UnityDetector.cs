using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Centrifuge.UnityInterop
{
    public class UnityDetector
    {
        public static UnityVersion TryFindUnityVersion()
        {
            var mscorlibPath = Path.Combine(Assembly.GetExecutingAssembly().Location, FileNames.MsCorLib);
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(mscorlibPath);

            if (fileVersionInfo.FileMajorPart > 2)
                return UnityVersion.Modularized;

            return UnityVersion.Monolithic;
        }
    }
}
