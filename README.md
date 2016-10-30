**English version comming soon.**

Este fichero readme está creado de fragmentos de conversaciones por chat, mis disculpas por la informalidad.
**Versión formal de este fichero, próximamente.**

## Historia 

Este proyecto sale de una pregunta que hice a unos amigos, el mensaje que envié fue este, literal:

>Estoy pensando en hacerme una cosa y previendo que ya estará hecho he decidido escribir antes a unos cuantos para ver si conocéis algo que lo haga.
Tras hacer unas pruebas con MongoDb y conocer también Firebase (de Google) se me ha ocurrido hacerme una pequeña librería para acceder a una base de datos MongoDb desde un cliente JavaScript, a través de peticiones ajax a un servidor. Parecido a lo que ofrece Firebase pero trabajando contra un MongoDb. Tener acciones InsertOne, UpdateOne, Find, ... Hasta ahí todo es fácil, con un controlador MVC (o usar NancyFX ya veré) pongo acciones para cada una de la operaciones y paso el payload de la petición a las llamadas a MongoDb según corresponda en cada acción. En cierto modo sería emular un poco Firebase pero con servidor propio. 
Lo que quiero añadir hacer es que puedas definir reglas de negocio a la hora de realizar operaciones contra la base de datos, por ejemplo que antes de insertar en la colección "Posts" asigne automáticamente el id del usuario actual en el campo "userId":

```` C#
public void beforeInsertPost(dynamic post) {
    post.userId = _currentUser.Id;
}
````

>Otra cosa podría ser establecer una proyección por defecto a una colección dependiendo del tipo de usuario que seas, por ejemplo ocultar el precio de los productos si no eres admin:

```` C#
public void onProjectProduct(dynamic projection) {    
    proyection.price = _currentUser.hasRole("admin")? 1: 0;
}
````

>Y cosas así.

>La idea es que puedas crearte un nuevo proyecto ASP.NET  y con poca cosa puedas acceder a una base de datos desde el client-side y poco a poco vayas configurando reglas de negocio. 

>En algún sitio poner:

```` C#
CreateDataBaseAccess("mongo://blabla", configuration);
````

>Donde configuration es un objeto donde están definidas las reglas de negocio.

>Con esto no tendrías que programar un solo controlador MVC ni WebApi, y desde el cliente, con javascript y sabiendo algo de mongodb, puedes tirar a coger datos:

```` javascript
miJsMongoDbAccess.collection("products").find({ amount: { $gte: 50 } }).then((response) => balbla).catch((reason) => blabla);
````

>Mi pregunta es, ¿conocéis algo que cubra esto o que se asemeje un poco?


Básicamente lo que quería era poder crear el backend de una aplicación de forma rápida, sin tener que escribir mucho boilerplate.
La cosa es que estás en tu casa y se te ocurre una chorrada de app, te creas el proyecto web en Visual studio, añades una referencia al proyecto, y puedes ir guardando y consultando cosas desde javascript, luego si vas a más pues puedes añadir reglas de negocio para controlar cosas y tal.

## Primer preview

Te creas un nuevo proyecto web en Visual Studio, añades la referencia a QuickApp, vas al Startup.cs y añades un servicio QuickApp y configuras el servicio de MongoDb:

```` C#
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    // QuickApp ...
    services
        .AddQuickApp()
        .AddMongoService( // Añade el servicio MongoDb a QuickApp
            "mongodb://localhost:27017", // Conection string
            "prueba" // Nombre de la base de datos.
        );
}
````

Luego vas a método Configure y añades una ruta al MVC:

```` C#
app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");

        routes.AddQuickAppRoute("routePrefix"); // Habilita la ruta /routePrefix/{servicioAñadidoQuickApp}/{accion}
    });
````

Vas al site.js y ya puedes tirar del tema:

```` javascript

