using System;
using System.Reflection;
using static System.Text.Encoding;
using static ArrayExtentions.ArrayExtentions;
using System.Linq;

namespace Monsajem_incs
{

    public static class Serialization
    {
        static Serialization()
        {
            InsertSerializer<object>(
            (object obj, ref byte[] Data,ref int From) =>{
 
                if (obj == null)
                {
                    if (Data.Length <= From + 1)
                        Array.Resize(ref Data, Data.Length + Cash);
                    Data[From] = 0;
                    From += 1;
                    return;
                }
                var Type = obj.GetType();
                if (Type ==typeof(object))
                {
                    if (Data.Length <= From + 1)
                        Array.Resize(ref Data, Data.Length + Cash);
                    Data[From] = 0;
                    From += 1;
                    return;
                }

                Data[From] = 1;
                From += 1;

                var Name = Unicode.GetBytes(Type.Assembly.FullName);
                if (Data.Length <= From + Name.Length+4)
                    Array.Resize(ref Data, Data.Length + Cash+Name.Length+4);
                Array.Copy(BitConverter.GetBytes(Name.Length), 0, Data, From, 4);
                From += 4;
                Array.Copy(Name, 0, Data, From, Name.Length);
                From += Name.Length;

                Name = Unicode.GetBytes(Type.FullName);
                if (Data.Length <= From + Name.Length + 4)
                    Array.Resize(ref Data, Data.Length + Cash + Name.Length + 4);
                Array.Copy(BitConverter.GetBytes(Name.Length), 0, Data, From, 4);
                From += 4;
                Array.Copy(Name, 0, Data, From, Name.Length);
                From += Name.Length;

                GetSerializer(Type)(obj, ref Data, ref From);

            },
            (ref byte[] Data, ref int From) =>{

                if (Data[From] == 0)
                {
                    From += 1;
                    return null;
                }

                From += 1;

                var AssemblyName = Unicode.GetString(Data, From + 4, BitConverter.ToInt32(Data, From));
                From += 4+(AssemblyName.Length*2);
                var TypeName = Unicode.GetString(Data, From + 4, BitConverter.ToInt32(Data, From));
                From += 4 + (TypeName.Length * 2);
                var Type =Assembly.Load(AssemblyName).GetType(TypeName);
               return GetDeserializer(Type)(ref Data, ref From);
            });

            InsertSerializer<bool>(
            (object Obj, ref byte[] Data, ref int From) =>{                 
                if (Data.Length <= From + 1)
                    Array.Resize(ref Data, Data.Length + Cash);
                if ((bool)Obj == true)
                    Data[From] = 1;
                else
                    Data[From] = 0;
                From += 1;
            },
            (ref byte[] c, ref int p) =>
            {                 
                int p2 = p; p += 1;
                var Result = ((byte[])c)[p2];
                return Result > 0;
            });

            InsertSerializer<char>(
            (object Obj, ref byte[] Data, ref int From) =>
            {
                if (Data.Length <= From + 2)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((char)Obj), 0, Data, From, 2);
                From += 2;
            }, 
            (ref byte[] c, ref int p) =>
            {
                int p2 = p; p += 2;
                return BitConverter.ToChar((byte[])c, p2);
            });

            InsertSerializer<byte>(
            (object Obj, ref byte[] Data, ref int From) =>
            {
                if (Data.Length <= From + 1)
                    Array.Resize(ref Data, Data.Length + Cash);
                Data[From] = (byte)Obj;
                From += 1;
            },
            (ref byte[] c, ref int p) =>
            {                
                int p2 = p; p += 1;
                return ((byte[])c)[p2];
            });

            InsertSerializer<short>(
            (object Obj, ref byte[] Data, ref int From) =>
            {              
                if (Data.Length <= From + 2)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((short)Obj), 0, Data, From, 2);
                From += 2;
            },
            (ref byte[] c, ref int p) =>
            {           
                int p2 = p; p += 2;
                return BitConverter.ToInt16((byte[])c, p2);
            });

