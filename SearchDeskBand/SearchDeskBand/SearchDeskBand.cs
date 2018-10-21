using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpDeskBand;

namespace H.NET.SearchDeskBand
{
    [ComVisible(true)]
    [DisplayName("Home Center Search")]
    [Guid("AE9E11C0-E4FD-4F96-B9B6-66CC76C2B45D")]
    public class WebSearchDeskBand : SharpDeskBand
    {
        protected override System.Windows.Forms.UserControl CreateDeskBand()
        {
            return new DeskBandControl();
        }

        protected override BandOptions GetBandOptions()
        {
            return new BandOptions
                   {
                       HasVariableHeight = false,
                       IsSunken = false,
                       ShowTitle = true,
                       Title = "Home Center Search",
                       UseBackgroundColour = false,
                       AlwaysShowGripper = true
                   };
        }
    }
}
