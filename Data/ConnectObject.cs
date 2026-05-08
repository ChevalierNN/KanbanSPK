using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSControl.Data
{
    class ConnectObject
    {
        public static SPKanbanEntities connect;

        public static SPKanbanEntities GetConnect()
        {
            if (connect == null)
            {
                connect = new SPKanbanEntities();
            }
            return connect;
        }
    }
}
