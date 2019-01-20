using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyValueDatabase
{
   
    public static class Extentions
    {
        public static void Update<ValueType>(
                this IFactualyData<ValueType> Data,
                Action<ValueType> CreateNewValue)
        {
            Data.Parent.Update((ValueType)Data, CreateNewValue);
        }

        public static void Update<ValueType>(
            this IEnumerable<IFactualyData<ValueType>> Table,
            Action<ValueType> CreateNewValue)
        {
            foreach (IFactualyData<ValueType> Data in Table)
            {
                Data.Parent.Update((ValueType)Data, CreateNewValue);
            }
        }

        public static void Update<ValueType>
            (this Table<ValueType> Table,
            Action<ValueType> CreateOldValue,
            ValueType NewValue)
            where ValueType : new()
        {
            var OldValue = new ValueType();
            CreateOldValue(OldValue);
            Table.Update(OldValue, NewValue);
        }

        public static void Update<ValueType>
            (this Table<ValueType> Table,
            Action<ValueType> CreateOldValue,
            Action<ValueType> CreateNewValue)
            where ValueType : new()
        {
            var OldValue = new ValueType();
            CreateOldValue(OldValue);
            Table.Update(OldValue, CreateNewValue);
        }

        public static bool IsExist<ValueType>
                (this Table<ValueType> Table,
                Action<ValueType> ValueAction)
                where ValueType : new()
        {
            var Value = new ValueType();
            ValueAction(Value);
            return Table.IsExist(Value);
        }

        public static void Insert<ValueType>
                (this Table<ValueType> Table,
                Action<ValueType> ValueAction)
                where ValueType : new()
        {
            var Value = new ValueType();
            ValueAction(Value);
            Table.Insert(Value);
        }

        public static ValueType GetItem<ValueType>
                (this Table<ValueType> Table,
                Action<ValueType> ValueAction)
                where ValueType : new()
        {
            var Value = new ValueType();
            ValueAction(Value);
            return Table.GetItem(Value);
        }

        public static ValueType GetOrInserItem<ValueType>
            (this Table<ValueType> Table,
            Action<ValueType> ValueAction)
            where ValueType : new()
        {
            var Value = new ValueType();
            ValueAction(Value);
            return Table.GetOrInserItem(Value);
        }

        public static void Delete<t>(
                this IFactualyData<t> Data)
        {
            Data.Parent.Delete((t)Data);
        }

        public static void Delete<t>(
            this IEnumerable<IFactualyData<t>> Table)
        {
            foreach (IFactualyData<t> Data in Table)
            {
                Data.Parent.Delete((t)Data);
            }
        }

        public static void Delete<ValueType>
            (this Table<ValueType> Table,
            Action<ValueType> ValueAction)
            where ValueType : new()
        {
            var Value = new ValueType();
            ValueAction(Value);
            Table.Delete(Value);
        }

        public static void Browse<t>(this IEnumerable<t> Table,Action<t> Browser)
        {
            foreach(t Value in Table)
            {
                Browser(Value);
            }
        }

        public class StructrureInfo<t,r>
        {
            public StructrureInfo(
                Func<StructrureInfo<t, r>, r> Function,
                IEnumerable<t> Data)
            {
                this.Function = Function;
                this.Data = Data;
            }

            public IEnumerable<t> Data;
            private Func<StructrureInfo<t, r>, r> Function;

            public r Repliy()
            {
                return Data.ToStructrure(Function);
            }
        }

        public static r ToStructrure<t,r>(this IEnumerable<t> Table,
            Func<StructrureInfo<t,r>,r> Function)
        {
           return Function(new StructrureInfo<t, r>(Function,Table));
        }
    }
}
