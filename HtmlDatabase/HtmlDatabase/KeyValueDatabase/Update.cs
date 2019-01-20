using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static ArrayExtentions.ArrayExtentions;
using Monsajem_incs;

namespace KeyValueDatabase
{
    public partial class Table<ValueType>
    {
        private partial class TableExtras
        {
            public Action<string, ValueType> Updating;
            public Action<string, ValueType> Updated;
            public Action<string, string> KeyChanging;
            public Action<string, string> KeyChanged;
        }

        public event Action<string, ValueType> Updating
        {
            add => Extras.Updating += value;
            remove => Extras.Updating -= value;
        }
        public event Action<string, ValueType> Updated
        {
            add => Extras.Updated += value;
            remove => Extras.Updated -= value;
        }
        public event Action<string, string> KeyChanged
        {
            add => Extras.KeyChanged += value;
            remove => Extras.KeyChanged -= value;
        }
        public event Action<string, string> KeyChanging
        {
            add => Extras.KeyChanging += value;
            remove => Extras.KeyChanging -= value;
        }

        public void Update(string OldKey, ValueType NewValue)
        {
            Extras.Inner_loading?.Invoke(NewValue);
            var NewKey = Extras.GetKey(NewValue);
            Extras.Updating?.Invoke(OldKey, NewValue);
            if (OldKey != NewKey)
            {
                var Position = BinaryInsert(ref Keys, NewKey);
                Extras.Inner_Saving?.Invoke(NewValue);
                Extras.Insert(NewValue.Serialize(),Extras.PreName+"_" + NewKey);
                Extras.Inner_loading?.Invoke(NewValue);
                Extras.Delete(Extras.PreName + "_" + OldKey);
                BinaryDelete(ref Keys, OldKey);
                Extras.KeyChanged?.Invoke(OldKey, NewKey);
                Extras.Updated?.Invoke(OldKey, NewValue);
            }
            else
            {
                Extras.Inner_Saving?.Invoke(NewValue);
                Extras.Insert(NewValue.Serialize(), Extras.PreName + "_" + NewKey);
                Extras.Inner_loading?.Invoke(NewValue);
                Extras.Updated?.Invoke(OldKey, NewValue);
            }
        }

        public void Update(ValueType OldValue, ValueType NewValue)
        {
            Update(Extras.GetKey(OldValue), NewValue);
        }

        public void Update(string OldFileName, Action<ValueType> NewValueCreator)
        {
            var NewValue = GetItem(OldFileName);
            if (NewValue != null)
            {
                NewValueCreator(NewValue);
                Update(OldFileName, NewValue);
            }
        }
        public void Update(ValueType OldValue, Action<ValueType> NewValueCreator)
        {
            Update(Extras.GetKey(OldValue), NewValueCreator);
        }

        public void Update(Action<ValueType> NewValueCreator)
        {
            foreach (var OldValue in this.AsEnumerable())
                Update(OldValue, NewValueCreator);
        }

    }
}
