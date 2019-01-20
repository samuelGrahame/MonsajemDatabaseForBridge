using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using static ArrayExtentions.ArrayExtentions;

namespace KeyValueDatabase
{
    public partial class Table<ValueType>
    {
        private partial class TableExtras
        {
            public Action<string> Deleting;
            public Action<string, int> Deleted;
        }

        public event Action<string> Deleting
        {
            add => Extras.Deleting += value;
            remove => Extras.Deleting -= value;
        }
        public event Action<string, int> Deleted
        {
            add => Extras.Deleted += value;
            remove => Extras.Deleted -= value;
        }

        public void Delete(string Key)
        {
            Extras.Deleting?.Invoke(Key);
            var Position = Ignore(Key);
            Extras.Delete(Extras.PreName + "_" + Key);
            Extras.Deleted?.Invoke(Key, Position);
        }

        public void Delete(ValueType Value)
        {
            var Key =Extras.GetKey(Value);
            Delete(Key);
        }

        public void Delete(Table<ValueType> Values)
        {
            foreach (var Key in Values.Keys)
                Delete(Key);
        }

        public void Delete(IEnumerable<ValueType> Values)
        {
            foreach (var Value in Values)
                Delete(Value);
        }

        public void Delete()
        {
            foreach (var Key in Keys)
                Delete(Key);
        }
    }
}
