using System.Linq;
using System.Management;

namespace Widtop.Utility
{
    public static class Display
    {
        public static bool TryGetResolution(int index, out int x, out int y)
        {
            x = 0;
            y = 0;

            var query = new ManagementObjectSearcher("SELECT CurrentHorizontalResolution, CurrentVerticalResolution FROM Win32_VideoController");
            var collection = query.Get();

            var record = collection.OfType<ManagementObject>().ElementAtOrDefault(index);

            if (record == null)
            {
                return false;
            }

            var ox = record["CurrentHorizontalResolution"];
            var oy = record["CurrentVerticalResolution"];

            if (!(ox is uint) || !(oy is uint))
            {
                return false;
            }

            x = int.Parse(ox.ToString());
            y = int.Parse(oy.ToString());

            return true;
        }
    }
}
