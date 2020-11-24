using System.Linq;
using System.Net.NetworkInformation;
using H.Core.Runners;

namespace HomeCenter.NET.Runners
{
    public class InternetRunner : Runner
    {
        #region Constructors

        public InternetRunner()
        {
            AddAction("check-internet", command => Print($"Internet is{(IsAvailableNetworkActive() ? "" : " not")} available"));

            AddVariable("$internet$", () => IsAvailableNetworkActive());
        }

        #endregion

        #region Private methods

        public static bool IsAvailableNetworkActive()
        {
            return NetworkInterface.GetIsNetworkAvailable() &&
                   (from face in NetworkInterface.GetAllNetworkInterfaces()
                       where face.OperationalStatus == OperationalStatus.Up
                       where (face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) && (face.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                       select face.GetIPv4Statistics()).Any(statistics => (statistics.BytesReceived > 0) && (statistics.BytesSent > 0));
        }

        #endregion
    }
}
