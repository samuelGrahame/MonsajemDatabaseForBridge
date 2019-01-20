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
            public Action<ValueType> Inner_loading;
        }


        public void FillRelations(ValueType Value)
        {
            Extras.Inner_loading?.Invoke(Value);
        }

        public ValueType GetItem(int Position)
        {
            return GetItem(Keys[Position]);
        }

        public ValueType GetItem(string Key)
        {
            var Item=Extras.GetItem(Extras.PreName + "_" +Key).Deserialize<ValueType>();
            Extras.Inner_loading?.Invoke(Item);
            return Item;
        }

        public Table<ValueType> GetItems(string[] Keys)
        {
            return new Table<ValueType>
                (Keys, this);
        }

        public Table<ValueType> GetElseItems(string[] Keys)
        {
           var Result = Copy();
            Result.Ignore(Keys);
            return Result;
        }

        public Table<ValueType> GetElseItems(Table<ValueType> Items)
        {
            var Result = Copy();
            Result.Ignore(Items);
            return Result;
        }

        public ValueType GetItem(ValueType Value)
        {
            return GetItem(Extras.GetKey(Value));
        }

        public ValueType GetOrInserItem(ValueType Value)
        {
            var FileName =Extras.GetKey(Value);
            if (IsExist(FileName))
            {
                return GetItem(FileName);
            }
            else
            {
                Insert(Value,FileName);
                return Value;
            }
        }

    }
}
