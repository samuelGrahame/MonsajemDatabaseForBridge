using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static ArrayExtentions.ArrayExtentions;
using static ArrayExtentions.ArrayReturns;

namespace KeyValueDatabase
{
    public partial class Table<ValueType>
    {
        [Serializable]
        public class RelationItem
        {
            private Table<ValueType> P_Array;
            public Table<ValueType> Array
            {
                get => P_Array;
                set
                {
                    this.P_Array = value;
                    if (P_GetValue != null)
                    {
                        Value = P_GetValue();
                        P_GetValue = null;
                    }
                }
            }
            public string Key;
            private Func<ValueType> P_GetValue;
            public ValueType Value
            {
                get => Array.GetItem(Key);
                set
                {
                    Key = Array.Extras.GetKey(value);
                }
            }

            public static implicit operator ValueType(RelationItem RL)
            {
                return RL.Value;
            }

            public static implicit operator RelationItem(ValueType Value)
            {
                return new RelationItem() { P_GetValue = () => Value };
            }
        }

        public void AddRelation<To>(
            Expression<Func<ValueType, Table<To>>> ThisRelationLink,
            Table<To> RelationArray,
            Expression<Func<To, Table<ValueType>>> ThatRelationLink)
        {
            var ThisFild = typeof(ValueType).GetField(((MemberExpression)ThisRelationLink.Body).Member.Name);
            var ThatFild = typeof(To).GetField(((MemberExpression)ThatRelationLink.Body).Member.Name);

            this.Extras.Inner_loading += (Value) =>
            {
                var ThisRelation =(Table<To>)ThisFild.GetValue(Value);

                if (ThisRelation == null)
                    ThisRelation = new Table<To>(new string[0], RelationArray);
                else if(ThisRelation.Extras==null)
                    ThisRelation = new Table<To>(ThisRelation.Keys, RelationArray);
                else
                    return;

                ThisRelation.Extras.Accepted += (Key, p) =>
                {
                    BinaryInsert(ref ThisRelation.Extras.Accepteds,Key);
                    BinaryDelete(ref ThisRelation.Extras.Ignoreds, Key);
                };
                ThisRelation.Extras.Ignored += (Key, p) =>
                {
                    BinaryDelete(ref ThisRelation.Extras.Accepteds, Key);
                    BinaryInsert(ref ThisRelation.Extras.Ignoreds, Key);
                };
                ThisRelation.Extras.Deleted += (Key, p) =>
                {
                    BinaryDelete(ref ThisRelation.Extras.Accepteds, Key);
                    BinaryDelete(ref ThisRelation.Extras.Ignoreds, Key);
                };
                ThisRelation.Extras.KeyChanged += (OldKey, NewKey) =>
                {
                    if (BinaryDelete(ref ThisRelation.Extras.Accepteds, OldKey)>-1)
                        BinaryInsert(ref ThisRelation.Extras.Accepteds, OldKey);
                };

                ThisFild.SetValue(Value,ThisRelation);
            };
            RelationArray.Extras.Inner_loading += (Value) =>
            {
                var ThatRelation = (Table<ValueType>)ThatFild.GetValue(Value);

                if (ThatRelation == null)
                    ThatRelation = new Table<ValueType>(new string[0], this);
                else if (ThatRelation.Extras == null)
                    ThatRelation =
                        new Table<ValueType>(ThatRelation.Keys, this);
                else
                    return;

                ThatRelation.Extras.Accepted += (Key, p) =>
                {
                    BinaryInsert(ref ThatRelation.Extras.Accepteds, Key);
                    BinaryDelete(ref ThatRelation.Extras.Ignoreds, Key);
                };
                ThatRelation.Extras.Ignored += (Key, p) =>
                {
                    BinaryDelete(ref ThatRelation.Extras.Accepteds, Key);
                    BinaryInsert(ref ThatRelation.Extras.Ignoreds, Key);
                };
                ThatRelation.Extras.Deleted += (Key, p) =>
                {
                    BinaryDelete(ref ThatRelation.Extras.Accepteds, Key);
                    BinaryDelete(ref ThatRelation.Extras.Ignoreds, Key);
                };
                ThatRelation.Extras.KeyChanged += (OldKey, NewKey) =>
                {
                    if (BinaryDelete(ref ThatRelation.Extras.Accepteds, OldKey) > -1)
                        BinaryInsert(ref ThatRelation.Extras.Accepteds, OldKey);
                };
                ThatFild.SetValue(Value, ThatRelation);
            };

            this.Extras.Inner_Saving += (Value) =>
            {
                var ThisRelation = (Table<To>)ThisFild.GetValue(Value);

                var Key = this.Extras.GetKey(Value);
                RelationArray.GetItems(ThisRelation.Extras.Ignoreds).Update(
                    (c) => BinaryDelete(ref ((Table<ValueType>)ThatFild.GetValue(c)).Keys, Key));

                RelationArray.GetItems(ThisRelation.Extras.Accepteds).Update(
                    (c)=> BinaryInsert(ref ((Table<ValueType>)ThatFild.GetValue(c)).Keys, Key));

                ThisRelation.Extras = null;
            };
            RelationArray.Extras.Inner_Saving += (Value) =>
            {
                var ThatRelation = (Table<ValueType>)ThatFild.GetValue(Value);
                var Key = RelationArray.Extras.GetKey(Value);

                this.GetItems(ThatRelation.Extras.Ignoreds).Update(
                    (c)=> BinaryDelete(ref ((Table<To>)ThisFild.GetValue(c)).Keys, Key));

                this.GetItems(ThatRelation.Extras.Accepteds).Update(
                    (c)=> BinaryInsert(ref ((Table<To>)ThisFild.GetValue(c)).Keys, Key));

                ThatRelation.Extras = null;
            };
        }