            InsertSerializer<ushort>(
            (object Obj, ref byte[] Data, ref int From) =>
            {
                if (Data.Length <= From + 2)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((ushort)Obj), 0, Data, From, 2);
                From += 2;
            },
            (ref byte[] c, ref int p) =>
            {                 /// as UInt16
                int p2 = p; p += 2;
                return BitConverter.ToUInt16((byte[])c, p2);
            });

            InsertSerializer<int>(
            (object Obj, ref byte[] Data, ref int From) =>
            {                 /// as Int32
                if (Data.Length <= From + 4)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((int)Obj), 0, Data, From, 4);
                From += 4;
            },
            (ref byte[] c, ref int p) =>
            {                 /// as Int32
                int p2 = p; p += 4;
                return BitConverter.ToInt32((byte[])c, p2);
            });

            InsertSerializer<uint>(
            (object Obj, ref byte[] Data, ref int From) =>
            {                 /// as UInt32
                if (Data.Length <= From + 4)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((uint)Obj), 0, Data, From, 4);
                From += 4;
            },
            (ref byte[] c, ref int p) =>
            {                 /// as UInt32
                int p2 = p; p += 4;
                return BitConverter.ToUInt32((byte[])c, p2);
            });

            InsertSerializer<long>(
            (object Obj, ref byte[] Data, ref int From) =>
            {                 /// as Int64
                if (Data.Length <= From + 8)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((long)Obj), 0, Data, From, 8);
                From += 8;
            },
            (ref byte[] c, ref int p) =>
            {                 /// as Int64
                int p2 = p; p += 8;
                return BitConverter.ToInt64((byte[])c, p2);
            });

            InsertSerializer<ulong>(
            (object Obj, ref byte[] Data, ref int From) =>
            {
                if (Data.Length <= From + 8)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((ulong)Obj), 0, Data, From, 8);
                From += 8;
            },
            (ref byte[] c, ref int p) =>
            {
                int p2 = p; p += 8;
                return BitConverter.ToUInt64((byte[])c, p2);
            });

            InsertSerializer<float>(
            (object Obj, ref byte[] Data, ref int From) =>
            {                 /// as Single
                if (Data.Length <= From + 4)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((float)Obj), 0, Data, From, 4);
                From += 4;
            },
            (ref byte[] c, ref int p) =>
            {                 /// as Single
                throw new System.Exception("Single siza?");
                int p2 = p; p += 1;
                return BitConverter.ToSingle((byte[])c, p2);
            });

            InsertSerializer<double>(
            (object Obj, ref byte[] Data, ref int From) =>
            {                 /// as double
                if (Data.Length <= From + 8)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes((double)Obj), 0, Data, From, 8);
                From += 8;
            },
            (ref byte[] c, ref int p) =>
            {                 /// as double
                throw new System.Exception("Double siza?");
                int p2 = p; p += 1;
                return BitConverter.ToDouble((byte[])c, p2);
            });

            InsertSerializer<DateTime>(
            (object Obj, ref byte[] Data, ref int From) =>
            {                
                if (Data.Length <= From + 8)
                    Array.Resize(ref Data, Data.Length + Cash);
                Array.Copy(BitConverter.GetBytes(((DateTime)Obj).ToFileTime()), 0, Data, From, 8);
                From += 8;
            },
            (ref byte[] c, ref int p) =>
            {                 
                int p2 = p; p += 8;
                return DateTime.FromFileTime(BitConverter.ToInt64((byte[])c, p2));
            });

            InsertSerializer<string>(
            (object Obj, ref byte[] Data, ref int From) =>
            {                 
                if(Obj==null)
                {
                    Array.Copy(BitConverter.GetBytes((int)-1), 0, Data, From, 4);
                    From += 4;
                    return;
                }
                var Str = Unicode.GetBytes((string)Obj);
                var Len = BitConverter.GetBytes(Str.Length);
                if (Data.Length <= From + 4 + Str.Length)
                    Array.Resize(ref Data, Data.Length + Cash + 4 + Str.Length);
                Array.Copy(Len, 0, Data, From, 4);
                From += 4;
                Array.Copy(Str, 0, Data, From, Str.Length);
                From += Str.Length;
            },
            (ref byte[] c, ref int p) =>
            {                
                var StrSize = BitConverter.ToInt32((byte[])c, p);
                p += 4;
                if (StrSize == -1)
                    return null;
                var p2 = p;
                p += StrSize;
                return Unicode.GetString((byte[])c, p2, StrSize);
            });

            
        }

        private static void InsertSerializer<t>(
            SerializeDeleagte Serializer,
            DeserializeDeleagte Deserializer)
        {
            var HashCode = typeof(t).GetHashCode();
            var Position = BinaryInsert(ref SerializersHashCodes, HashCode);
            Insert(ref ExactSerializers, Serializer, Position);
            Insert(ref DeserializersHashCodes, HashCode, Position);
            Insert(ref ExactDeserializers, Deserializer, Position);
        }

        public static Func<Type, object> CreateInstance = (t) =>
             throw new MissingMethodException("Please set CreateInstance method in Serialization");
        public static int Cash=500;

        private static SerializeDeleagte[] ExactSerializers = new SerializeDeleagte[0];
        private static int[] SerializersHashCodes = new int[0];
        private static DeserializeDeleagte[] ExactDeserializers = new DeserializeDeleagte[0];
        private static int[] DeserializersHashCodes = new int[0];

        private delegate object DeserializeDeleagte(ref byte[] Data, ref int From);
        private delegate void SerializeDeleagte(object Obj,ref byte[] Data, ref int From);

        public static byte[] Serialize<t>(this t obj)
        {
            var Data = new byte[Cash];
            var From = 0;
            var Object = (object)obj;
            GetSerializer(typeof(t))(Object, ref Data,ref From);
            Array.Resize(ref Data, From);
            return Data;
        }



        public static t Deserialize<t>(this byte[] Data)
        {
            var From = 0;
            return (t)GetDeserializer(typeof(t))(ref Data, ref From);
        }

        public static object Deserialize(this byte[] Data,Type Type)
        {
            var From = 0;
            return GetDeserializer(Type)(ref Data, ref From);
        }

        private static SerializeDeleagte GetSerializer(Type Type)
        {
            var Position = Array.BinarySearch(SerializersHashCodes, Type.GetHashCode());
            if (Position > -1)
                return ExactSerializers[Position];
            if (Type.IsArray)
            {
                var ItemsSerializer = GetSerializer(Type.GetElementType());

                SerializeDeleagte Serializer = (object Obj, ref byte[] Data, ref int From) =>
                {
                    var ar = (Array)Obj;
                    if (Data.Length <= From + 4)
                        Array.Resize(ref Data, Data.Length + Cash);
                    if (Obj == null)
                    {
                        Array.Copy(BitConverter.GetBytes((int)0), 0, Data, From, 4);
                        From += 4;
                        return;
                    }
                    else
                    {
                        Array.Copy(BitConverter.GetBytes(ar.Rank), 0, Data, From, 4);
                        From += 4;
                    }

                    var Currents = new int[ar.Rank];
                    var Ends = new int[ar.Rank];

                    for (int i = 0; i < ar.Rank; i++)
                    {
                        Ends[i] = ar.GetUpperBound(i) + 1;
                        Array.Copy(BitConverter.GetBytes(Ends[i]), 0, Data, From, 4);
                        From += 4;
                    }

                    while (Currents[Currents.Length - 1] < Ends[Ends.Length - 1])
                    {
                        for (Currents[0] = 0; Currents[0] < Ends[0]; Currents[0]++)
                        {
                            ItemsSerializer(ar.GetValue(Currents),ref Data,ref From);
                        }
                        for (int i = 1; i < ar.Rank; i++)
                        {
                            if (Currents[i] < Ends[i])
                            {
                                Currents[i]++;
                                Currents[i - 1] = 0;
                            }
                        }
                    }

                };
                Position = BinaryInsert(ref SerializersHashCodes, Type.GetHashCode());
                Insert(ref ExactSerializers, Serializer, Position);
                return Serializer;
            }
            else
            {
                var Filds = Type.GetFields(
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.CreateInstance |
                    BindingFlags.Instance).OrderBy((c)=> c.Name).ToArray();
                var FildSerializer = new SerializeDeleagte[Filds.Length];

                for (int i=0;i<Filds.Length;i++)
                {
                    FildSerializer[i] = GetSerializer(Filds[i].FieldType);
                }

                SerializeDeleagte Serializer = (object Obj, ref byte[] Data, ref int From) =>
                {
                    if (Data.Length <= From + 1)
                        Array.Resize(ref Data, Data.Length + Cash);
                    if (Obj == null)
                    {
                        Data[From] = 0;
                        From += 1;
                    }
                    else
                    {
                        Data[From] = 1;
                        From += 1;
                        for (int i = 0; i < Filds.Length; i++)
                        {
                            FildSerializer[i](Filds[i].GetValue(Obj), ref Data, ref From);
                        }
                    }
                    
                };

                
                Position = BinaryInsert(ref SerializersHashCodes, Type.GetHashCode());
                Insert(ref ExactSerializers, Serializer, Position);
                return Serializer;
            }
        }

        private static DeserializeDeleagte GetDeserializer(Type Type)
        {
            var Position = Array.BinarySearch(DeserializersHashCodes, Type.GetHashCode());
            if (Position > -1)
                return ExactDeserializers[Position];
            if (Type.IsArray)
            {
                var ItemType = Type.GetElementType();
                var ItemsDeserializer = GetDeserializer(ItemType);

                DeserializeDeleagte Deserializer = (ref byte[] Data, ref int from) =>
                {
                    var Rank = BitConverter.ToInt32(Data, from);
                    from += 4;
                    if (Rank == 0)
                    {
                        return null;
                    }
                    var Currents = new int[Rank];
                    var Ends = new int[Rank];
                    for (int i = 0; i < Rank; i++)
                    {
                        Ends[i]=BitConverter.ToInt32(Data, from);
                        from += 4;
                    }
                    var ar = Array.CreateInstance(ItemType, Ends);

                    while (Currents[Currents.Length - 1] < Ends[Ends.Length - 1])
                    {
                        for (Currents[0] = 0; Currents[0] < Ends[0]; Currents[0]++)
                        {
                            ar.SetValue(ItemsDeserializer(ref Data, ref from), Currents);
                        }
                        for (int i = 1; i < ar.Rank; i++)
                        {
                            if (Currents[i] < Ends[i])
                            {
                                Currents[i]++;
                                Currents[i - 1] = 0;
                            }
                        }
                    }
                    return ar;
                };
                Position = BinaryInsert(ref DeserializersHashCodes, Type.GetHashCode());
                Insert(ref ExactDeserializers, Deserializer, Position);
                return Deserializer;
            }
            else
            {
                var Filds = Type.GetFields(
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.CreateInstance |
                    BindingFlags.Instance).OrderBy((c) => c.Name).ToArray();
                DeserializeDeleagte[] FildDeserializer = new DeserializeDeleagte[Filds.Length];

                for (int i = 0; i < Filds.Length; i++)
                    FildDeserializer[i] = GetDeserializer(Filds[i].FieldType);

                DeserializeDeleagte Deserializer = (ref byte[] Data, ref int from) =>
                {
                    if (Data[from] == 0)
                    {
                        from += 1;
                        return null;
                    }
                    from += 1;
                    object Result = CreateInstance(Type);
                    for (int i = 0; i < Filds.Length; i++)
                    {
                        Filds[i].SetValue(Result, FildDeserializer[i](ref Data, ref from));
                    }
                    return Result;
                };
                Position = BinaryInsert(ref DeserializersHashCodes, Type.GetHashCode());
                Insert(ref ExactDeserializers, Deserializer, Position);
                return Deserializer;
            }
        }
    }
}
