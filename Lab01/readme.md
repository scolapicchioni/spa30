# The Vue FrontEnd: A Progressive Web Application

We will start by building the client side application using Vue.js.

From [the official Vue documentation:](https://vuejs.org/v2/guide/)

## What is Vue.js?

> Vue (pronounced /vjuː/, like view) is a progressive framework for building user interfaces. Unlike other monolithic frameworks, Vue is designed from the ground up to be incrementally adoptable. The core library is focused on the view layer only, and is easy to pick up and integrate with other libraries or existing projects. On the other hand, Vue is also perfectly capable of powering sophisticated Single-Page Applications when used in combination with modern tooling and supporting libraries.

Watch the [video](https://www.vuemastery.com/courses/intro-to-vue-js/vue-instance/) on the documentation site in order to understand what Vue is and how it works.

Although not recommended for beginners, we will make use of already made templates that will take care of configuration and build steps for us. 
In order to install and use these, we first need **npm**.

If do not know what npm is, how to install it and how to use it, watch lessons 1 to 10 of the [documentation](
https://docs.npmjs.com/getting-started/what-is-npm).

When you're done learning, install the latest version of [nodejs](https://nodejs.org/en/) 

## vue-cli Installation

We are going to install and use [vue-cli](https://cli.vuejs.org/)

> Vue.js provides an [official CLI](https://github.com/vuejs/vue-cli) for quickly scaffolding ambitious Single Page Applications. It provides batteries-included build setups for a modern frontend workflow. It takes only a few minutes to get up and running with hot-reload, lint-on-save, and production-ready builds

**NOTE: At the time of this writing, vue cli is at version 3.11. If you use an updated version your experience may vary**

We are going to create a new project and we're going to manually select the following features:
- Babel
- Progressive Web App (PWA) Support
- Router
- CSS Pre-processors
- Linter / Formatter

Do not worry if you don't know what these options are, we are going to focus on these aspects during the labs. For now we're just going to use these options in order to get a project that will invoke **WebPack** to build our assets.
If you want to know more about WebPack, read the [documentation](https://webpack.js.org/concepts/) and follow the [Core Concepts of the WebPack Academy](https://webpack.academy/p/the-core-concepts).

- Open a command prompt
- Navigate to the ```Labs\Lab01\Start\MarketPlace``` folder
- Type the following commands

```
npm install -g @vue/cli
vue create frontend
```

When asked to `Please pick a preset`, select `Manually select features`. 
You will be asked a few questions.
This is the configuration you should use:

```
- (*) Babel
- ( ) TypeScript
- (*) Progressive Web App (PWA) Support
- (*) Router
- ( ) Vuex
- (*) CSS Pre-processors
- (*) Linter / Formatter
- ( ) Unit Testing
- ( ) E2E Testing       
```

- To the question ` Use history mode for router? (Requires proper server setup for index fallback in production) (Y/n)` 
  - answer `Y`
- On `Pick a CSS pre-processor (PostCSS, Autoprefixer and CSS Modules are supported by default):` 
  - select `Sass/SCSS (with node-sass)`
- On `Pick a linter / formatter config: (Use arrow keys)` 
  - select ` ESLint + Standard config`
- On `Pick additional lint features: (Press <space> to select, <a> to toggle all, <i> to invert selection)` 
  - select only `(*) Lint and fix on commit`
- On ` Where do you prefer placing config for Babel, PostCSS, ESLint, etc.? (Use arrow keys)` 
  - select `In dedicated config files`
- To the question `Save this as a preset for future projects? (y/N)` 
  - answer `N`


Now type

```
cd frontend
npm run serve
```

After a while you should be able to see a message on the console showing the address of your new website. Open a browser and navigate to that address (it should be `http://localhost:8080/`). You should see a page with the Vue logo and some links to the Vue docs, resources, ecosystem and so on.

[It is also recommended to install the dev tools](https://github.com/vuejs/vue-devtools#vue-devtools)

You can now go back to the console and press `CTRL + C` and then `Y` to stop the website.

Open the `frontend` folder in Visual Studio Code, by typing `code .`.
Visual Studio Code does not understand Vuejs natively, but there is an extension that can help. If you haven't already, install [Vetur](https://vuejs.github.io/vetur/)

Let's see what was generated for us.
It is going to be a very long journey to understand what's going on, so bear with me and don't get discouraged if you have the impression that it's overwhelming. It will all make sense in the end (hopefully).

We got a lot of folders and files, so let's start from the start.

When the user navigates to the root of our website, he downloads an `index.html` page. What the user gets is different from what we have in our `public` folder, because it is  the result of 
- the build process of our `vue-cli` 
- the dynamic runtime process of `vue`

As per the [Documentation](https://cli.vuejs.org/guide/html-and-static-assets.html#html)

>The file `public/index.html` is a template that will be processed with [html-webpack-plugin](https://github.com/jantimon/html-webpack-plugin). During build, asset links will be injected automatically. In addition, Vue CLI also automatically injects resource hints (preload/prefetch), manifest/icon links (when PWA plugin is used), and the asset links for the JavaScript and CSS files produced during the build.

Translated into common human language, this means that when we build and run our application (remember the `npm run serve` command you had to type to start the website?) a lot of stuff happens.
The [vue-cli-service](https://cli.vuejs.org/guide/#cli-service)
- opens our `main.js` file 
- starts following every `import` it finds
- invokes the specific loaders if necessary (to translate everything into pure javascript)
- bundles the output in one `.js`
- injects the `.js` file into our `index.html`
 
So the `index.html` that the user gets also contains a reference to a `javascript` file. This file tells `Vue` to take care of a specific `div` in the `index` page, specifically a `div` with an `app` id.
This means that the rest of the magic happens at runtime.
  
If you inspect `public/index.html` you can find a 

```html
<div id="app"></div>
``` 

This is where Vue will render its content.
You can also find 

```html
<!-- built files will be auto injected -->
```

This is where the assets will be injected every time we build our project. 

The ```main.js``` file is the starting point of the dynamic content and logic behind our application.

This file:
- Registers a [Service Worker](https://developers.google.com/web/ilt/pwa/introduction-to-service-worker)
- Creates a [Vue instance](https://vuejs.org/v2/guide/instance.html)
- Injects and configures the [Vue Router](https://router.vuejs.org/)
- Renders the `App` [Single File Component](https://vuejs.org/v2/guide/single-file-components.html)
- Mounts the `App component` to the `html tag` whose `id` is `app`

If this doesn't make sense, let's go step by step and try to understand each part.

We need to follow all the files that are involved in the construction and configuration of the `Vue instance`; they are a lot, so have patience. 

Open the ```src/main.js``` file. Examine the code that has been generated for us:

```js
import Vue from 'vue'
import App from './App.vue'
import router from './router'
import './registerServiceWorker'

Vue.config.productionTip = false

new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
```

The code begins by importing the **vue** npm package. This is necessary to create the [Vue instance](https://vuejs.org/v2/guide/instance.html), later in the file.

It then proceeds to import `App.vue`, our main [component](https://vuejs.org/v2/guide/components.html)

> Components are one of the most powerful features of Vue. They help you extend basic HTML elements to encapsulate reusable code. At a high level, components are custom elements that Vue’s compiler attaches behavior to.

The project we created makes use of [Vue Router](https://router.vuejs.org/).

> Vue Router is the official router for Vue.js. It deeply integrates with Vue.js core to make building Single Page Applications with Vue.js a breeze.

Vue Router is configured in our `src\router.js` file, imported on line 3 (more on this later).

The next step is to import the `registerServiceWorker` file, which is used by [Progressive Web Applications](https://developers.google.com/web/progressive-web-apps/)

The [production tip](https://vuejs.org/v2/api/#productionTip) is nicely turned off.

Finally, we have everything we need to create the [Vue Instance](https://vuejs.org/v2/guide/instance.html)

> Every Vue application starts by creating a new Vue instance with the ```Vue``` function
> 
> (...)
>
> When you create a Vue instance, you pass in an options object.

The template we used to create our application uses [Single File Components](https://vuejs.org/v2/guide/single-file-components.html)

The `App` component is created and mounted to the DOM through the [render function](https://vuejs.org/v2/guide/render-function.html#ad) and the [$mount](https://vuejs.org/v2/api/#vm-mount) method. 

So, as a recap:
- Index.html starts, with a reference to the bundle created by webpack
- The bundle starts with main.js
- main.js:
  - registers the serviceworker
  - creates an instance of Vue
  - configures and injects the Router
  - renders and mounts the App component on the `div` tag with the `app` id

This means that when the user navigates to our root, `index.html` is downloaded together with a javascript file that creates a Vue instance that replaces the `app div` with the content of the `App` component. 

So, what is the `App` component?

In order to proceed, let's switch to ```App.vue```, which we can also find in the ```src``` folder.

You will find two of the three sections of any Single File Component:

```html
<template>
...  
</template>

<style>
...
</style>
```

The `App` component uses the [router-link](https://router.vuejs.org/api/#router-link) and the [router-view](https://router.vuejs.org/api/#router-view) components.

> `<router-link>` is the component for enabling user navigation in a router-enabled app. The target location is specified with the `to` prop. It renders as an `<a>` tag with correct `href` by default. In addition, the link automatically gets an `active` CSS class when the target route is active.

>The `<router-view>` component is a functional component that renders the matched component for the given path.

A Route is just an address, like `/about`, or `/products/create` and so on.
When you use `vue router`, you configure each path with a specific `component`, so that when the user navigates to a route that component gets rendered in the `router-view`. To the user it looks like he navigated to the page `/about`, but he actually never left the `index.html` page. It's the `vue router` that dynamically changes the content of the page with the specific component.

What are the configured routes and which components will be rendered by the `router-view`? Open `src\router.js` to find out.

```js
import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

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

As you can see: 
- The `Home` component is mapped to the `/` route
- The `About` component is mapped to the `/about` route

This means that when the user navigates to the root (`/`), the `router-view` component will render the content of the `Home` component, while when the user navigates to `/about` the `About` component will be rendered.

So let's see what's the content of the `Home` component.

```html
<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js App"/>
  </div>
</template>

<script>
// @ is an alias to /src
import HelloWorld from '@/components/HelloWorld.vue'

export default {
  name: 'home',
  components: {
    HelloWorld
  }
}
</script>
```

Apparently, the `Home` component renders a `div` that contains an `image` with the `vue` logo and a `HelloWorld` component.

Open the `components/HelloWorld.vue` file and you'll see that it just contains a bunch of links to Vue resources like plugins, docs, etc.

So, as a recap (last time, I promise):
- `index.html` has a reference to the js bundle
- the js bundle starts with `main.js`
- `main.js` creates a `vue instance` (`new Vue(...)`)
- the vue instance renders the `App` component
- the `App` component uses `vue router`
- the Router renders the `Home` component
- the `Home` component renders the `HelloWorld` component

I told you it was a lot, but we're finally there.   

So now it's time to customize the website.

## The Products Component

Our goal is to display a list of products instead of the HelloWorld.  
In future labs we will take care of 
- the UI by using Material Design
- the data by creating and using a REST service
For the time being we will display a simple list retrieved from an array in memory.

Let's start by creating a `Products` component

Go to the `components` folder and add a `Products.vue` file.
In Visual Studio Code, type `vue` and press TAB. If you have `Vetur` you should now have the following code:

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

Replace the `template` section, with the following code:

```html
<template>
  <h1>Products</h1>
</template>
```

Save the file.

Now let's change the `Home` vue to use our newly created `Products` component instead of the `HelloWorld` one. 

- Open the `vues/Home.vue` file.
- Replace its content with

```html
<template>
  <div>
    <Products/>
  </div>
</template>

<script>
// @ is an alias to /src
import Products from '@/components/Products.vue'

export default {
  name: 'home',
  components: {
    Products
  }
}
</script>
```

- Save the file.

- Open your [Visual Studio Code integrated terminal](https://code.visualstudio.com/docs/editor/integrated-terminal). 
- Type `npm run serve`.

**Note: ensure that the terminal output is ```DONE Compiled successfully```. If it isn't, correct all the errors from top to bottom and save the file again.** 

- Navigate to your website (`http://localhost:8080) and verify that the home page has the **Products** title instead of the links.

Let's continue with our `Products` component by adding some data that will be used to dynamically render UI.

Vue has a [Reactive System](https://vuejs.org/v2/guide/instance.html#Data-and-Methods) that binds the data to the view.

> When a Vue instance is created, it adds all the properties found in its data object to Vue’s reactivity system. When the values of those properties change, the view will “react”, updating to match the new values.

Since we are creating a component, our ```data``` property is a [function](https://vuejs.org/v2/guide/components.html#data-Must-Be-a-Function).

The ```data``` function should return an object with a ```products``` property. The products property should be an ```array``` with some ```product``` object. Each product object should have four properties: ```id``` *(number)*, ```name``` *(string)*, ```description``` *(string)* and ```price``` *(number)*.

You ```data``` function could look something like this:

```js
data () {
    return {
      products: [
        {id: 1, name: 'Win-win survival strategies', description: 'Bring to the table win-win survival strategies to ensure proactive domination.', price: 1234},
        {id: 2, name: 'High level overviews', description: 'Iterative approaches to corporate strategy foster collaborative thinking to further the overall value proposition.', price: 23},
        {id: 3, name: 'Organically grow world', description: 'Organically grow the holistic world view of disruptive innovation via workplace diversity and empowerment.', price: 456},
        {id: 4, name: 'Agile frameworks', description: 'Leverage agile frameworks to provide a robust synopsis for high level overviews', price: 98765}
      ]
    }
  }
```

This means that the `<script>` section of the `Products.vue` file should now look like the following:

```html
<script>
export default {
  data () {
    return {
      products: [
        {id: 1, name: 'Win-win survival strategies', description: 'Bring to the table win-win survival strategies to ensure proactive domination.', price: 1234},
        {id: 2, name: 'High level overviews', description: 'Iterative approaches to corporate strategy foster collaborative thinking to further the overall value proposition.', price: 23},
        {id: 3, name: 'Organically grow world', description: 'Organically grow the holistic world view of disruptive innovation via workplace diversity and empowerment.', price: 456},
        {id: 4, name: 'Agile frameworks', description: 'Leverage agile frameworks to provide a robust synopsis for high level overviews', price: 98765}
      ]
    }
  }
}
</script>
```

Save your file and ensure that it gets compiled. If not, resolve your issues before going further.

Now let's update the UI. We will use a loop in order to render multiple divs, one for each product.

The directive to loop through array items is [v-for](https://vuejs.org/v2/guide/list.html)

> We can use the v-for directive to render a list of items based on an array. The v-for directive requires a special syntax in the form of *item in items*, where *items* is the source data array and *item* is an alias for the array element being iterated on

We are also going to use our ```id``` property as [key](https://vuejs.org/v2/guide/list.html#key)

> To give Vue a hint so that it can track each node’s identity, and thus reuse and reorder existing elements, you need to provide a unique key attribute for each item. An ideal value for key would be the unique id of each item.

Replace the ```<template>``` section with the following code:

```html
<template>
    <div>
        <div v-for="product in products" :key="product.id">
        {{ product.id }} - {{ product.name }} 
        <p>{{ product.description }}</p>
        {{ product.price }}
        </div>
    </div>
</template>
```

Save your file. 
Go to the browser and verify that the home page now contains four divs with the details of our products.
Do not worry about the style. We will fix it on a later lab.

In the next lab we will build four additional views:

- Details
- Insert
- Update
- Delete 

To continue, open ```Labs/Lab02/readme.md``` and follow the provided instructions.

- To know more about vue-router, check this [tutorial](https://scotch.io/tutorials/getting-started-with-vue-router)
- For more information on PWA with Vue, check this [tutorial](https://blog.sicara.com/a-progressive-web-application-with-vue-js-webpack-material-design-part-1-c243e2e6e402)