        public void AddRelation<To>(
            Expression<Func<ValueType, Table<To>.RelationItem>> ThisRelationLink,
            Table<To> RelationArray,
            Expression<Func<To, Table<ValueType>>> ThatRelationLink)
        {
            var ThisFild = typeof(ValueType).GetField(((MemberExpression)ThisRelationLink.Body).Member.Name);
            var ThatFild = typeof(To).GetField(((MemberExpression)ThatRelationLink.Body).Member.Name);

            this.Extras.Inner_loading += (Value) =>
            {
                var ThisRelation = (Table<To>.RelationItem)ThisFild.GetValue(Value);
                if (ThisRelation == null)
                    ThisRelation = new Table<To>.RelationItem();
                if (ThisRelation.Array == null)
                {
                    ThisRelation.Array = new Table<To>(RelationArray.Keys, RelationArray);
                    ThisRelation.Array.Extras.OldKey = ThisRelation.Key;
                }
                ThisFild.SetValue(Value, ThisRelation);
            };
            RelationArray.Extras.Inner_loading += (Value) =>
            {
                var ThatRelation = (Table<ValueType>)ThatFild.GetValue(Value);
                if (ThatRelation == null)
                    ThatRelation = new Table<ValueType>(new string[0], this);
                else if (ThatRelation.Extras == null)
                    ThatRelation =new Table<ValueType>(ThatRelation.Keys, this);
                else
                    return;

                ThatRelation.Extras.Accepted += (Key, p) =>
                {
                    BinaryInsert(ref ThatRelation.Extras.Accepteds, Key);
                    BinaryDelete(ref ThatRelation.Extras.Ignoreds, Key);
                };
                ThatRelation.Extras.Ignored += (Key, p) =>
                {
                    BinaryDelete(ref ThatRelation.Extras.Accepteds, Key);
                    BinaryInsert(ref ThatRelation.Extras.Ignoreds, Key);
                };
                ThatRelation.Extras.Deleted += (Key, p) =>
                {
                    BinaryDelete(ref ThatRelation.Extras.Accepteds, Key);
                    BinaryDelete(ref ThatRelation.Extras.Ignoreds, Key);
                };
                ThatRelation.Extras.KeyChanged += (OldKey, NewKey) =>
                {
                    if (BinaryDelete(ref ThatRelation.Extras.Accepteds, OldKey) > -1)
                        BinaryInsert(ref ThatRelation.Extras.Accepteds, OldKey);
                };
                ThatFild.SetValue(Value, ThatRelation);
            };

            this.Extras.Inner_Saving += (Value) =>
            {
                var ThisRelation = (Table<To>.RelationItem)ThisFild.GetValue(Value);
                var Key = ThisRelation.Key;
                var OldKey = ThisRelation.Array.Extras.OldKey;
                if (Key != OldKey)
                {
                    if (Key != null)
                    {
                        ThisRelation.Array.Hidden.Update(Key,
                            (c) =>BinaryInsert(ref ((Table<ValueType>)ThatFild.GetValue(c)).Keys, 
                                             this.Extras.GetKey(Value)));
                    }
                    if (OldKey != null)
                        ThisRelation.Array.Hidden.Update(OldKey,
                            (c) =>BinaryDelete(ref ((Table<ValueType>)ThatFild.GetValue(c)).Keys, 
                                             this.Extras.GetKey(Value)));
                }
                ThisRelation.Array = null;
            };
            RelationArray.Extras.Inner_Saving += (Value) =>
            {
                var ThatRelation = (Table<ValueType>)ThatFild.GetValue(Value);
                var Key = RelationArray.Extras.GetKey(Value);

                this.GetItems(ThatRelation.Extras.Ignoreds).Hidden.Update(
                    (c) => ((Table<To>.RelationItem)ThisFild.GetValue(c)).Key = null);

                this.GetItems(ThatRelation.Extras.Accepteds).Hidden.Update(
                    (c) => ((Table<To>.RelationItem)ThisFild.GetValue(c)).Key = Key);

                ThatRelation.Extras = null;
            };
        }
        public void AddRelation<To>(
            Expression<Func<ValueType, Table<To>>> ThisRelationLink,
            Table<To> RelationArray,
            Expression<Func<To, RelationItem>> ThatRelationLink)
        {
            RelationArray.AddRelation(ThatRelationLink, this, ThisRelationLink);
        }

