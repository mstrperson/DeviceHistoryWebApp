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
    }
}