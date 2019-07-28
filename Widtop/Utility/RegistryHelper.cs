using Microsoft.Win32;

namespace Widtop.Utility
{
    public class RegistryHelper
    {
        public static void EnsureStartup()
        {
            var registryKey = Registry.CurrentUser
                .OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            registryKey?.SetValue("Widtop", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
