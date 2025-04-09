using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    namespace Auto_parts_store
    {
        public static class Entities
        {
            private static AutoPartsStoreEntities context;

            public static AutoPartsStoreEntities GetContext()
            {
                if (context == null)
                    context = new AutoPartsStoreEntities();
                return context;
            }
        }
    }


