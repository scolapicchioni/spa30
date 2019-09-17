# FrontEnd: Additional Views

Now that we have a Vue project with our first HomeView, we will proceed to create four additional views:
- Details
- Create
- Update
- Delete

We will also create a first javascript  Data Layer with methods to 
- get a list of all the products
- get one product given its id
- create a given product
- update a given product
- delete a product given its id 

For now our data layer will work with an array in memory, but we will replace it with one that will communicate with a REST service in a later lab.

- Create a ```datalayer.js``` in your ```src``` folder.
- Define a ```datalayer``` constant as an object containing the following:
  - a ```products``` property with an array with four products (just so that we will see some initial data in our HomeView as soon as we switch to this datalayer).
  - a ```getProducts``` method that returns the ```products``` array.
  - a ```getProductById``` method that 
    - accepts an id
    - looks for a product in the ```products``` array with the given id
    - returns the found product or undefined
  - an ```insertProduct``` method that 
    - accepts a product object
    - calculates a new id by finding the maximum id contained in the ```products``` array and adding 1
    - updates the id of the given product with newly calculated id
    - pushes the product in the array
  - a ```deleteProduct``` method that
    - accepts an id
    - looks for a product in the ```products``` array
    - deletes the found products or does nothing otherwise
  - an ```updateProduct``` method that 
    - accepts an id and a product
    - looks for a product in the ```products``` array with the given id
    - if the product is found
      - updates the properties of the found product with the values of the properties of the given product
    - does nothing otherwise
- Export the datalayer constant as default

In the end your file may look something like this:

```js
const datalayer = {
  products: [
    {id: 1, name: 'WIN-WIN survival strategies', description: 'Bring to the table win-win survival strategies to ensure proactive domination.', price: 12345},
    {id: 2, name: 'HIGH level overviews', description: 'Iterative approaches to corporate strategy foster collaborative thinking to further the overall value proposition.', price: 2345},
    {id: 3, name: 'ORGANICALLY grow world', description: 'Organically grow the holistic world view of disruptive innovation via workplace diversity and empowerment.', price: 45678},
    {id: 4, name: 'AGILE frameworks', description: 'Leverage agile frameworks to provide a robust synopsis for high level overviews', price: 9876}
  ],
  getProducts () {
    return this.products
  },
  getProductById (id) {
    return this.products.find(p => p.id === id)
  },
  insertProduct (product) {
    const id = this.products.reduce((prev, curr) => prev.id > curr.id ? prev.id : curr.id) + 1
    product.id = id
    this.products.push(product)
  },
  updateProduct (id, product) {
    const oldProduct = this.products.find(p => p === id)
    if (oldProduct) {
      oldProduct.name = product.name
      oldProduct.price = product.price
      oldProduct.description = product.description
    }
  },
  deleteProduct (id) {
    const productindex = this.products.findIndex(p => p.id === id)
    this.products.splice(productindex, 1)
  }
}

export default datalayer

```

We are going to start by initializing the state of the data.products property with an empty array, then during the created [lifecycle hook](https://vuejs.org/v2/guide/instance.html#Instance-Lifecycle-Hooks) we will update it with a call to our datalayer.

Let's start with the `Products` component.

Open ```src\components\Product.vue```.

Initialize the `products` property with an empty array.

Import the ```datalayer``` constant from ```/src/datalayer``` and invoke the ```getProducts``` method during creation of the component.

Your ```<script>``` section should look like this:

```js
<script>
import datalayer from '@/datalayer'

export default {
  data () {
    return {
      products: []
    }
  },
  created () {
    this.products = datalayer.getProducts()
  }
}
</script>
```

By saving your files you should see the page refresh with the new data.

**NOTE: If vue-cli-service is not running:**
- Open your [Visual Studio Code integrated terminal](https://code.visualstudio.com/docs/editor/integrated-terminal). 
- Type `npm run serve`.

**Note: ensure that the terminal output is ```DONE Compiled successfully```. If it isn't, correct all the errors from top to bottom and save the file again.** 

- Navigate to your website (`http://localhost:8080) and verify that the home page has the **Products** coming from the datalayer.


Now let's proceed with the new views. We are going to:
- Create four new components
- Create and configure four new routes, each mapped to its corresponding component
- Link the routes in the Home View
- Refactor the components to add the logic to interact with the datalayer

Our first step is to create four new Vue Components.

Open `src/views` and create a `Details.vue` file. 
Fill the file with an initial skeleton of a `template`, `script` and `style`.
Repeat for `Create.vue`, `Update.vue` and `Delete.vue`.

Your files should look something like this:

```html
<template>
    <h1>Details</h1>
</template>

<script>
export default {
  
}
</script>

<style>

</style>
```

with, of course, the corresponding titles matching the action they represent.

Now let's create our routes.

Open the `src\router.js` file.

Import the four components and add four new routes, each bound to the corresponding path and component.
In the end your file may look like this:

```js
import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import Create from './views/Create.vue'
import Details from './views/Details.vue'
import Update from './views/Update.vue'
import Delete from './views/Delete.vue'

Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/create',
      name: 'create',
      component: Create
    },
    {
      path: '/details',
      name: 'details',
      component: Details
    },
    {
      path: '/update',
      name: 'update',
      component: Update
    },
    {
      path: '/delete',
      name: 'delete',
      component: Delete
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "about" */ './views/About.vue')
    }
  ]
})


