using System;
using System.Collections.Generic;
using static ArrayExtentions.ArrayExtentions;

namespace KeyValueDatabase
{
    public partial class Table<ValueType>
    {
        private partial class TableExtras
        {
            public Action<string> Accepting;
            public Action<string, int> Accepted;
        }

        public event Action<string> Accepting
        {
            add => Extras.Accepting += value;
            remove => Extras.Accepting -= value;
        }
        public event Action<string, int> Accepted
        {
            add => Extras.Accepted += value;
            remove => Extras.Accepted -= value;
        }

        public int Accept(string Key)
        {
            //Extras.Accepting?.Invoke(Key);
            if (IsExist(Key))
                throw new InvalidOperationException("Value be exist!");
            var Position = BinaryInsert(ref Keys, Key);
            //Extras.Accepted?.Invoke(Key, Position);
            return Position*-1;
        }

        public void Accept(ValueType Value)
        {
            var Key = Extras.GetKey(Value);
            Accept(Key);
        }

        public void Accept(Table<ValueType> Values)
        {
            foreach (var Key in Values.Keys)
                Accept(Key);
        }

        public void Accept(IEnumerable<ValueType> Values)
        {
            foreach (var Value in Values)
                Accept(Value);
        }
        public void Accept(IEnumerable<string> Keys)
        {
            foreach (var Key in Keys)
                Accept(Key);
        }

    }
}
