using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayExtentions
{


    public static class ArrayExtentions
    {

        public static void DeleteByPosition<t>(ref t[] ar, int Position)
        {
            if (Position == ar.Length - 1)
            {
                Array.Resize(ref ar, Position);
                return;
            }
            System.Array.Copy(ar, 0, ar, 0, Position);
            System.Array.Copy(ar, Position + 1, ar, Position, (ar.Length - Position) - 1);
            System.Array.Resize(ref ar, ar.Length - 1);
        }
        public static void DeleteFrom<t>(ref t[] ar, int from)
        {
            Array.Resize(ref ar, from);
        }
        public static void DeleteFromTo<t>(ref t[] ar, int from,int To)
        {
            System.Array.Copy(ar, To+1, ar, from, ar.Length-(To+1));
            Array.Resize(ref ar, ar.Length - (To - from + 1));
        }
        public static void DeleteTo<t>(ref t[] ar, int To)
        {
            System.Array.Copy(ar, To+1, ar, 0, ar.Length - (To+1));
            Array.Resize(ref ar, ar.Length - (To+1));
        }

        public static t Pop<t>(ref t[] ar)
        {
            var Item = ar[ar.Length - 1];
            Array.Resize(ref ar, ar.Length - 1);
            return Item;
        }
        public static t[] PopFrom<t>(ref t[] ar, int From)
        {
            var Result = ar.From(From);
            DeleteFrom(ref ar, From);
            return Result;
        }
        public static t[] PopTo<t>(ref t[] ar, int to)
        {
            var Result = ar.To(to);
            DeleteTo(ref ar, to);
            return Result;
        }
        public static t[] PopFromTo<t>(ref t[] ar,int From,int To)
        {
            var Result = ar.From(From).To(To);
            DeleteFromTo(ref ar, From, To);
            return Result;
        }

        public static int BinaryDelete<t>(ref t[] ar, t Value)
        {
            var Place = System.Array.BinarySearch(ar, Value);
            if(Place >= 0)
            {
                DeleteByPosition(ref ar,Place);
            }
            return Place;
        }

        public static void BinaryDelete<t>(ref t[] ar, IEnumerable<t> Values)
        {
            foreach (var Value in Values)
                BinaryDelete(ref ar, Value);
        }

        public static int BinaryInsert<t>(ref t[] ar, t Value)
        {
            var Place = System.Array.BinarySearch(ar, Value);
            if (Place < 0)
                Place = (Place * -1) - 1;
            Insert(ref ar, Value, Place);
            return Place;
        }
        public static void BinaryInsert<t>(ref t[] ar,params t[] Values)
        {
            for (int i=0;i<Values.Length;i++)
            {
                BinaryInsert(ref ar, Values[i]);
            }
        }
        public static void BinaryInsert<t>(ref t[] ar, IEnumerable<t> Values)
        {
            foreach(var Value in Values)
            {
                BinaryInsert(ref ar, Value);
            }
        }

        public static void Insert<t>(ref t[] ar, t Value)
        {
            Array.Resize(ref ar, ar.Length + 1);
            ar[ar.Length - 1] = Value;
        }

        public static void Insert<t>(ref t[] ar,params t[] Values)
        {
            var From = ar.Length;
            Array.Resize(ref ar, ar.Length + Values.Length);
            System.Array.Copy(Values, 0, ar, From, Values.Length);
        }
        public static void Insert<t>(ref t[] ar, IEnumerable<t> Values)
        {
            var From = ar.Length;
            var Count = Values.Count();
            Array.Resize(ref ar, ar.Length + Count);
            var i = From;
            Count = ar.Length;
            var Reader = Values.GetEnumerator();
            Reader.MoveNext();
            while(i<Count)
            {
                ar[i] = Reader.Current;
                Reader.MoveNext();
                i++;
            }
            Reader.Dispose();
        }
        public static void Insert<t>(ref t[] ar, t[] Values,int From)
        {
            var ArLen = ar.Length;
            Array.Resize(ref ar, ar.Length + Values.Length);
            System.Array.Copy(ar, From, ar, ArLen+1, ArLen - From);
            System.Array.Copy(Values, 0, ar, From, Values.Length);
        }
        public static void Insert<t>(ref t[] ar, IEnumerable<t> Values, int From)
        {
            var ArLen = ar.Length;
            var Count = Values.Count();
            Array.Resize(ref ar, ar.Length + Count);
            System.Array.Copy(ar, From, ar, ArLen+1, ArLen - From);
            var i = From;
            Count = ar.Length;
            var Reader = Values.GetEnumerator();
            Reader.MoveNext();
            while (i < Count)
            {
                ar[i] = Reader.Current;
                Reader.MoveNext();
                i++;
            }
            Reader.Dispose();
        }
        public static void Insert<t>(ref t[] ar, t Value, int Position)
        {
            Array.Resize(ref ar, ar.Length + 1);
            if (Position == ar.Length - 1)
                ar[ar.Length - 1] = Value;
            else
            {
                System.Array.Copy(
                    ar, Position,
                    ar, Position + 1, ar.Length - Position - 1);
                ar[Position] = Value;
            }
        }

        public static void DropFromInsertTo<t>(ref t[] ar, int From, int To, t Value)
        {
            if (From < To)
            {
                System.Array.Copy(ar, From + 1, ar, From, (To - From));
            }
            else if (From > To)
            {
                System.Array.Copy(ar, To, ar, To + 1, (From - To));
            }
            ar[To] = Value;
        }
        public static void DropFromInsertTo<t>(ref t[] ar, int From, int To)
        {
            var Value = ar[From];
            if (From < To)
            {
                System.Array.Copy(ar, From + 1, ar, From, (To - From));
            }
            else if (From > To)
            {
                System.Array.Copy(ar, To, ar, To + 1, (From - To));
            }
            ar[To] = Value;
        }
    }
}
