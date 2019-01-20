using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ArrayExtentions.ArrayExtentions;

namespace KeyValueDatabase
{
    public partial class Table<ValueType>
    {
        private Table<ValueType> Hidden
        {
            get {

                var Result = new Table<ValueType>()
                {
                    Extras = new TableExtras()
                    {
                        GetKey = Extras.GetKey,
                        GetItem = Extras.GetItem,
                        Insert = Extras.Insert,
                        Delete = Extras.Delete,
                        PreName = Extras.PreName
                    },
                    Keys = Keys
                };
                Result.Accepted += (Key, p) => BinaryInsert(ref Keys, Key);
                Result.Ignored+=(Key, p) => BinaryDelete(ref Keys, Key);
                return Result;
            }
        }
        public Table<ValueType> Copy()
        {
            return new Table<ValueType>(Keys,this);
        }
    }
}