$("#quickAppButton").click(function () {

    var insertData = {
        collectionName: "People",
        document: { name: "Javier", surname: "Ros" }
    };

    $.ajax("/qa/mongodb/InsertOne", {
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        method: "POST",
        data: JSON.stringify(insertData)
    }).success(function (data) {

        var findData = {
            collectionName: "People",
            filter: {}
        };
        $.ajax("/qa/mongodb/Find",
            {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                method: "POST",
                data: JSON.stringify(findData)
            }).success(function (data) {
                for (var i = 0, j = data.length; i < j; i++) {
                    alert(data[i].name + " " + data[i].surname);
                }
            });
    });
});
````

Al final, todo se basa en una acción de un controlador mvc, el cual recibe un nombre de servicio y un nombre de un método. En el payload van los argumentos que pueda necesitar ese método. Por reflexión llama al método del servicio pasándole los argumentos del payload.
Los servicios que se ejecutan deben haberse añadido a una colección interna, si no están ahí no se pueden llamar.

Si se hace un HTTP POST /QuickApp/servicioChachi/SuperMetodo { param1: 0, param2: ""}, se buscará un servicio "servicioChachi", en el que se busca un método "SuperMetodo" que acepte los parámetros "param1" y "param2". Si existe lo ejecuta y devuelve el resultado.

El paquete viene con un servicio para MongoDb con las cosas más comunes, InsertOne, InsertMany, Find, Count, Update, ... se registra ese servicio con un nombre, por defecto "mongodb", y ya se puede hacer uso de él.

Como cosas a hacer, a priori, tengo:

- Añadir una forma fácil para interceptar la llamada a los métodos, antes y después, para poder meter reglas de negocio.
- Intentar añadir un servicio para otra base de datos para poner a prueba la versatilidad y viabilidad del proyecto.
- Facilitar el tema de autenticación en web (sin usar Identity de los cojones, ni EF), añadir autenticación por cookies, habilitando algún servicio para gestionar usuarios.
- Establecer una unión sencilla entre la gestión de usuarios (y otras entidades que puedan ser comunes, como configuración y tal) y cada servicio de cada base de datos para poder persistirlos.
- Oauth.

## Interceptores

Se pueden añadir interceptores de tres formas:

* Añadir una acción que se ejecuta en un momento dado (Moment) cuando se llama a una acción:

```` C#
services
    .AddQuickApp()
    .AddMongoService("mongodb://localhost:27017", "QuickAppTest", "mongodb")
    .AddInterceptor("mongodb", "InsertOne", Moment.Before, context =>
    {
        context.Arguments.document.name  += " 2";
        context.Arguments.document.userId = 44;
    })
````

* Añadir un objeto de un tipo que implementa una interface IServiceMethodCallInterceptor:

```` C#
public class MongoInterceptorImplementingInterface : IServiceMethodCallInterceptor
{
    public void Intercept(Moment moment, CallContext context)
    {
        if (context.MethodName == "Find" && moment == Moment.After)
        {
            context.Result.Add(new { name = "From", surname = "Class Interceptor" });
        }
    }
}

services
    .AddQuickApp()
    .AddMongoService("mongodb://localhost:27017", "QuickAppTest", "mongodb")
    .AddInterceptor("mongodb", new MongoInterceptorImplementingInterface())
````

* Añadir un objeto de un tipo que hereda de InterceptorByMethodName y escribir los interceptores con nombre "Moment" + "MethodName"

```` C#
public class MongoInterceptorUsingMethodsNames : InterceptorByMethodName
{
    public void AfterFind(CallContext context)
    {
        context.Result.Add(new { name = "From", surname = "Class by method Interceptor" });
    }

    public void OnExceptionFind(CallContext context)
    {
        throw blabla();
    }
}

services
    .AddQuickApp()
    .AddMongoService("mongodb://localhost:27017", "QuickAppTest", "mongodb")
    .AddInterceptor("mongodb", new MongoInterceptorUsingMethodsNames());
````

Moment es un enumerado de After, Before y OnException.
