using Bridge;
using System;
using KeyValueDatabase;
using System.Linq;
using Bridge.Html5;

namespace HtmlDatabase
{
    public static class DatabaseMaker
    {
        public static Table<t> Make<t>(
            Func<t, string> Key,
            string TableName)
        {
            Storage Storage = null;
            Script.Eval("Storage=localStorage;");
            return new Table<t>(
                GetKey: Key,
               GetItem:(k) =>
               {
                   
                return Convert.FromBase64String((string)Storage.GetItem(k));
               },
               Insert:(Obj, k) =>
               {
                   Storage.SetItem(k, Convert.ToBase64String(Obj));
               },
               Delete:(k) =>
               {
                   Storage.RemoveItem(k);
               },
               PreName: TableName);
        }
    }

}