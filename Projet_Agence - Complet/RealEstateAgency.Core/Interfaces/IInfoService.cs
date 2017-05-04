using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.Interfaces
{
    public interface IInfoService
    {
        string DeviceId { get; }
        string PackageName { get; }
        string AppVersionName { get; }
        int AppVersionCode { get; }
        double DeviceScreenWidth { get; }
        double DeviceScreenHeight { get; }
    }
}


// Pour Android :
//    [assembly: Xamarin.Forms.Dependency(typeof(YourApp.Droid.Services.InfoService))]
//namespace YourApp.Droid.Services
//    {
//        public class InfoService : IInfoService
//        {
//            public string DeviceId
//            {
//                get
//                {
//                    return Settings.Secure.GetString(Forms.Context.ContentResolver,
//                        Settings.Secure.AndroidId);
//                }
//            }

//            public string PackageName
//            {
//                get
//                {
//                    return Forms.Context.PackageName;
//                }
//            }



//            public string AppVersionName
//            {
//                get
//                {
//                    var context = Forms.Context;
//                    return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
//                }
//            }

//            public int AppVersionCode
//            {
//                get
//                {
//                    var context = Forms.Context;
//                    return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
//                }
//            }

//            public double DeviceScreenWidth
//            {
//                get
//                {
//                    var displayMetrics = Forms.Context.Resources.DisplayMetrics;
//                    return displayMetrics.WidthPixels / displayMetrics.Density;
//                }
//            }
//            public double DeviceScreenHeight
//            {
//                get
//                {
//                    var displayMetrics = Forms.Context.Resources.DisplayMetrics;
//                    return displayMetrics.HeightPixels / displayMetrics.Density;
//                }
//            }
//        }