        public void AddRelation<To>(
            Expression<Func<ValueType, Table<To>.RelationItem>> ThisRelationLink,
            Table<To> RelationArray,
            Expression<Func<To, RelationItem>> ThatRelationLink)
        {
            var ThisFild = typeof(ValueType).GetField(((MemberExpression)ThisRelationLink.Body).Member.Name);
            var ThatFild = typeof(To).GetField(((MemberExpression)ThatRelationLink.Body).Member.Name);

            this.Extras.Inner_loading += (Value) =>
            {
                var ThisRelation = (Table<To>.RelationItem)ThisFild.GetValue(Value);
                if (ThisRelation == null)
                    ThisRelation = new Table<To>.RelationItem();
                if (ThisRelation.Array == null)
                {
                    ThisRelation.Array = new Table<To>(RelationArray.Keys, RelationArray);
                    ThisRelation.Array.Extras.OldKey = ThisRelation.Key;
                }
                ThisFild.SetValue(Value, ThisRelation);
            };
            RelationArray.Extras.Inner_loading += (Value) =>
            {
                var ThatRelation = (Table<ValueType>.RelationItem)ThatFild.GetValue(Value);
                if (ThatRelation == null)
                    ThatRelation = new Table<ValueType>.RelationItem();
                if (ThatRelation.Array == null)
                {
                    ThatRelation.Array = new Table<ValueType>(RelationArray.Keys, this);
                    ThatRelation.Array.Extras.OldKey = ThatRelation.Key;
                }
                ThatFild.SetValue(Value, ThatRelation);
            };

            this.Extras.Inner_Saving += (Value) =>
            {
                var ThisRelation = (Table<To>.RelationItem)ThisFild.GetValue(Value);
                var Key = ThisRelation.Key;
                var OldKey = ThisRelation.Array.Extras.OldKey;
                if (Key != OldKey)
                {
                    if (Key != null)
                    {
                        ThisRelation.Array.Hidden.Update(Key,
                            (c) => ((Table<ValueType>.RelationItem)ThatFild.GetValue(c)).Key = this.Extras.GetKey(Value));
                    }
                    if (OldKey != null)
                        ThisRelation.Array.Hidden.Update(OldKey,
                            (c) => ((Table<ValueType>.RelationItem)ThatFild.GetValue(c)).Key = null);
                }
                ThisRelation.Array = null;
            };
            RelationArray.Extras.Inner_Saving += (Value) =>
            {
                var ThatRelation = (Table<ValueType>.RelationItem)ThatFild.GetValue(Value);
                var Key = ThatRelation.Key;
                var OldKey = ThatRelation.Array.Extras.OldKey;
                if (Key != OldKey)
                {
                    if (Key != null)
                    {
                        ThatRelation.Array.Hidden.Update(Key,
                            (c) => ((Table<To>.RelationItem)ThisFild.GetValue(c)).Key = RelationArray.Extras.GetKey(Value));
                    }
                    if (OldKey != null)
                        ThatRelation.Array.Hidden.Update(OldKey,
                            (c) => ((Table<To>.RelationItem)ThisFild.GetValue(c)).Key = null);
                }
                ThatRelation.Array = null;
            };
        }
    }
    }
