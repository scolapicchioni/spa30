# FrontEnd: Connecting with the BackEnd

In this lab we're going connect our two projects to each other.

Our client will issue http requests to our server and it will handle the results to update the model. Vue will take care of updating the UI.

Let's start by our `frontend` project.

We're going to replace our old datalayer with a new one that uses the [fetch API](https://developers.google.com/web/updates/2015/03/introduction-to-fetch), [Promises](https://developers.google.com/web/fundamentals/primers/promises) and [async functions](https://developers.google.com/web/fundamentals/primers/async-functions).

### Modify the JavaScript datalayer to issue http requests

Under the `src` folder of the `frontend` project, open `datalayer.js` in Visual Studio Code.

We don't need a `products` array anymore, so we will remove it.
What we do need is a property to store the address of the service.

In the `datalayer` constant, remove 

```js
products: [
  {id: 1, name: 'WIN-WIN survival strategies', description: 'Bring to the table win-win survival strategies to ensure proactive domination.', price: 12345},
  {id: 2, name: 'HIGH level overviews', description: 'Iterative approaches to corporate strategy foster collaborative thinking to further the overall value proposition.', price: 2345},
  {id: 3, name: 'ORGANICALLY grow world', description: 'Organically grow the holistic world view of disruptive innovation via workplace diversity and empowerment.', price: 45678},
  {id: 4, name: 'AGILE frameworks', description: 'Leverage agile frameworks to provide a robust synopsis for high level overviews', price: 9876}
]
```
  
and replace it with

```js
serviceUrl: 'https://localhost:44361/api/products'
```

**Note: Your port number may vary, make sure to use the port assigned by Visual Studio. If you don't know which one it is, open tour `BackEnd` project in Visual Studio, right click the project name in the `Solution Explorer`, select `Properties`, select the `Debug` tab and check the address listed next to the `Enable SSL` checkbox**


Now let's change the `getProducts` method by fetching a request to our service, asynchronously waiting for the result and returning the response parsed as json. Don't forget to turn the method into an `async function`.

```js
async getProducts () {
  const response = await fetch(this.serviceUrl)
  return response.json()
}
```

The `getProductById` will be very similar. The only difference is the address to fetch, which will contain the `id` of the product to retrieve:

```js
async getProductById (id) {
  const response = await fetch(`${this.serviceUrl}/${id}`)
  return response.json()
}
```

The `insertProduct` will pass the `fetch` method not only the url to call, but also an object with three properties:
- `method` - set to `POST`
- `body` - set to a json string representing the `product` parameter
- `headers` - set to an instance of the `Header` class, with a `Content-type` property set to `application/json`

```js
async insertProduct (product) {
  const response = await fetch(this.serviceUrl, {
    method: 'POST',
    body: JSON.stringify(product),
    headers: new Headers({
      'Content-Type': 'application/json'
    })
  })
  return response.json()
}
```

The `updateProduct` will be very similar. The only differences are
- the address to fetch, which will contain the `id` of the product to update
- the `method` option, set to `PUT`

```js
async updateProduct (id, product) {
  return fetch(`${this.serviceUrl}/${id}`, {
    method: 'PUT',
    body: JSON.stringify(product),
    headers: new Headers({
      'Content-Type': 'application/json'
    })
  })
}
```

The `deleteProduct` will also fetch an url containing the `id` of the product to delete. The `method` will be `delete` and we won't need neither the `body` nor the `header`.

```js
async deleteProduct (id) {
  return fetch(`${this.serviceUrl}/${id}`, {
    method: 'DELETE'
  })
}
```

By starting both project, you will notice an error in the browser console: 

```
Failed to load https://localhost:44361/api/products: No 'Access-Control-Allow-Origin' header is present on the requested resource. Origin 'http://localhost:8080' is therefore not allowed access. If an opaque response serves your needs, set the request's mode to 'no-cors' to fetch the resource with CORS disabled.
```

This happens because our server does not allow [Cross Origin Requests (CORS)](https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0). Let's proceed to modify our server project.

- Open `Startup.cs`
- In the `ConfigureServices` method, add the following code:

```cs
services.AddCors(options =>
    options.AddPolicy("frontend", builder =>
        builder.WithOrigins("http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod()
    )
);
```

- In the `Configure` method, **BEFORE**  

```
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});
```

add the following code:

```cs
app.UseCors("frontend"); 
```

### The FrontEnd Views

Now we need to modify our Views to asyncronously wait for our datalayer.

Let's start with our `src/components/Products.vue` component. The `created` method has to become and `async function` and it has to `await` the `getProducts`.

```js
async created () {
  this.products = await datalayer.getProducts()
}
```

Same goes for the `src/views/Details.vue` view, where the `created` method has to become `async functions` and has to `await` for the `getProductById` method of our `datalayer` object.

```js
async created () {
  this.product = await datalayer.getProductById(+this.$route.params.id)
}
```

The `insertProduct` of the `src/views/Create.vue` view will also turn into an `async function`:

```js
async insertProduct () {
  await datalayer.insertProduct(this.product)
  this.$router.push('/')
}
```

The `created` and `updateProduct` of the `scr/views/Update.vue` view will get the same facelift.

```js
async created () {
  this.product = await datalayer.getProductById(+this.$route.params.id)
},
methods: {
  async updateProduct () {
    await datalayer.updateProduct(+this.$route.params.id, this.product)
    this.$router.push('/')
  }
}
```

Last but not least, let's not forget `src/views/Delete.vue`

```js
async created () {
    this.product = await datalayer.getProductById(+this.$route.params.id)
  },
  methods: {
    async deleteProduct () {
      await datalayer.deleteProduct(+this.$route.params.id)
      this.$router.push({name: 'home'})
    }
  }
```

Save and verify that the client can send and receive data to and from the server.

We did not implement any security yet. Our next lab will start with setup and configure a new project that will act as an *Authentication Server*. We will then protect the Create operation and we will use the Authentication Server to authenticate the user and have the client gain access to the protected operation.

Go to `Labs/Lab06`, open the `readme.md` and follow the instructions thereby contained.   