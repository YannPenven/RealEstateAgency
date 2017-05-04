using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RealEstateAgency.App.Tools
{
    public class InfoService : Core.Interfaces.IInfoService
    {
        public int AppVersionCode
        {
            get;
        }

        public string AppVersionName
        {
            get;
        }

        public string DeviceId
        {
            get;
        }

        public double DeviceScreenHeight
        {
            get;
        }

        public double DeviceScreenWidth
        {
            get;
        }

        public string PackageName
        {
            get;
        }

        private InfoService(string appVersionName)
        {
            AppVersionName = appVersionName;
        }

        internal static InfoService Get()
        {
            return new InfoService(GetAppVersion());
        }
        private static string GetAppVersion()
        {

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

        }
    }
}
