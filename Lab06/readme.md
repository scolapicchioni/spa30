# Security: Authentication and Authorization

We have not implemented any security yet. In this lab we are going to setup and configure a new project that will act as an *Authentication Server*. We will then protect the **Create** operation and we will use the Authentication Server to authenticate the user and issue a *token*, then have the client gain access to the protected operation by using such token.
The Authentication Server that we're going to use implements [OAuth 2.0 and IDConnect](https://www.oauth.com/).

## The Authentication Server

We are going to use [Identity Server 4](https://identityserver4.readthedocs.io/en/latest/index.html)

We are going to use the [Templates](https://github.com/IdentityServer/IdentityServer4.Templates)

**NOTE: At the time of the writing the templates have not yet been updated to .NET core 3, so you may want to go to the [GitHub repo](https://github.com/IdentityServer/IdentityServer4.Templates/tree/master/src/IdentityServer4AspNetIdentity) and clone this it, instead of installing the templates on the command line.**

- Open a command prompt and navigate to your `Labs\Lab06\Start\MarketPlace` folder
- If you haven't installed the IdentityServer templates yet, do it by typing the following command:

```
dotnet new -i identityserver4.templates
```

- Create an empty IdentityServer project that uses ASP.NET Identity for user management by typing the following command: 

```
dotnet new is4aspid --allow-scripts
```

- You will get two users: alice and bob - both with password Pass123$.

- Open Visual Studio
- Open the project you just created in the `Lab06\Start\MarketPlace\IdentityProvider` folder

## Identity Provider Configuration

Now that we have a project, we need to cofigure it for our own purposes.

In your project, open the `Config.cs` file located in the root of your project.

Our [Resource](https://identityserver4.readthedocs.io/en/latest/topics/resources.html) has already been configured, so we don't need to change that.

```cs
public static IEnumerable<IdentityResource> GetIdentityResources() {
    return new List<IdentityResource> {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    };
}
```

We do need to configure the ApiResource. 
- We will name it `backend`
- We will describe it as `MarketPlace REST API`
- We will include the `Name` of the user in the access token. We will use the name in a future lab to allow products update and deletion only to the product owner.

```cs
public static IEnumerable<ApiResource> GetApiResources(){
    return new List<ApiResource> {
        new ApiResource("backend", "MarketPlace REST API") {
            // include the following using claims in access token (in addition to subject id)
            //requires using using IdentityModel;
            UserClaims = { JwtClaimTypes.Name }
        }
    };
}
```

The second thing we need to configure is the [Javascript Client](https://identityserver4.readthedocs.io/en/latest/topics/clients.html)
- Locate the `GetClients` method.
- Remove every client except for the last one (`spa`)
- Change the `ClientId` to `frontend`
- Change the `ClientName` will be `MarketPlace JavaScript Client`
- Change the `ClientUri` to `http://localhost:8080`,
- Change the `RedirectUris` to
```cs
{
    "http://localhost:8080",
    "http://localhost:8080/callback",
    "http://localhost:8080/silent",
    "http://localhost:8080/popup"
},
```

- Change the `PostLogoutRedirectUris` to `http://localhost:8080`
- Change the `AllowedCorsOrigins` to `http://localhost:8080`
- Change the `AllowedScopes` to `{ "openid", "profile", "backend" }`

The `GetClients` method should look like the following:

```cs
public static IEnumerable<Client> GetClients() {
    return new[] {
        // SPA client using code flow + pkce
        new Client {
            ClientId = "frontend",
            ClientName = "MarketPlace JavaScript Client",
            ClientUri = "http://localhost:8080",

            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RequireClientSecret = false,

            RedirectUris = {
                "http://localhost:8080",
                "http://localhost:8080/callback",
                "http://localhost:8080/silent",
                "http://localhost:8080/popup",
            },

            PostLogoutRedirectUris = { "http://localhost:8080" },
            AllowedCorsOrigins = { "http://localhost:8080" },

            AllowedScopes = { "openid", "profile", "backend" }
        }
    };
}
```

This project is also configured to use Google Authentication. We can remove it because we're not going to use it.

Open the `Startup.cs`, locate the `ConfigureServices` method and remove the `AddGoogle` call. This code

```cs
services.AddAuthentication()
  .AddGoogle(options =>
  {
      // register your IdentityServer with Google at https://console.developers.google.com
      // enable the Google+ API
      // set the redirect URI to http://localhost:5000/signin-google
      options.ClientId = "copy client ID from Google here";
      options.ClientSecret = "copy client secret from Google here";
  });
```

simply becomes

```cs
services.AddAuthentication();
```

The command we ran to create the project created a SQLite database. If you want to browse it from within Visual Studio, you will need a Visual Studio Extension. 
- In Visual Studio, select `Tools -> Extension And Updates`, 
- Select `Online` 
- Search for `SqLite`. 
- Select the `SQLite / SQL Server Compact Toolbox` 
- Click on `Download`. 
- When the download completes, close Visual Studio to start the installation. 
- When the installation complets, go back to Visual Studio
- Click on `View -> Other Windows -> SQLite / SQL Compact Toolbox`, 
- Click on the `Add SQLite / SQL Compact from current solution` button. 
- You should see a `AspIdUsers.db` Database.
  - Feel free to explore its structure and content. 
  - You will notice the configuration tables used by IdentityServer, together with the tables for the IdentityDbContext.  

In Visual Studio, run the application and test a user login, by navigating to `http://localhost:5000/Account/Login` and using `alice` or `bob` as UserName and `Pass123$` as password. You should see the user correctly logged on.

## Configuring the REST Service

We can now switch to our Web Api project. We need to:
- Configure it to use Identity Server
- Protect the access to the `Create` action to allow only authenticated users

As explained it the [Adding an API](https://identityserver4.readthedocs.io/en/latest/quickstarts/1_client_credentials.html#configuration) tutorial, we need to configure the API.

We need to add the authentication services to DI and the authentication middleware to the pipeline. These will:

- validate the incoming token to make sure it is coming from a trusted issuer
- validate that the token is valid to be used with this api (aka scope)

We now need to add the authentication services to DI and configure "Bearer" as the default scheme. We can do that thanks to the `AddAuthentication` extension method. We then also have to add the JwtBearer access token validation handler into DI for use by the authentication services, throught the invocation of the `AddJwtBearer` extension method, to which we have to configure the `Authority` (which is the http address of our Identity Server) and the `Audience` (which we set in the previous project as `backend`). The Metadata Address or Authority must use HTTPS unless disabled for development by setting `RequireHttpsMetadata=false`.

Open your `Startup` class, locate the `ConfigureServices` method and add the following code:

```cs
services.AddAuthentication("Bearer")
  .AddJwtBearer("Bearer", options => {
      options.Authority = "http://localhost:5000";
      options.RequireHttpsMetadata = false;

      options.Audience = "backend";
  });
```

We also need to add the authentication middleware to the pipeline so authentication will be performed automatically on every call into the host, by invoking the `UseAuthentication` extension method **BEFORE** the `UseMvc` in the `Configure` method of our `Startup` class.

Locate the `Configure` method and add the following code right before the `app.UseMvc()` line:

```cs
app.UseAuthentication();
```

The last step is to protect the `Create` action of our `ProductsController` by using the [Authorize](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/simple?view=aspnetcore-3.0) attribute.

Open your `ProductsController` class, locate the `PostProduct` method and add the `[Authorize]` attribute right before the definition of the method:

```cs
[Authorize]
[HttpPost]
public async Task<ActionResult<Product>> PostProduct(Product product) {
    _context.Product.Add(product);
    await _context.SaveChangesAsync();

    return CreatedAtAction("GetProduct", new { id = product.Id }, product);
}
```

If you use the POSTMAN to invoke the Create action you should get a 401 status code in return. This means your API requires a credential.

The API is now protected by IdentityServer.

## Configuring the Javascript Client

The third part requires the configuration of our client project.

Let's begin by testing if the create still works. Run all the three projects, then try to post a new product using our client application. You should see that the product is not added, while you  should still be able to get the list of all products, and also to get the details, modify and delete a specific product.

What we need to do is to give the user the chance to log in, get the tokens from Identity Server and add the access token to the post request in order to be authorized.
The process of configuring a javascript client is described in the 
[Identity Server Documentation](https://identityserver4.readthedocs.io/en/latest/quickstarts/6_javascript_client.html)

We are going to:
- Add the `oidc-client` library
- Create a `ApplicationUserManager` class extending the [UserManager](https://github.com/IdentityModel/oidc-client-js/wiki) that
    - autoconfigures itself in the constructor
    - provides login and logout functionalities
- Expose a global constant instance of the ApplicationUserManager
- Add a `Login` functionality
    - Add a button to the `App` ViewComponent
    - Handle its click by invoking the `login` method of our `applicationUserManager`
- Implement the LoginCallBack
    - Create a `LoginCallBackView` ViewComponent
    - Invoke the `signinRedirectCallback` method of our `applicationUserManager`
    - Go back to the HomeView
    - Configure the route to the `LoginCallbackView`
- Use the `applicationUserManager` in the `datalayer` to
    - get the access token
    - pass the token in the header of the post request



In order to add the `oidc-client` library, follow the instructions described on the [oidc-client git page](https://github.com/IdentityModel/oidc-client-js): 
open a console window, ensure to navigate to the client project folder, then type

```
npm install oidc-client --save
```

The following steps are:

- Create an `ApplicationUserManager` class extending the [UserManager](https://github.com/IdentityModel/oidc-client-js/wiki) that
    - autoconfigures itself in the constructor
    - provides login and logout functionalities
- Expose a global constant instance of the ApplicationUserManager

Let's start by creating a new file `applicationusermanager.js` in your `src` folder.

- Start by importing the `UserManager` dependency from the `oidc-client` package.
- Create a new class `ApplicationUserManager` that extends `UserManager`
- implement a constructor and invoke the constructor of the base class passing an object with the following properties:
  - authority: 'http://localhost:5000',
  - client_id: 'frontend',
  - redirect_uri: 'http://localhost:8080/callback',
  - response_type: 'code',
  - scope: 'openid profile backend',
  - post_logout_redirect_uri: 'http://localhost:8080'
- implent an `async login` method that
    - asynchronously waits for the `signinRedirect` method
    - invokes the `getUser` method and returns the result
- implement an `async logout` method that invokes the `signoutRedirect` method and returns the result
- Create an instance of the `ApplicationUserManager` class and export it as a default constant

Your `applicationusermanager.js` file should look like this:

```js
import { UserManager } from 'oidc-client'

class ApplicationUserManager extends UserManager {
  constructor () {
    super({
      authority: 'http://localhost:5000',
      client_id: 'frontend',
      redirect_uri: 'http://localhost:8080/callback',
      response_type: 'code',
      scope: 'openid profile backend',
      post_logout_redirect_uri: 'http://localhost:8080'
    })
  }

  async login () {
    await this.signinRedirect()
  }

  async logout () {
    return this.signoutRedirect()
  }
}

const applicationUserManager = new ApplicationUserManager()
export { applicationUserManager as default }

```
Now let's create a `Login` component to encapsulate the ui and the logic to
- Show the user a login button that uses the applicationManager to redirect the user to our Identity Provider to let the user sign in
- Show the user a logout button if the user is already logged on, to give the chance to logout by invoking the applicationManager

### The Login Component

- In the `src/components` folder, add a new `Login.vue` file. 
- Type `vue` and press TAB to let `vetur` scaffold the default template for you:

```html
<template>
  
</template>

<script>
export default {

}
</script>

<style>

</style>
```
Let's start by adding the  `Login` functionality
    - Add a button 
    - Handle its click by invoking the `login` method of our `applicationUserManager`
    
### Add a button to the toolbar of the `App` View

Let's first think about the UI. In the `template` section, add the following code:


```html
  <v-btn icon @click="login">
    <v-icon>mdi-account</v-icon>
  </v-btn>
```

Now it's time to write the logic. We need to make use of our applicationUserManager constant, so the first thing we need to do is to import it from the module.
Locate the `<script>` tag and add this code as first statement:

```js
import applicationUserManager from '../applicationusermanager'
```

Now we need to add a new `async login` method that invokes the `login` of our `applicationUserManager` object and asynchronously wait for the result:

```js
methods: {
    async login () {
      try {
        await applicationUserManager.login()
      } catch (error) {
        console.log(error)
      }
    }
  }
```

Whenever we login, we are sent to the IdentityServer site where we can proceed to enter our credentials. If we get authenticated by the system, we are then redirected to a callback url. This is what we still don't have and that we're going to create next.

As described in the [Identity Server Documentation](https://identityserver4.readthedocs.io/en/latest/quickstarts/6_javascript_client.html)

> This HTML file is the designated `redirect_uri` page once the user has logged into IdentityServer. It will complete the OpenID Connect protocol sign-in handshake with IdentityServer. The code for this is all provided by the UserManager class we used earlier. Once the sign-in is complete, we can then redirect the user back to the main index.html page. 

- To implement the LoginCallBack we need to:
    - Create a `LoginCallBack` View
    - Invoke the `signinRedirectCallback` method of our `applicationUserManager`
    - Go back to the `Home` View
    - Configure the `route` to the `LoginCallback` View  

Should our site be slow, we will also show a message to inform the user that no further actions are necessary, since the page should soon automatically refresh.

In your `src/views` folder, create a `LoginCallback.vue` file.
In the template, show a message informing that the page should refresh.
In the script, start by importing the `applicationUserManager` object from the `applicationusermanager` module.
Then export the default object giving it a `name` of `logincallback-view`. Ensure that the `created` event is handled by asynchronously waiting for the `signinRedirectCallback` method of the `applicationUserManager` object. Remember to also push the HomeView route so that the page is refreshed.

Your `LoginCallback.vue` should look like this:

```html
<template>
  <h3>Please wait, you are being redirected to the Home page</h3> 
</template>

<script>
import applicationUserManager from '../applicationusermanager'

export default {
  async created () {
    try {
      await applicationUserManager.signinRedirectCallback()
      this.$router.push({name: 'home'})
    } catch (e) {
      console.log(e)
    }
  }
}
</script>

<style>

</style>
```  

Configure the callback route.

Open the `src/router.js` file.

Import `LoginCallback` component:

```js
import CallBack from './views/LoginCallBack.vue'
```

and a new route and bind it to the corresponding path and component:
```js
{
  path: '/callback',
  name: 'callback',
  component: CallBack
}
```

Now we need to use the `applicationUserManager` in the datalayer to:
- get the access token
- pass the token in the header of the post request

- Open the `datalayer.js` file in your `src` folder.
- Import the `applicationUserManager` object from the `../applicationusermanager` module.
- Modify the `async insertProduct` method to
    - asynchronously wait for the `applicationUserManager.getUser()` method and put the result into a `user` constant
    - add a new `Authorization` property to the `Headers` object, set to `'Bearer '` followed by the `user.access_token` property (if the `user` constant has a value)

Your `datalayer` should begin with

```js
import applicationUserManager from '../applicationusermanager'
```

and the `insertProduct` should now look like this:

```js
async insertProduct (product) {
  const user = await applicationUserManager.getUser()
  const response = await fetch(this.serviceUrl, {
    method: 'POST',
    body: JSON.stringify(product),
    headers: new Headers({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + (user ? user.access_token : '')
    })
  })
  let result
  if (response.status !== 201) {
    result = response.statusText
  } else {
    result = await response.json()
  }
  return result
}
```

The last thing to do is to add the component to the `App` toolbar.

- Open the `src/App.vue` file
- Add a `<script>` section to import the `Login` component and register it 

```html
<script>
import Login from "@/components/Login.vue";
export default {
  components: {
    Login
  }
}
</script>
```

In the the `<template>` section, add a `<login />` component in the toolbar, between the button to add a new product and the one to navigate to the `about` view. 

```html
<v-btn icon :to="{name: 'about'}">
  <v-icon>mdi-chat</v-icon>
</v-btn>

<login/>

<v-btn icon :to="{name: 'create'}">
  <v-icon>mdi-pencil-plus</v-icon>
</v-btn>

```

Save the files and run the frontend, after making sure that both the backend and the identity provider are running.

Click on the button to logon.
You should get redirected to the IdentityServer site. You should be able to log on using the username and password you used earlier (`alice` and `Pass123$`) and you should briefly see the callback url and then the home page. At that point you should be able to insert a new product.

### Logout

To continue, we're going to 
- Show the `LOGIN` button only if the user is not logged on
- Show a `LOGOUT` button and the user name if the user is logged on

In the `Login.vue` component, we're going to [conditionally render a template](https://vuejs.org/v2/guide/conditional.html#Conditional-Groups-with-v-if-on-lt-template-gt): if the user is not yet authenticated, we will show the "LOGIN" button; else, we will show the user name, a "LOGOUT button and we will bind the click event of the button to a `logout` method. 

In order to do that, we will have to create a `data` function to return a `user` object  with a `name` and a `isAuthenticated` property. We will update this object during creation and at [every change in route](https://router.vuejs.org/guide/essentials/dynamic-matching.html#reacting-to-params-changes) by invoking the `getUser` method of our `applicationUserManager` object, testing for a result and reading the `profile.name` property of the return value.

The `<script>` section of our `Login.vue` component will become:

```html
<script>
import applicationUserManager from '../applicationusermanager'
export default {
    data () {
    return {
      user: {
        name: '',
        isAuthenticated: false
      }
    }
  },
  async created () {
    await this.refreshUserInfo()
  },
  watch: {
    async '$route' (to, from) {
      await this.refreshUserInfo()
    }
  },
  methods: {
    async login () {
      try {
        await applicationUserManager.login()
      } catch (error) {
        console.log(error)
      }
    },
    async logout () {
      try {
        await applicationUserManager.logout()
      } catch (error) {
        console.log(error)
      }
    },
    async refreshUserInfo () {
      const user = await applicationUserManager.getUser()
      if (user) {
        this.user.name = user.profile.name
        this.user.isAuthenticated = true
      } else {
        this.user.name = ''
        this.user.isAuthenticated = false
      }
    }
  }
}
</script>
```

The `<template>` section will become:

```html
<template>
  <v-btn icon @click="login" v-if="!user.isAuthenticated">
    <v-icon>mdi-account</v-icon>
  </v-btn>
  <v-btn text @click="logout" v-else>
      {{ user.name }}
    <v-icon>mdi-account-off</v-icon>
  </v-btn>
</template>
```

Save, rebuild and test. You should be able to login and logout and see the UI change accordingly.

### Show / Hide the create button

The `App` vue also needs to know if the user is authenticated in order to decide wether to show the `Create` button or not, so let's move the `user` property and the logic behind the `refreshUserInfo` into a [`mixin`](https://vuejs.org/v2/guide/mixins.html).

In the `src` folder, create a `checksec.js` file.

```js
import applicationUserManager from './applicationusermanager'

const checksecmixin = {
  data () {
    return {
      user: {
        name: '',
        isAuthenticated: false
      }
    }
  },
  async created () {
    await this.refreshUserInfo()
  },
  watch: {
    async '$route' (to, from) {
      await this.refreshUserInfo()
    }
  },
  methods: {
    async refreshUserInfo () {
      const user = await applicationUserManager.getUser()
      if (user) {
        this.user.name = user.profile.name
        this.user.isAuthenticated = true
      } else {
        this.user.name = ''
        this.user.isAuthenticated = false
      }
    }
  }
}

export default checksecmixin

```

Now let's import the mixin in the `Login` component.

- Open the `src/components/Login.vue` file.
- Add the `import` for the `checksec`

```js
import checkSecMixin from "../checksec"
```

- Add the mixin to the `mixins` array

```js
mixins:[checkSecMixin],
```

- Remove what's been moved to the mixin

The `<script>` section of your `Login` should now look like this:

```html
<script>
import checkSecMixin from "../checksec"
import applicationUserManager from '../applicationusermanager'
export default {
  mixins:[checkSecMixin],
  methods: {
    async login () {
      try {
        await applicationUserManager.login()
      } catch (error) {
        console.log(error)
      }
    },
    async logout () {
      try {
        await applicationUserManager.logout()
      } catch (error) {
        console.log(error)
      }
    }
  }
}
</script>
```

The functionalities of your site should result unchanged, but now we can proceed to modify the `App` view.

- Open the `src/App.vue` View.
- Import the mixin

```js
import checkSecMixin from "./checksec"
```

- Add the mixin to a `mixins` array

```js
  mixins:[checkSecMixin]
```

- Show the `create` button only if the user is authenticated

```html
<v-btn icon :to="{name: 'create'}"  v-if="user.isAuthenticated">
  <v-icon>mdi-pencil-plus</v-icon>
</v-btn>
```

Try your frontend. You should see the `Create` button only if the user is logged on.

We have successfully managed to protect the `Create` operation.

What we need to do next is to allow updates and deletes only to the product owners.
This is what we're going to do in the next lab.

Go to `Labs/Lab07`, open the `readme.md` and follow the instructions thereby contained.   

## Additional Resources

For complete samples of IdentityServer 4, you can clone the [IdentityServer4 Samples github project](https://github.com/IdentityServer/IdentityServer4.Samples)
