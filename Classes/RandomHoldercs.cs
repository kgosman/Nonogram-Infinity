using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram_Infinity
{
    public class RandomHolder
    {
        private static Random _instance;

        public static Random Instance
        {
            get { return _instance ?? (_instance = new Random()); }
        }
    }
}
