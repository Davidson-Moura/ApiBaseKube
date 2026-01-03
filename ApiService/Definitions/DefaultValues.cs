using System.Runtime.InteropServices;

namespace ApiService.Definitions
{
    public static class DefaultValues
    {
        internal const string DefaultHiddenPassword = "******";
        internal const int MaxEntityPerRequest = 10000;

        internal static OSPlatform OSPlatform { get; private set; }
        internal static void SetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                OSPlatform = OSPlatform.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                OSPlatform = OSPlatform.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) //Mac
                OSPlatform = OSPlatform.OSX;
        }
        internal static EnvironmentVariableTarget EnvironmentTarget { get => OSPlatform == OSPlatform.Windows ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process; }

        internal const string MGPort = "Application_MGPort";
        internal const string MGUrl = "Application_MGUrl";
        internal const string MGDBName = "Application_MGDBName";
        internal const string MGUser = "Application_MGUser";
        internal const string MGPWD = "Application_MGPWD";

        internal const string POSTEGRES_CONNECTION = "POSTEGRES_CONNECTION";
    }
}
