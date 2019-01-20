

using Bridge;
using System;
using static HtmlDatabase.DatabaseMaker;
using KeyValueDatabase;
using System.Linq;
using Bridge.Html5;
using Monsajem_incs;
using static ArrayExtentions.ArrayExtentions;
using System.Collections.Generic;
using System.Collections;

namespace TestOnIt
{
    public class Person
    {
        public string Name;
        public Table<Product>.RelationItem Product;
        public Table<Product> Products;
        public Table<Product> Products2;
    }

    public class Product
    {
        public string Name;
        public Table<Person>.RelationItem Person;
        public Table<Person>.RelationItem Person2;
        public Table<Person> Persons;
    }

    public class App
    {
        public static void Main()
        {


            Serialization.CreateInstance = (Type) =>
      Type.Assembly.CreateInstance(Type.FullName);

            Storage Storage = null;
            Script.Eval("Storage=localStorage;");
            Storage.Clear();

            var Persons = Make<Person>(
                    (person) => person.Name,
                    "Persons");
                var Products = Make<Product>(
                    (Product) => Product.Name,
                    "products");
                Products.AddRelation((c) => c.Person, Persons, (c) => c.Product);//1 to 1
                Products.AddRelation((c) => c.Persons, Persons, (c) => c.Products);//m to n
                Products.AddRelation((c) => c.Person2, Persons, (c) => c.Products2);// 1 to n

                Persons.Delete();
                Products.Delete();


                Products.Insert((c) => c.Name = "p1");
                Persons.Insert((c) => c.Name = "ali");

                Products.Update((c) => c.Name = "p1", (c) => c.Person.Value = Persons.GetItem((c2) => c2.Name = "ali"));//1 to 1
                Products.Update((c) => c.Name = "p1", (c) => c.Person2.Value = Persons.GetItem((c2) => c2.Name = "ali"));//n to 1
                Products.Update((c) => c.Name = "p1", (c) => c.Persons.Accept(Persons.GetItem((c2) => c2.Name = "ali")));//m to n

                Products.SaveChange();
                Persons.SaveChange();

                var Ipr = Products.First();
                var Ips = Persons.First();

                Console.WriteLine(Ipr.Name);
                Console.WriteLine(Ipr.Person.Value.Name);
                Console.WriteLine(Ipr.Persons.First().Name);
        }
    }
}