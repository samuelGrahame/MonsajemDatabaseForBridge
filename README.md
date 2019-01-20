# <img src="https://github.com/miladsadeghi72/NightRavenProject/blob/master/MonsajemLogo.png" width="50" height="50">Monsajem Soft incs
<img src="https://github.com/miladsadeghi72/NightRavenProject/blob/master/Night%20Raven%20Project.jpg" width="100%">

# why shold use this
in fact you don't have to, because every things in the world is just offer, no ruls exist.
however we provide this product fast and standard and we use this in real projects, so don't worry about bugs.
if we recive any report about bug then next day fixed, or tell what is our logic.

# Sample Codes:

```csharp

// Person Model
public class Person
{
    public string Name;
    public Table<Product>.RelationItem Product;
    public Table<Product> Products;
    public Table<Product> Products2;
}

// Produc Model
public class Product
{
    public string Name;
    public Table<Person>.RelationItem Person;
    public Table<Person>.RelationItem Person2;
    public Table<Person> Persons;
}

var Persons = Make<Person>(
                    (person) => person.Name,
                    "Persons");
var Products = Make<Product>(
                    (Product) => Product.Name,
                    "products");

//1 to 1 Relation
Products.AddRelation((c) => c.Person, Persons, (c) => c.Product);

//m to n Relation
Products.AddRelation((c) => c.Persons, Persons, (c) => c.Products);

// 1 to n Relation
Products.AddRelation((c) => c.Person2, Persons, (c) => c.Products2);

Products.Insert((c) => c.Name = "p1");
Persons.Insert((c) => c.Name = "ali");

Products.Update((c) => c.Name = "p1", (c) => c.Person.Value = Persons.GetItem((c2) => c2.Name = "ali"));//1 to 1
Products.Update((c) => c.Name = "p1", (c) => c.Person2.Value = Persons.GetItem((c2) => c2.Name = "ali"));//n to 1
Products.Update((c) => c.Name = "p1", (c) => c.Persons.Accept(Persons.GetItem((c2) => c2.Name = "ali")));//m to n

Persons.Delete();
Products.Delete();

Products.SaveChange();
Persons.SaveChange();


```
