using SharpShell.Attributes;
using SharpShell.SharpDeskBand;
using System;
using System.Runtime.InteropServices;

namespace SearchDeskBand
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

        [ComRegisterFunction]
        public static void RegisterClass(Type type) => ComUtilities.RegisterDeskBandClass(type);

        [ComUnregisterFunction]
        public static void UnregisterClass(Type type) => ComUtilities.UnregisterClass(type);
    }
}
