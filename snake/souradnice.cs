using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snake
{
    class souradnice
    {
        private static int sid;
        public int id { get; set; }
        public int sloupce { get; set; }
        public int radky { get; set; }

        public souradnice(int sloupce, int radky)
        {
            id = sid;
            sid++;
            this.sloupce = sloupce;
            this.radky = radky;
        }
    }
}
