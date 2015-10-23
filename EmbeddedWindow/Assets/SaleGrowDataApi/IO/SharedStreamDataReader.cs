using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace FileMap
{
    public class SharedStreamDataReader<T> : SharedDataReader<T>
    {
        public SharedStreamDataReader(string readingFileName)
            : base(readingFileName)
        {
        }

        protected int getSleepTimeOnNoData()
        {
            return 2000;
        }
    }
}
