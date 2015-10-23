using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Container
{
    public interface SharedDataListener<T>
    {
        void onDataReceived(T obj);
    }
}
