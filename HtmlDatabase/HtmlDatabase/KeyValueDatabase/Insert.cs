using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using static ArrayExtentions.ArrayExtentions;
using Monsajem_incs;

namespace KeyValueDatabase
{
    public partial class Table<ValueType>
    {
        private partial class TableExtras
        {
            public Action<ValueType> Inner_Saving;
            public Action<ValueType> Inserting;
            public Action<ValueType, int> Inserted;
        }

        public event Action<ValueType> Inserting
        {
            add => Extras.Inserting += value;
            remove => Extras.Inserting -= value;
        }
        public event Action<ValueType, int> Inserted
        {
            add => Extras.Inserted += value;
            remove => Extras.Inserted -= value;
        }

        public void Insert(ValueType Value)
        {
            Insert(Value, Extras.GetKey(Value));
        }

        private int Insert(ValueType Value,string Key)
        {
            Extras.Inner_loading?.Invoke(Value);
            Extras.Inserting?.Invoke(Value);
            Extras.Accepting?.Invoke(Key);
            if (IsExist(Key))
                throw new InvalidOperationException("Value be exist!");
            var Position = BinaryInsert(ref Keys, Key);
            Extras.Inner_Saving?.Invoke(Value);
            Extras.Insert(Value.Serialize(), Extras.PreName + "_" + Key);
            Extras.Inner_loading?.Invoke(Value);
            Extras.Accepted?.Invoke(Key, Position);
            Extras.Inserted?.Invoke(Value,Position);
            return Position*-1;
        }
    }
}
