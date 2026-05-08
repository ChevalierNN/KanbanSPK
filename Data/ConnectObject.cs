using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSControl.Data
{
    class ConnectObject
    {
        public static FTSKanbanEntities2 connect;

        public static FTSKanbanEntities2 GetConnect()
        {
            if (connect == null)
            {
                connect = new FTSKanbanEntities2();
            }
            return connect;
        }
    }
}
