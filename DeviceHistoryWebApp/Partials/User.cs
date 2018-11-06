using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceHistoryWebApp
{
    public partial class User
    {
        public static int NextAvailableId
        {
            get
            {
                using (DeviceHistoryEntities db = new DeviceHistoryEntities())
                {
                    if (db.Users.Count() <= 0) return 1;
                    return db.Users.ToList().OrderBy(u => u.Id).Last().Id + 1;
                }
            }
        }
    }
}