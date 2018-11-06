using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceHistoryWebApp
{
    public partial class HistoryEntry
    {
        public static int NextAvailableId
        {
            get
            {
                using (DeviceHistoryEntities db = new DeviceHistoryEntities())
                {
                    if (db.HistoryEntries.Count() <= 0) return 1;
                    return db.HistoryEntries.ToList().OrderBy(u => u.Id).Last().Id + 1;
                }
            }
        }
    }
}