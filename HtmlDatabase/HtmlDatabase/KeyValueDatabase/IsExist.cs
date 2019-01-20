using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

namespace KeyValueDatabase
{
   
    public partial class Table<ValueType>
    {
        public bool IsExist(ValueType Value)
        {
            return IsExist(Extras.GetKey(Value));
        }

        public bool IsExist(string Key)
        {
            return Array.BinarySearch(Keys, Key) >= 0;
        }
    }
}
