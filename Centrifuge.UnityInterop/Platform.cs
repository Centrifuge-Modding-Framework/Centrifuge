using System;

namespace Centrifuge.UnityInterop
{
    public static class Platform
    {
        public static bool IsMonoPlatform()
        {
            var platformID = (int)Environment.OSVersion.Platform;
            return platformID == 4 || platformID == 6 || platformID == 128;
        }

        public static bool IsUnix()
        {
            var platformID = Environment.OSVersion.Platform;
            switch (platformID)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return true;
                default:
                    return false;
            }
        }
    }
}