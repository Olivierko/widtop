// ReSharper disable InconsistentNaming

namespace Widtop.Logitech
{
    public class Discharge
    {
        public int Minutes { get; }
        public double Volts { get; }
        public double mWh { get; }

        public Discharge(int minutes, double volts, double mwh)
        {
            Minutes = minutes;
            Volts = volts;
            mWh = mwh;
        }
    }
}
