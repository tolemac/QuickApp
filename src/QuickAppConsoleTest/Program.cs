using System;
using Newtonsoft.Json.Linq;
using QuickApp;
using QuickApp.MongoDb;
using QuickApp.Services;

namespace QuickAppConsoleTest
{
    public class Service1
    {
        public string Saludar(string nombre)
        {
            Console.WriteLine("hola " + nombre);
            return "Ok!";
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new QuickApplication(null);
            app.AddService(new ServiceDescriptor(typeof(Service1), () => new Service1()));
            app.AddService(new ServiceDescriptor(typeof(IMongoDbDatabaseService),
                () => new MongoDbDatabaseService("mongodb://localhost:27017", "prueba")));

            app.CallServiceMethod("MongoDbDataBaseService", "InsertOne", new
            {
                collectionName = "People",
                document = new
                {
                    nombre = "Javier",
                    surName = "Ros"
                }
            });
            //app.CallServiceMethod("MongoDbDataBaseService", "InsertMany", new
            //{
            //    collectionName = "People",
            //    documents = new[]
            //    {
            //        new {
            //            nombre = "Pepito",
            //            surName = "Lucas"
            //        },
            //        new {
            //            nombre = "Juan",
            //            surName = "Moreno"
            //        }
            //    }
            //});
            //Console.WriteLine(app.CallServiceMethod("MongoDbDataBaseService", "Count",
            //    new
            //    {
            //        collectionName = "People"
            //    }));
            //Console.WriteLine(app.CallServiceMethod("MongoDbDataBaseService", "Get",
            //    new
            //    {
            //        collectionName = "People",
            //        id = "5813b9e1ad89ee106091bbfc"
            //    }));
            //foreach (var doc in (IEnumerable<dynamic>)app.CallServiceMethod("MongoDbDataBaseService", "Find",
            //    new
            //    {
            //        collectionName = "People",
            //        skip = 0,
            //        take = 6,
            //        filter = JsonConvert.DeserializeObject(" { 'nombre': { '$in': ['Javier', 'Juan', 'Pepito'] } }"),
            //        order = new { surName = 1 },
            //        projection = new { _id = 0 }
            //    }))
            //{
            //    Console.WriteLine(doc);
            //}
            //app.CallServiceMethod("MongoDbDataBaseService", "UpdateOne",
            //    new
            //    {
            //        collectionName = "People",
            //        filter = new { nombre = "Javier" },
            //        update = JsonConvert.DeserializeObject<dynamic>("{'$set': { 'role': 'admin'}}")
            //    });
            //app.CallServiceMethod("MongoDbDataBaseService", "UpdateMany",
            //    new
            //    {
            //        collectionName = "People",
            //        filter = JsonConvert.DeserializeObject(" { 'nombre': { '$in': ['Javier', 'Juan', 'Pepito'] } }"),
            //        update = JsonConvert.DeserializeObject<dynamic>("{'$set': { 'age': '18'}}")
            //    });
            //app.CallServiceMethod("MongoDbDataBaseService", "DeleteOne",
            //    new
            //    {
            //        collectionName = "People",
            //        filter = JsonConvert.DeserializeObject(" { 'nombre': { '$in': ['Juan', 'Pepito'] } }"),
            //    });


            Console.WriteLine(
                app.CallServiceMethod("service1", "Saludar",
                        JObject.FromObject(new {nombre = 9}))
                    .ToString());

            Console.ReadKey();
        }
    }
}
