using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSControl.Data
{
    class ConnectObject
    {
        public static SPKanbanEntities1 connect;

        public static SPKanbanEntities1 GetConnect()
        {
            if (connect == null)
            {
                connect = new SPKanbanEntities1();
            }
            return connect;
        }
    }
}
