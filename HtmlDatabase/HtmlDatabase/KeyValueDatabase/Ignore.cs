using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArrayExtentions.ArrayExtentions;

namespace KeyValueDatabase
{
    public partial class Table<ValueType>
    {
        private partial class TableExtras
        {
            public Action<string> Ignoring;
            public Action<string, int> Ignored;
        }

        public event Action<string> Ignoring
        {
            add => Extras.Ignoring += value;
            remove => Extras.Ignoring -= value;
        }
        public event Action<string, int> Ignored
        {
            add => Extras.Ignored += value;
            remove => Extras.Ignored -= value;
        }

        public int Ignore(string Key)
        {
            Extras.Ignoring?.Invoke(Key);
            var Position = BinaryDelete(ref Keys, Key);
            Extras.Ignored?.Invoke(Key, Position);
            return Position;
        }

        public void Ignore(ValueType Value)
        {
            var Key = Extras.GetKey(Value);
            Ignore(Key);
        }

        public void Ignore(Table<ValueType> Values)
        {
            foreach (var Key in Values.Keys)
                Ignore(Key);
        }

        public void Ignore(IEnumerable<ValueType> Values)
        {
            foreach (var Value in Values)
                Ignore(Value);
        }
        public void Ignore(IEnumerable<string> Keys)
        {
            foreach (var Key in Keys)
                Ignore(Key);
        }

        public void Ignore()
        {
            foreach (var Key in Keys)
                Ignore(Key);
        }
    }
}