```  

Save all your files, ensure that everything gets compiled correctly and try to navigate to the paths you configured by entering the addresses on your browser. You should see the corresponding component replacing Home.

The Details, Update, and Delete views need some data to display and the route should contain a parameter in the form of an id. We are going to use [Dynamic Matching](https://router.vuejs.org/en/essentials/dynamic-matching.html)

> A dynamic segment is denoted by a colon :. When a route is matched, the value of the dynamic segments will be exposed as this.$route.params in every component

Let's change the `src/router.js` to add a dynamic `id` parameter at the end of the `details`, `update` and `delete` routes:

```js
{
  path: '/details/:id',
  name: 'details',
  component: Details
},
{
  path: '/update/:id',
  name: 'update',
  component: Update
},
{
  path: '/delete/:id',
  name: 'delete',
  component: Delete
}
```

Now let's update the components to show the id parameter contained in the global `$route` object:

```html
<template>
    <h1>Details {{ $route.params.id }}</h1>
</template>
``` 

Repeat also for the Update and Create.
Save and navigate to `details/3`.
You should see in the header the number `3` following the `Details` word.

We will follow the same pattern of initializing an empty product property in the data function and then refresh it during the creation of our component.

Our `script` tag will look like this:

```js
<script>
import datalayer from '@/datalayer'
export default {
  name: 'details-view',
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
  created () {
    this.product = datalayer.getProductById(+this.$route.params.id)
  }
}
</script>
```

Now let's update our template to show the product data:

```html
<template>
<div>
    <h1>Details {{ $route.params.id }}</h1>
    <div>{{ product.id }}</div>
    <div>{{ product.name }}</div>
    <div>{{ product.description }}</div>
    <div>{{ product.price }}</div>
</div>
</template>
```

Save and check that the details view updates correctly when you enter an address such as `/details/1` and `/details/2`.

Repeat the same steps for the Delete View.
In the end your Delete.vue should look like this:

```html
<template>
  <div>
    <h1>Delete</h1>
    <div>{{ product.id }}</div>
    <div>{{ product.name }}</div>
    <div>{{ product.description }}</div>
    <div>{{ product.price }}</div>
  </div>
</template>

<script>
import datalayer from '@/datalayer'
export default {
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
  created () {
    this.product = datalayer.getProductById(+this.$route.params.id)
  }
}
</script>

<style>

</style>
```

We will repeat this process also for the `Update View`, but our template will be different, because we will have to render a form and [bind](https://vuejs.org/v2/guide/forms.html) two textboxes and a textarea to our product data in order to let the user input the new values.

This is how our UpdateView will look like:

```html
<template>
<form>
  <div>
    <label for="name">Product Name</label>
    <input id="name" v-model="product.name" type="text" placeholder="name"/>
  </div>
  <div>
    <label for="description">Product Description</label>
    <textarea id="description" v-model="product.description" placeholder="description"></textarea>
  </div>
  <div>
    <label for="price">Product Price</label>
    <input id="price" v-model.number="product.price" type="text" placeholder="price"/>
  </div>
</form>
</template>

<script>
import datalayer from '../datalayer'
export default {
  name: 'update-view',
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
  watch: {
    '$route' (to, from) {
      this.product = datalayer.getProductById(+this.$route.params.id)
    }
  },
  created () {
    this.product = datalayer.getProductById(+this.$route.params.id)
  }
}
</script>

<style>

