using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TejiLib {

    public class Information {

        /// <summary>
        /// the visual version
        /// </summary>
        public static string Version {
            get { return "v0.0.1 build 0001"; }
        }

        /// <summary>
        /// the version determine the transport
        /// </summary>
        public static int TransportVersion { get { return 0; } }

        /// <summary>
        /// get the app's work path
        /// </summary>
        public static FilePathBuilder WorkPath {
            get {
                return new FilePathBuilder(Environment.CurrentDirectory, Information.OS);
            }
        }

        public static PlatformID OS {
            get {
                return Environment.OSVersion.Platform;
            }
        }

        /// <summary>
        /// universal encoding
        /// </summary>
        public static Encoding UniversalEncoding {
            get {
                return Encoding.UTF8;
            }
        }

        public static int TransportSign { get { return 6161; } }

        public static long NowUNIXStamp {
            get { return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000; }
        }

        /// <summary>
        /// get native iso 639-1 name
        /// </summary>
        public static string NativeISO6391Name { get { return System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName; } }

        ///// <summary>
        ///// get native country name
        ///// </summary>
        //public static string NativeCountryCode {
        //    get {
        //        var cache = System.Globalization.CultureInfo.CurrentCulture.Name.Split('-');
        //        return cache[cache.Length - 1];
        //    }
        //}

    }

}
