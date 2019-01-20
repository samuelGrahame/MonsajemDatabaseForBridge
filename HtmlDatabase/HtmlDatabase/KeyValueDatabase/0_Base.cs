using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;
using static ArrayExtentions.ArrayExtentions;
using Monsajem_incs;

namespace KeyValueDatabase
{

    public interface UpdatableData
    {
        long UpdateCode { get; set; }
    }

    public interface IFactualyData<ValueType>
    {
        Table<ValueType> Parent { get; set; }
    }

    [Serializable]
    public partial class Table<ValueType>:
        IEnumerable<ValueType>
    {
        public string[] Keys;
        private TableExtras Extras;
        private bool IsChild;

        [Serializable]
        private partial class TableExtras
        {
            public Func<ValueType, string> GetKey;
            public Func<string,byte[]> GetItem;
            public Action<byte[], string> Insert;
            public Action<string> Delete;
            public string PreName;
            public string[] Accepteds = new string[0];
            public string[] Ignoreds = new string[0];
            public string OldKey;
        }

        public Table()
        { }

        public Table(
            Func<ValueType, string> GetKey,
            Func<string, byte[]> GetItem,
            Action<byte[], string> Insert,
            Action<string> Delete,
            string PreName="")
        {
            this.Extras = new TableExtras()
            {
                GetKey = GetKey,
                GetItem = GetItem,
                Insert = Insert,
                Delete = Delete,
                PreName = PreName
            };
            try
            {
                Keys =Extras.GetItem(Extras.PreName + "k").Deserialize<string[]>();
                if (Keys == null)
                    Keys = new string[0];
            }
            catch
            {
                Keys = new string[0];
            }

            if (typeof(ValueType).GetInterfaces().Where((c)=>
                c.DeclaringType == typeof(UpdatableData)).Count()>0)
            {
                this.Extras.Inserting += (NewValue) =>
                {
                    ((UpdatableData)NewValue).UpdateCode =
                                    DateTime.Now.Ticks;
                };
            }
        }

        private Table(string[] Keys, Table<ValueType> Parent)
        {
            this.Extras= new TableExtras()
            {
                GetKey = Parent.Extras.GetKey,
                GetItem = Parent.Extras.GetItem,
                Insert = Parent.Extras.Insert,
                Delete = Parent.Extras.Delete,
                PreName = Parent.Extras.PreName
            };
            this.Keys = Keys;
            this.IsChild = true;

            this.Inserting += (NewValue) =>{
                Parent.Accept(NewValue);
                Parent.Extras.Inserting?.Invoke(NewValue);
            };
            this.Inserted += (NewValue, NewPosition) =>
                Parent.Extras.Inserted?.Invoke(NewValue, NewPosition);

            this.Updating += (OldKey,NewValue) =>
                Parent.Extras.Updating?.Invoke(OldKey,NewValue);
            this.Updated += (OldKey, NewValue) =>
                Parent.Extras.Updated?.Invoke(OldKey, NewValue);

            this.KeyChanging += (OldKey, NewKey) =>{
                Parent.Extras.KeyChanging?.Invoke(OldKey, NewKey);
            };
            this.KeyChanged += (OldKey, NewKey) => {
                BinaryInsert(ref Parent.Keys, NewKey);
                BinaryDelete(ref Parent.Keys, OldKey);
                Parent.Extras.KeyChanged?.Invoke(OldKey, NewKey);
            };

            this.Deleting += (Key) =>
                Parent.Extras.Deleting?.Invoke(Key);
            this.Deleted += (Key, Position) =>
            {
                Parent.Ignore(Key);
                Parent.Extras.Deleted?.Invoke(Key, Position);
            };

            this.Extras.Inner_Saving += (Value) =>
                  Parent.Extras.Inner_Saving?.Invoke(Value);
            this.Extras.Inner_loading += (Value) =>
                  Parent.Extras.Inner_loading?.Invoke(Value);
            this.Extras.Inner_loading += (Value) =>
                  Parent.Extras.Inner_loading?.Invoke(Value);
        }

        public void SaveChange()
        {
            if(this.IsChild!= true)
                Extras.Insert(Keys.Serialize(),Extras.PreName + "k");
        }

        private IEnumerator<ValueType> AsEnumerator()
        {
            int Position = 0;
            Extras.Deleted += (string Key, int ps) =>
            {
                if (ps <= Position)
                    Position--;
            };
            Extras.Inserted += (ValueType Value, int ps) =>
            {
                if (ps <= Position)
                    Position++;
            };

            for (; Position < Keys.Length; Position++)
            {
                yield return GetItem(Position);
            }
        }

        public IEnumerator<ValueType> GetEnumerator()
        {
            return AsEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AsEnumerator();
        }
    }

}