</style>
```

Lastly, we will update the Create View. It won't be necessary to update the product data during creation, seen the fact that this route is not dynamically bound.

The Create View will have the following code:

```html
<template>
<form>
  <div>
    <label for="name">Product Name</label>
    <input id="name" v-model="product.name" type="text" placeholder="name"/>
  </div>
  <div>
    <label for="description">Product Description</label>
    <textarea id="description" v-model="product.description" placeholder="description"></textarea>
  </div>
  <div>
    <label for="price">Product Price</label>
    <input id="price" v-model.number="product.price" type="text" placeholder="price"/>
  </div>
</form>
</template>

<script>
export default {
  name: 'create-view',
  data () {
    return {
      product: {
        id: 0,
        name: '',
        description: '',
        price: 0
      }
    }
  }
}
</script>

<style>

</style>
```

Of course neither the Delete, nor the Update or the Create do anything, so let's fix that.
Let's start with the `DeleteView`.

We're going to [bind a click event](https://vuejs.org/v2/guide/events.html) of a link tag to a [method event handler](https://vuejs.org/v2/guide/events.html#Method-Event-Handlers) and we are going to make sure that it [prevents the default form behavior](https://vuejs.org/v2/guide/events.html#Event-Modifiers).
The method will invoke the datalayer to delete the corresponding product and will then [programmatically navigate](https://router.vuejs.org/en/essentials/navigation.html) to the root.

The `template` will now contain 

```html
<a href="#" @click.prevent="deleteProduct">
  DELETE PRODUCT
</a>
```

and in the `script` we will add a 

```js
methods: {
  deleteProduct () {
    datalayer.deleteProduct(+this.$route.params.id)
    this.$router.push({name: 'home'})
  }
}
```

Save and navigate to `/delete/2`, then click on the link and check that the product is deleted and that you return to the home view.

Now on to the `Update` view. We will handle the click event of a link as well. The handler method will invoke the datalayer to update the product and it will navigate back to the root.

Let's add the following code to the `template`:

```html
<a href="#" @click.prevent="updateProduct">
  UPDATE PRODUCT
</a>
```

And now let's handle the click event in a new `updateProduct` method in the `script` section.

```js
methods: {
  updateProduct () {
    datalayer.updateProduct(+this.$route.params.id, this.product)
    this.$router.push({name: 'home'})
  }
}
``` 

Save, navigate to `/update/2`, change some values, click on the link and verify that the product gets updated and that you are sent back to the root.

Our last step is to complete the `CreateView`. We will have to handle the click event of a link one last time and invoke the datalayer to create the new product.

Updated template:

```html
<a href="#" @click.prevent="insertProduct">
  INSERT PRODUCT
</a>
```

Updated script:
```js
methods: {
  insertProduct () {
      datalayer.insertProduct(this.product)
      this.$router.push({name: 'home'})
    }
}
``` 
Also, don't forget to import the datalayer by adding

```
import datalayer from '@/datalayer'
```

at the beginning of the ```<script>``` section.


Save, navigate to `/create`, insert some values, click on the link and verify that the product gets created and that you are sent back to the root.

The very last thing we need to do is to add some [navigation links](https://router.vuejs.org/en/api/router-link.html) to our `App.vue` and to the `Products.vue` component.

We will insert a link to view the details, update and delete for each product (passing the specific id to the route) and one link to create a new product.

The `template` of our `App.vue` becomes:

```html
<template>
  <div id="app">
    <div id="nav">
      <router-link to="/">Home</router-link> |
      <router-link to="/about">About</router-link>
      <router-link to="/create">Create</router-link>
    </div>
    <router-view/>
  </div>
</template>
```

Save and verify that you can navigate to the `create` route from any page.

Now let's update the `Products` component.

Add three new `<router-link>` tags to navigate to the `update`, `details` and `delete` routes, passing the `id` as a `param`.

The template of the `Products` component becomes:

```html
<template>
    <div>
        <div v-for="product in products" :key="product.id">
        {{ product.id }} - {{ product.name }} 
        <p>{{ product.description }}</p>
        {{ product.price }}
        <router-link :to="{name: 'details', params: {id: product.id}}">details</router-link>
        <router-link :to="{name: 'update', params: {id: product.id}}">edit</router-link>
        <router-link :to="{name: 'delete', params: {id: product.id}}">delete forever</router-link>
        </div>
    </div>
</template>
```

Save and verify that each product now link to its own edit, update and delete.

Our views are functionally ready but their appearance could improve. We are going to take care of their styles in the next lab.

Go to `Labs/Lab03`, open the `readme.md` and follow the instructions thereby contained.