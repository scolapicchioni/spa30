# Backend: Web API with ASP.NET Core 3.0 and Visual Studio 2019

In this lab we're going to build a [REST](https://www.restapitutorial.com/lessons/whatisrest.html#) service using [ASP.NET Core 3.0 Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0).

## Create a Web API with ASP.NET Core

Here is the API that you'll create:

| API                       | Description                | Request body           | Response body     |
| ------------------------- | -------------------------- | ---------------------- | ----------------- |
| GET /api/products	        | Get all products   	       | None	                  | Array of products |
| GET /api/products/{id}    | Get a product by ID        | None                   | Product           |
| POST /api/products        | Add a new product          | Product                | Product           |
| PUT /api/products/{id}    | Update an existing product | Product                |                   |	
| DELETE /api/products/{id} | Delete a product           | None. No request body- | None              |

The client submits a request and receives a response from the application. Within the application we find the controller, the model, and the data access layer. The request comes into the application's controller, and read/write operations occur between the controller and the data access layer. The model is serialized and returned to the client in the response.

The **client** is whatever consumes the web API (browser, mobile app, and so forth). We aren't writing a client in this tutorial. We'll use [Postman](https://www.getpostman.com/apps) to test the app. We will write the client in the following lab.

A **model** is an object that represents the data in your application. In this case, the only model is a Product item. Models are represented as simple C# classes (POCOs).

A **controller** is an object that handles HTTP requests and creates the HTTP response. This app will have a single controller.

### Prerequisites
Install the following:

- [.NET Core 3.0.0 SDK](https://www.microsoft.com/net/core) or later.
- [Visual Studio 2019](https://www.visualstudio.com/downloads/) version 16.4 or later with the ASP.NET and web development workload.

### Create the project

- Open Visual Studio.
- In the `Create New Project` window, select the `ASP.NET Core Web Application` project template. 
    - Go to the folder `Lab04\Start\MarketPlace`.
    - Name the Solution `BackEnd`. 
    - Name the Project `BackEnd`
    - Select `Create`
- In the `Create a new ASP.NET Core Web Application` window:
    - Select `.NET Core`
    - Select `ASP .NET Core 3.0`
    - Select the `API` template
    - Leave `No Authentication`. 
    - Ensure that the `Configure for Https` checkbox is selected
    - Do not check `Enable Docker Support`.
    - Click on `Create`

### Add a model class

A **model** is an object that represents the data in your application. In this case, the only model is a `Product` item, whose properties are `Id` *(int)*, `Name` *(string)*, `Description` *(string)* and `Price` *(decimal)*.

Add a folder named `Models`. 
- In `Solution Explorer`, right-click the project. 
- Select `Add` > `New Folder`. 
- Name the folder `Models`.

Note: You can put model classes anywhere in your project, but the `Models` folder is used by convention.

Add a `Product` class. 
- Right-click the `Models` folder and
- Select `Add` > `Class`. 
- Name the class `Product` 
- Select `Add`.

Replace the generated code with:

```cs
namespace BackEnd.Models {
    public class Product {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
```

### Generate the Controller and the DbContext

Ensure that the project compiles first, then let the wizard generate the `Controller` and the `DbContext` for you.
- In `Solution Explorer`, right-click the `Controllers` folder. 
- Select `Add` > `Controller`. 
- Select  `API Controller with actions, using Entity Framework`
- Click `Add`
- As `Model class` select `Product (BackEnd.Models)`
- As `Data context class`, click on the `+` button
- Name the context `BackEnd.Models.BackEndContext`
- Name the Controller `ProductsController`
- Click `Add`

After a while you should see some new files. 

### The database context

The database context is the main class that coordinates [Entity Framework](https://docs.microsoft.com/en-us/ef/core/) functionality for a given data model. This class is created by deriving from the `Microsoft.EntityFrameworkCore.DbContext` class.

The wizard added a `Data` folder in which you can find the `BackEndContext` class.

```cs
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models {
    public class BackEndContext : DbContext {
        public BackEndContext (DbContextOptions<BackEndContext> options) : base(options) {
        }

        public DbSet<BackEnd.Models.Product> Product { get; set; }
    }
}
```

The wizard also configured the context and added it as a Service using the [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0) features of `ASP.NET Core`.
Open the [`Startup.cs`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.0) file and you will see a method where this happens:

```cs
public void ConfigureServices(IServiceCollection services) {
    services.AddControllers();

    services.AddDbContext<BackEndContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("BackEndContext")));
}
```
The `BackEndContext` connection string is [configured](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.0) in the `appsettings.json` file, as per [Default](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.0#default-configuration):

```json
"ConnectionStrings": {
    "BackEndContext": "Server=(localdb)\\mssqllocaldb;Database=BackEndContext-7ded1dbd-82d6-4b02-8451-a878c2bcce44;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
```

### The Controller

The wizard took care of the [Controller](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0) by generating a class that derives from [ControllerBase](https://docs.microsoft.com/dotnet/api/microsoft.aspnetcore.mvc.controllerbase), which provides many properties and methods that are useful for handling HTTP requests.

The `Microsoft.AspNetCore.Mvc` namespace provides attributes that can be used to configure the behavior of web API controllers and action methods.

The [ApiController](https://docs.microsoft.com/dotnet/api/microsoft.aspnetcore.mvc.apicontrollerattribute) attribute was applied to the controller class to enable the following API-specific behaviors:
- [Attribute routing requirement](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0#attribute-routing-requirement)
- [Automatic HTTP 400 responses](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0#automatic-http-400-responses)
- [Binding source parameter inference](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0#binding-source-parameter-inference)
- [Multipart/form-data request inference](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0#multipartform-data-request-inference)
- [Problem details for error status codes](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0#problem-details-for-error-status-codes) 

The controller and every action were mapped to a route through the use of [routing](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.0) system, in particular [attribute routing](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.0#attribute-routing) and [attribute routing using http verbs attributes](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.0#attribute-routing-with-httpverb-attributes).

The Controller makes use of the DI container by explicitly declaring its dependency on the `BackEndContext` in its constructor.

```cs
public class ProductsController : ControllerBase
{
    private readonly BackEndContext _context;

    public ProductsController(BackEndContext context)
    {
        _context = context;
    }
    //code omitted for brevity
}
```

## Getting Product items

We got two `GetProduct` methods:

```cs
// GET: api/Products
[HttpGet]
public async Task<ActionResult<IEnumerable<Product>>> GetProduct() {
    return await _context.Product.ToListAsync();
}

// GET: api/Products/5
[HttpGet("{id}")]
public async Task<ActionResult<Product>> GetProduct(int id) {
    var product = await _context.Product.FindAsync(id);

    if (product == null) {
        return NotFound();
    }

    return product;
}
```

It returns an `Task<ActionResult<IEnumerable<Product>>>`. MVC automatically serializes the list of `Product` to JSON and writes the JSON into the body of the response message. The response code for this method is 200, assuming there are no unhandled exceptions. (Unhandled exceptions are translated into 5xx errors.)

Here is an example HTTP response for the first `GetProduct()` method:

```
HTTP/1.1 200 OK
   Content-Type: application/json; charset=utf-8
   Server: Microsoft-IIS/10.0
   Date: Thu, 18 Jun 2015 20:51:10 GMT
   Content-Length: 82

   [{"Id":1,"Name":"Product 1","Description":"First Sample Product", "Price" : 1234}]
```

The second `GetProduct(int id)` method returns a `Task<ActionResult<Product>>` type:

- A 404 status code is returned when the product represented by id doesn't exist in the underlying data store. The `NotFound` convenience method is invoked as shorthand for return `new NotFoundResult();`.
- A 200 status code is returned with the Product object when the product does exist. The `Ok` convenience method is invoked as shorthand for return `new OkObjectResult(product);`.

### Create Action

As for REST standards, the action that adds a product to the database is bound to the `POST http verb`.

```cs
[HttpPost]
public async Task<ActionResult<Product>> PostProduct(Product product)
{
    _context.Product.Add(product);
    await _context.SaveChangesAsync();

    return CreatedAtAction("GetProduct", new { id = product.Id }, product);
}
```

The `PostProduct()` is an `HTTP POST` method, indicated by the `[HttpPost]` attribute. The `[ApiController]` attribute at the top of the controller declaration tells MVC to get the value of the `Product` item from the body of the HTTP request.

The `CreatedAtRoute` method:

- Returns a 201 response. HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
- Adds a Location header to the response. The Location header specifies the URI of the newly created Product item. See [10.2.2 201 Created](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html).
- Uses the `GetProduct` named route to create the URL. The `GetProduct` named route is defined in `GetProduct(int id)`

Thanks to the `[ApiController]` attribute, the request is checked against the validation engine validation and in case of a BadRequest the default response type for an HTTP 400 response is ValidationProblemDetails. The following request body is an example of the serialized type:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "|7fb5e16a-4c8f23bbfc974667.",
  "errors": {
    "": [
      "A non-empty request body is required."
    ]
  }
}
```

The ValidationProblemDetails type:

- Provides a machine-readable format for specifying errors in web API responses.
- Complies with the RFC 7807 specification.

### Update

Update is similar to Create, but uses `HTTP PUT`. 

```cs
[HttpPut("{id}")]
public async Task<IActionResult> PutProduct(int id, Product product)
{
    if (id != product.Id)
    {
        return BadRequest();
    }

    _context.Entry(product).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!ProductExists(id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }

    return NoContent();
}
```


The response is 204 (No Content). According to the HTTP spec, a PUT request requires the client to send the entire updated entity, not just the deltas. To support partial updates, use HTTP PATCH.

### Delete

The Delete uses `HTTP DELETE` verb and expects an `id` in the address. 

```cs
[HttpDelete("{id}")]
public async Task<ActionResult<Product>> DeleteProduct(int id)
{
    var product = await _context.Product.FindAsync(id);
    if (product == null)
    {
        return NotFound();
    }

    _context.Product.Remove(product);
    await _context.SaveChangesAsync();

    return product;
}
```

It returns 
- A 200 (Ok) with the deleted product if successful
- A 404 (Not Found) if the id is not found in the database

### Generate migrations and database

The database has not been created. We're going to use [Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/) to generate the DB and update the schema on a later Lab.

To add an initial migration, run the following command.

```
Add-Migration InitialCreate
```

Three files are added to your project under the Migrations directory:

- XXXXXXXXXXXXXX_InitialCreate.cs--The main migrations file. Contains the operations necessary to apply the migration (in Up()) and to revert it (in Down()).
- XXXXXXXXXXXXXX_InitialCreate.Designer.cs--The migrations metadata file. Contains information used by EF.
- BackEndContextModelSnapshot.cs--A snapshot of your current model. Used to determine what changed when adding the next migration.

The timestamp in the filename helps keep them ordered chronologically so you can see the progression of changes.

### Update the database
Next, apply the migration to the database to create the schema.

```
Update-Database
```

### Try the Get Actions of the controller

In Visual Studio, press `CTRL+F5` to launch the app. Visual Studio launches a browser and navigates to http://localhost:port/weatherforecast, where port is a randomly chosen port number. Navigate to the Products controller at `http://localhost:port/api/products`.

You should see an empty JSON array. 

### Install Postman

This tutorial uses Postman to test the web API.

- Install [Postman](https://www.getpostman.com/downloads/)
- Start the web app.
- Start Postman.
- Disable SSL certificate verification
- From File > Settings (*General tab), disable SSL certificate verification.

### Use Postman to send a Create request

- Set the HTTP method to POST
- Select the Body radio button
- Select the raw radio button
- Set the type to JSON
- In the key-value editor, enter a product item such as

```json
{
  "Name":"Product 1",
  "Description":"New Product 1",
  "Price":111
}
```

Select `Send`

Select the `Headers` tab in the lower pane and copy the `Location` header.

You can use the `Location` header URI to access the resource you just created.

If you add multiple products and try to navigate to the products again, you should see an array with all the products you created.

For example:

```
HTTP/1.1 200 OK
Transfer-Encoding: chunked
Content-Type: application/json; charset=utf-8

[{"id":1,"name":"Product 1","description":"First Sample Product","price":1234.0},{"id":2,"name":"Product 2","description":"Second Sample Product","price":2345.0},{"id":3,"name":"Product 3","description":"Third Sample Product","price":3456.0},{"id":4,"name":"Product 4","description":"Fourth Sample Product","price":4567.0},{"id":5,"name":"Product 1","description":"First Sample Product","price":1234.0},{"id":6,"name":"Product 2","description":"Second Sample Product","price":2345.0},{"id":7,"name":"Product 3","description":"Third Sample Product","price":3456.0},{"id":8,"name":"Product 4","description":"Fourth Sample Product","price":4567.0}]
```

Navigate to `http://localhost:port/api/products/1`

You should see this response:

```
HTTP/1.1 200 OK
Transfer-Encoding: chunked
Content-Type: application/json; charset=utf-8

{"id":1,"name":"Product 1","description":"First Sample Product","price":1234.0}
```

Navigate to `http://localhost:port/api/products/99`

You should see this response header:

```
HTTP/1.1 404 Not Found
Content-Length: 0
```

### Try the Update Action

You can use POSTMAN to test the Update action.

- Set the HTTP method to PUT
- Select the Body radio button
- Select the raw radio button
- Set the type to JSON
- In the key-value editor, enter a product item such as

```json
{
  "Id":1,
  "Name":"Product 111",
  "Description":"Modified description",
  "Price":1111
}
```

Select `Send`


### Try the Delete Action

You can use POSTMAN to test the Delete action.

- Set the HTTP method to DELETE
- Set the address to `http://localhost:port/api/products/1`

Select `Send`

Check that the response contains the first product.
If you call the action to get all the products, you should not see the product with id 1 anymore.

Our service is ready. In the next lab we will setup the client side. 

Go to `Labs/Lab05`, open the `readme.md` and follow the instructions thereby contained.   