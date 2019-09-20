# Security: Resource Based Authorization

We did not protect the update and delete operations, yet.

What we would like to have is an application where:
- Every product has an owner
- Products may be updated and deleted only by their respective owners

In order to achieve this, we have to update both our Backend and our FrontEnd.

## BackEnd
- We'll add a UserName property to the Product class so that we can persist who the owner is
- We will update the Create action to check the UserName property of the product being added
- We will configure the [Resource Based Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-3.0) by creating
    - A ProductOwner [Policy](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-3.0)
    - A ProductOwner [Requirement](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-3.0#requirements)
    - A ProductOwner [Handler](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-3.0#authorization-handlers). This handler will succeed only if the UserName property of the Product being updated/deleted matches the value of the name claim received in the access_token and the claim has been issued by our own IdentityServer Web Application.
- We will check the ProductOwner Policy on Update eventually denying the user the possibility to complete the action if he's not the product's owner
- We will check the ProductOwner Policy on Delete eventually denying the user the possibility to complete the action if he's not the product's owner

## FrontEnd
- We will:
  - update the User Interface of the Product Item by adding a userName property to product
  - add a userName property to current product of the Vue instance setting it to the logged on user name
  - pass the Credentials to our MarketPlaceService during update, just like we did during the Create
  - pass the Credentials to our MarketPlaceService during delete, just like we did during the Create
  - make sure that the Update and Delete buttons are shown only if allowed by:
    - adding a userIsOwner computed property to product-item component
    - showing the update and delete buttons of each product-item only if userIsOwner is true


Let's start by updating our BackEnd Service.

### The Model and the DataBase

We are currently missing a crucial information about our product: the name of the user that created the product. The easiest thing we can do is add a new property on our `Product` model and update the database schema accordingly. Thanks to Entity Framework Migrations, it is going to be easy.

Open the `Product` class (under the `Models` folder) and add a new `UserName` public property  of type `string`:

```cs
public string UserName { get; set; }
```

In order to update the database schema we need to add a migration in the code, then invoke the EF command to update the database.

Open the [Package manager Console(https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell#opening-the-console-and-console-controls) and type:

```
Add-Migration "ProductUserName"
```

Then type:

```
Update-DataBase
```

This will ensure that the model and the database match.


### Update the Create action to check the UserName property of the product being added

We want to retrieve the name of the user and write it on the Product before we save it on the database.

The name of the user has been added to the access_token by our Identity Provider, because when we configured the Api, we added the `Name` [Claim](https://andrewlock.net/introduction-to-authentication-with-asp-net-core/).

```cs
public static IEnumerable<ApiResource> GetApis() {
    return new ApiResource[] {
        new ApiResource("backend", "MarketPlace REST API"){ UserClaims = { JwtClaimTypes.Name } }
    };
}
```

This means that we can retrieve it using the `FindFirst` method of the `User` property in our `Controller` 

Open the `BackEnd/Controllers/ProductsController.cs`, locate the `PostProduct` action and add this line code before you save the product to the database:

```cs
product.UserName = User.FindFirst(c => c.Type == JwtClaimTypes.Name && c.Issuer == "http://localhost:5000").Value;
```
Optionally, you may want to open the db an manually fill the UserName column for each product in your Products table, for example by opening the `SQL Server Object Explorer` in `Visual Studio` and select your `BackEnd` database. 





## Authorization

Now that the code for our Database is ready, let's proceed to enforce Authorization Policies by implementing [Resource Based Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-3.0).

### Custom Policy-Based Authorization

Role authorization and Claims authorization make use of 

- a requirement 
- a handler for the requirement 
- a pre-configured policy

These building blocks allow you to express authorization evaluations in code, allowing for a richer, reusable, and easily testable authorization structure.

An *authorization policy* is made up of one or more *requirements* and registered at application startup as part of the Authorization service configuration, in ```ConfigureServices``` in the ```Startup.cs``` file.

Open the ```Startup.cs``` and add the following code at the bottom of the ```ConfigureServices``` method  

```cs
services.AddAuthorization(options => {
    options.AddPolicy("ProductOwner", policy => policy.Requirements.Add(new ProductOwnerAuthorizationRequirement()));
});
```

Here you can see a `ProductOwner` policy is created with a single requirement, that of being the owner of a product, which is passed as a parameter to the requirement. ```ProductOwnerAuthorizationRequirement``` is a class that we will create in a following step, so don't worry if your code does not compile.

Policies can usually be applied using the ```Authorize``` attribute by specifying the policy name, but not in this case.
Our authorization depends upon the resource being accessed. A `Product` has a `UserName` property. Only the product owner is allowed to update it or delete it, so the resource must be loaded from the product repository before an authorization evaluation can be made. This cannot be done with an `[Authorize]` attribute, as attribute evaluation takes place before data binding and before your own code to load a resource runs inside an action. Instead of *declarative authorization*, the attribute method, we must use *imperative authorization*, where a developer calls an authorize function within their own code.

### Authorizing within your code

Authorization is implemented as a service, ```IAuthorizationService```, registered in the service collection and available via dependency injection for Controllers to access.

```cs
public class ProductsController : ControllerBase {
  private readonly BackEndContext _context;
  private readonly IAuthorizationService _authorizationService;

  public ProductsController(BackEndContext context, IAuthorizationService authorizationService) {
    _context = context;
    _authorizationService = authorizationService;
  }
  //same code as before
}
```

The ```IAuthorizationService``` interface has two methods, one where you pass the resource and the policy name and the other where you pass the resource and a list of requirements to evaluate.
To call the service, load your product within your action then call the `AuthorizeAsync`, returning a `ChallengeResult` if the `Succeeded` property of the result is false. 
Also, add the `[Authorize]` attribute on top of the `PutProduct` method in order to make sure that the user is at least authenticated before proceeding with the action.

### Update Action

This is how the `PutProduct` becomes:

```cs
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> PutProduct(int id, Product product) {
    if (id != product.Id) {
        return BadRequest();
    }
    Product original = await _context.Product.AsNoTracking<Product>().FirstOrDefaultAsync(p => p.Id == id);
    AuthorizationResult authresult = await _authorizationService.AuthorizeAsync(User, original, "ProductOwner");
    if (!authresult.Succeeded) {
        return new ForbidResult();
    }

    _context.Entry(product).State = EntityState.Modified;

    try {
        await _context.SaveChangesAsync();
    } catch (DbUpdateConcurrencyException) {
        if (!ProductExists(id)) {
            return NotFound();
        } else {
            throw;
        }
    }

    return NoContent();
}
```

### Delete Action

This is how the `DeleteProduct` becomes:

```cs
[HttpDelete("{id}")]
[Authorize]
public async Task<ActionResult<Product>> DeleteProduct(int id) {
  var product = await _context.Product.FindAsync(id);
  AuthorizationResult authresult = await _authorizationService.AuthorizeAsync(User, product, "ProductOwner");
  if (!authresult.Succeeded) {
    return new ForbidResult();
  }

  if (product == null) {
    return NotFound();
  }

  _context.Product.Remove(product);
  await _context.SaveChangesAsync();

  return product;
}
```

### Requirements

An authorization requirement is a collection of data parameters that a policy can use to evaluate the current user principal. In our ProductOwner policy the requirement we have is a single parameter, the owner. A requirement must implement ```IAuthorizationRequirement```. This is an empty, marker interface. 
Create a new Folder ```Authorization``` in your MarketPlaceService project.
Add a new ```ProductOwnerAuthorizationRequirement``` class and let the class implement the ```IAuthorizationRequirement``` interface by replacing the file content with the following code :

```cs
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Authorization {
    public class ProductOwnerAuthorizationRequirement : IAuthorizationRequirement {
    }
}
```

A requirement doesn't need to have data or properties.

### Authorization Handlers

An *authorization handler* is responsible for the evaluation of any properties of a requirement. The authorization handler must evaluate them against a provided ```AuthorizationHandlerContext``` to decide if authorization is allowed. A requirement can have multiple handlers. Handlers must inherit ```AuthorizationHandler<T>``` where ```T``` is the requirement it handles.

We will first look to see if the current user principal has a name claim which has been issued by an Issuer we know and trust. If the claim is missing we can't authorize so we will return. If we have a claim, we'll have to figure out the value of the claim, and if it matches the UserName of the product then authorization will be successful. Once authorization is successful we will call context.Succeed() passing in the requirement that has been successful as a parameter.

In the ```Authorization``` folder, add a ```ProductOwnerAuthorizationHandler``` class and replace its content with the following code:

```cs
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BackEnd.Authorization {
  public class ProductOwnerAuthorizationHandler : AuthorizationHandler<ProductOwnerAuthorizationRequirement, Product> {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProductOwnerAuthorizationRequirement requirement, Product resource) {
      if (!context.User.HasClaim(c => c.Type == JwtClaimTypes.Name && c.Issuer == "http://localhost:5000"))
        return Task.CompletedTask;

      var userName = context.User.FindFirst(c => c.Type == JwtClaimTypes.Name && c.Issuer == "http://localhost:5000").Value;

      if (userName == resource.UserName)
        context.Succeed(requirement);

      return Task.CompletedTask;
    }
  }
}
```

Handlers must be registered in the services collection during configuration. 

Each handler is added to the services collection by using ```services.AddSingleton<IAuthorizationHandler, YourHandlerClass>();``` passing in your handler class.

Open the Startup.cs and add this line of code at the bottom of the ```ConfigureServices``` method:

```cs
//requires using Microsoft.AspNetCore.Authorization;
//requires using BackEnd.Authorization;
services.AddSingleton<IAuthorizationHandler, ProductOwnerAuthorizationHandler>();
```

Save everything and ensure it compiles.

We are now ready to move to the FrontEnd.

## Update the Products Component, Details and Delete View

First of all, let's see if we can speed up our process a bit. We have three pages (Home, Details and Delete) that basically use the same layout: a card with the product information. The only thing that changes are which buttons to show. This means that we will have to update the UI and the logic of three components (Products, Details and Delete), writing  the same code three times. This situation is less than ideal, so let's refactor the `card` into its own component and let's make it so that we can configure which buttons to show depending on the view.

## The Product Component

In the `src/components` folder, create a new file `Product.vue`.
Scaffold the usual vue template with `<template>`, `<script>` and `<style>`.
Now cut the `card` of the `Products` component and paste it in the `Product` component.

```html
<template>
  <v-card elevation="10">
    <v-card-title>{{ product.id }} - {{ product.name }} </v-card-title>
    <v-card-text>
    <p>{{ product.description }}</p>
    <p>{{ product.price }}</p>
    </v-card-text>
    <v-card-actions>
      <v-btn icon :to="{name: 'details', params: {id: product.id}}"><v-icon>mdi-card-bulleted</v-icon></v-btn>
      <v-btn icon :to="{name: 'update', params: {id: product.id}}"><v-icon>mdi-pencil</v-icon></v-btn>
      <v-btn icon :to="{name: 'delete', params: {id: product.id}}"><v-icon>mdi-delete</v-icon></v-btn>
    </v-card-actions>
  </v-card>
</template>
```

This time, instead of loading the product by asking it to the datalayer, we will accept it as a [prop](https://vuejs.org/v2/guide/components-props.html). So the `<script>` becomes

```html
<script>
export default {
    props: {
        product : Object
    }
}
</script>
```

Now let's use the component from within the `Products` component.
- The `<template>` becomes

```html
<template>
  <v-row>
    <v-col
     v-for="product in products" :key="product.id"
     cols="12" md="4"
    >
      <product :product="product" />
    </v-col>
  </v-row>
</template>
```

-The `<script>` becomes

```html
<script>
import datalayer from '@/datalayer'
import Product from "@/components/Product.vue"

export default {
  components: {
    Product
  },
  data () {
    return {
      products: []
    }
  },
  async created () {
    this.products = await datalayer.getProducts()
  }
}
</script>
```

Save and verify that the home view still works as before.

Because we want to use the card from within the `Details` and `Delete` views as well, we need to be able to configure which buttons to show.
- The `Products` Component used in the `Home` View will configure the `Product` to show: 
  - A button to navigate to the `Details` View
  - A button to navigate to the `Update` View
  - A button to navigate to the `Delete` View
- The `Details` View  will configure the `Product` to show:
  - A button to navigate to the `Update` View
  - A button to navigate to the `Delete` View
- The `Delete` View  will configure the `Product` to show:
  - A button to actually delete the product

Let's create some `Boolean` `props` into the `Product` component:

```js
details: {type: Boolean, default: false},
update: {type: Boolean, default: false},
requestdelete: {type: Boolean, default: false},
confirmdelete: {type: Boolean, default: false}
```

Let's use the props to show the corresponding buttons.

```html
<v-card-actions>
  <v-btn icon :to="{name: 'details', params: {id: product.id}}" v-if="details===true"><v-icon>mdi-card-bulleted</v-icon></v-btn>
  <v-btn icon :to="{name: 'update', params: {id: product.id}}" v-if="update===true"><v-icon>mdi-pencil</v-icon></v-btn>
  <v-btn icon :to="{name: 'delete', params: {id: product.id}}" v-if="requestdelete===true"><v-icon>mdi-delete</v-icon></v-btn>
  <v-btn @click="deleteProduct" color="warning" v-if="confirmdelete===true">DELETE PRODUCT</v-btn>
</v-card-actions>
```

Let's also transfer the `deleteProduct` method from the `Delete` view to the `Product` component.

```js
deleteProduct () {
    datalayer.deleteProduct(+this.product.id)
    this.$router.push({name: 'home'})
}
```

Now let's have the `Products` component [pass the values](https://vuejs.org/v2/guide/components-props.html#Passing-a-Boolean) it needs to show. We don't need to pass the rest, as they are already `false`.

```html
<product :product="product" details update requestdelete />
```

Let's repeat for the `Details` and `Delete` Views.

The `<template>` section of the `src/views/Details.vue` view becomes:

```html
<template>
  <v-row>
    <v-col cols="sm">
      <product :product="product" update requestdelete />
    </v-col>
  </v-row>
</template>
```

while the `<script>` section becomes

```js
<script>
import datalayer from '@/datalayer'
import Product from "@/components/Product.vue"
export default {
  components: {
    Product
  },
  data () {
    return {
      product: {
        id: 0,
        name: '',
        description: '',
        price: 0
      }
    }
  },
  async created () {
    this.product = await datalayer.getProductById(+this.$route.params.id)
  }
}
</script>
```

The `<template>` section of the `src/views/Delete.vue` view becomes

```html
<template>
  <v-row>
    <v-col cols="sm">
      <product :product="product" confirmdelete />
    </v-col>
  </v-row>
</template>
```

The `<script>` section becomes

```js
<script>
import datalayer from '@/datalayer'
import Product from "@/components/Product.vue"
export default {
  components: {
    Product
  },
  data () {
    return {
      product: {
        id: 0,
        name: '',
        description: '',
        price: 0
      }
    }
  },
  async created () {
    this.product = await datalayer.getProductById(+this.$route.params.id)
  }
}
</script>
```

We're done refactoring. Everything should still work exactly as before, so we can (finally!) proceed to 
- Show the owner name on each product
- Show the delete and update buttons only if the user is authorized 

### The product owner name

Let's show the owner of each product. Open the `Product.vue` component under the `src\components` folder and replace this code

```html
<v-card-text>
  <p>{{ product.description }}</p>
  <p>{{ product.price }}</p>
</v-card-text>
```

with the following

```
<v-card-text>
  <p>{{ product.description }}</p>
  <p>{{ product.price }}</p>
  <p>{{ product.userName }}</p>
</v-card-text>
```

If you have updated the database content with user names for the products, you should see them on each view, now.

Now we want to show the buttons only if the product is owned by the current user. In order to do that we have to use the `authenticationManager` to get who the current user is. We will use this information to to compare it with each product's `userName` property in the view template. Luckily we have written our code in a mixin, so we can import that to already include most of the logic we need.

In the `Product` component we will test if the `product.userName` is equal to the `user.name`.

## Import the `checksec` mixin to retrieve the current user

Open the `HomeView.vue` component under your `src/components` folder, locate the `<script>` tag and import the applicationUserManager constant by adding the followin line:

```js
import checkSecMixin from "@/checksec"
```

Add the mixin to a new `mixins` array:

```
  mixins:[checkSecMixin],
```

We can now update the template. Locate the following code

```html
<v-card-actions>
  <v-btn icon :to="{name: 'details', params: {id: product.id}}" v-if="details===true"><v-icon>mdi-card-bulleted</v-icon></v-btn>
  <v-btn icon :to="{name: 'update', params: {id: product.id}}" v-if="update===true"><v-icon>mdi-pencil</v-icon></v-btn>
  <v-btn icon :to="{name: 'delete', params: {id: product.id}}" v-if="requestdelete===true"><v-icon>mdi-delete</v-icon></v-btn>
  <v-btn @click="deleteProduct" color="warning" v-if="confirmdelete===true">DELETE PRODUCT</v-btn>
</v-card-actions>
```

and update it like this:

```html
<v-card-actions>
  <v-btn icon :to="{name: 'details', params: {id: product.id}}" v-if="details===true"><v-icon>mdi-card-bulleted</v-icon></v-btn>
  <v-btn icon :to="{name: 'update', params: {id: product.id}}" v-if="update===true && user.name===product.userName"><v-icon>mdi-pencil</v-icon></v-btn>
  <v-btn icon :to="{name: 'delete', params: {id: product.id}}" v-if="requestdelete===true && user.name===product.userName"><v-icon>mdi-delete</v-icon></v-btn>
  <v-btn @click="deleteProduct" color="warning" v-if="confirmdelete===true && user.name===product.userName">DELETE PRODUCT</v-btn>
</v-card-actions>
```

If you run the application, you should see the update and delete button only on products created by the logged on user.

### Pass the credentials during Update and Delete

Now let's proceed to update our `datalayer`: we need to pass the credentials during the update and delete, to make sure that our service can recognize the user by extracting the user name from the token.

Modify the `updateProduct` method of your `datalayer.js` file as follows:

```js
async updateProduct (id, product) {
  const user = await applicationUserManager.getUser()
  return fetch(`${this.serviceUrl}/${id}`, {
    method: 'PUT',
    body: JSON.stringify(product),
    headers: new Headers({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + (user ? user.access_token : '')
    })
  })
}
```

The `deleteProduct` method becomes:

```js
async deleteProduct (id) {
  const user = await applicationUserManager.getUser()
  return fetch(`${this.serviceUrl}/${id}`, {
    method: 'DELETE',
    headers: new Headers({
      'Authorization': 'Bearer ' + (user ? user.access_token : '')
    })
  })
}
```

If you run the application, you should be able to update and delete a product only if the product owner is the currently logged on user.

You may have noticed that the `Product` component does not get refreshed whenever we navigate back `Home` after a delete or update. This is due to a little mess we made with the `state` of our data. There are different components using and updating the data and it starts to get complicated to follow what happens when.

We'll try to fix the [State Management](https://vuejs.org/v2/guide/state-management.html) in the next lab, using [vuex](https://github.com/vuejs/vuex)

