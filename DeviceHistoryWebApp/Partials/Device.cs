using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceHistoryWebApp
{
    public partial class Device
    {
        public static int NextAvailableId
        {
            get
            {
                using (DeviceHistoryEntities db = new DeviceHistoryEntities())
                {
                    if (db.Devices.Count() <= 0) return 1;
                    return db.Devices.ToList().OrderBy(u => u.Id).Last().Id + 1;
                }
            }
        }

        public String JsonData
        {
            get
            {
                return String.Format("{{0}" +
                    "\t\"Id\":\"{1}\",{0}" +
                    "\t\"Uid\":\"{2}\",{0}" +
                    "\t\"SerialNo\":\"{3}\",{0}" +
                    "\t\"TypeId\":\"{4}\"{0}" +
                    "}", Environment.NewLine, this.Id, this.Uid, this.SerialNo, this.TypeId);
            }
        }

        public static String GetJsonData(List<Device> list)
        {
            String result = "[ ";
            for(int i = 0; i < list.Count; i++)
            {
                result += list[i].JsonData;
                if (i + 1 < list.Count) result += ",";
            }

            result += "]";

            return result;
        }
    }
}