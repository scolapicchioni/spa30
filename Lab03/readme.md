# FrontEnd: Styling the views with the vuetify

There are many different [CSS frameworks](https://onaircode.com/top-css-frameworks-web-designer/) around.
[Many](https://www.sitepoint.com/free-material-design-css-frameworks-compared/) implement the [Google Material Design Guidelines](https://material.io/guidelines/).
It's hard to make a choice, so we'll just jump right in and select what the [community advices as popular choice](https://vue-community.org/guide/ecosystem/ui-libraries.html#vuetify): vuetify 

> Vuetify is a semantic component framework for Vue. It aims to provide clean, semantic and reusable components that make building your application a breeze. Build amazing applications with the power of Vue, Material Design and a massive library of beautifully crafted components and features.
Vuetify is praised not only for a wide selection of components, but also for the way it's maintained. Currently it's developed by a full team of developers and it's got big and well organized community. The project is funded via Patreon and Open Collective.
The library's ecosystem provides multiple tools, such as theme generator, Webpack loader or opinionated Eslint config. It's also easy to implement Vuetify in your application thanks to provided plugins for Vue CLI and Nuxt.
Internally, Vuetify is written with Typescript. The upcoming release of Vuetify 2 with improved theme system, accessibility and performance, will also bring the move from Stylus to SASS.

Seen the fact that we created our project using the cli, we can take advantage of the [Vuetify package](https://github.com/vuetifyjs/vue-cli-plugin-vuetify).

Open a terminal, navigate to the frontend folder (Lab03\Solution\MarketPlace\frontend) and type

```
vue add vuetify
```

- At the question `Choose a preset` select `Configure (advanced)`
- At the question `Use a pre-made template? (will replace App.vue and HelloWorld.vue) (Y/n)` answer `N`
- At the question `Use custom theme? (y/N)` answer `Y`
- At the question `Use custom properties (CSS variables)? (y/N)` answer `N`
- At the option `Select icon font (Use arrow keys)` select `Material Design Icons`
- At the question `Use fonts as a dependency (for Electron or offline)? (y/N)` answer `N`
- At the question `Use a-la-carte components? (Y/n)` select `Y`
- As an option for ` Select locale (Use arrow keys)` select `English`

After a while you should see a message with a list of files that have been touched by vuetify. Let's check some of those.

First of all, if you open `src/main.js` you'll notice that there's a new `import vuetify from './plugins/vuetify'` and that `vuetify` is injected as an option in the Vue instance. 
You can see that there's a new `src/plugins` folder and there you can find the `vuetify.js` file used to configure vuetify.
As per the [documentation](https://vuejs.org/v2/guide/plugins.html), the plugin is installed globally and comfigured with specific options.

Another file that has changed is the `public/index.html`, where we can find the reference to the [Roboto](https://fonts.google.com/specimen/Roboto) font and to the [Material Design Icons](https://material.io/resources/icons/?style=round):

```html
<link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:100,300,400,500,700,900">
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@mdi/font@latest/css/materialdesignicons.min.css">
```

We are ready to proceed to modify `App.vue`.

The first component we need to add is the [Application](https://vuetifyjs.com/en/components/application)

>In Vuetify, the v-app component and the app prop on components like v-navigation-drawer, v-app-bar, v-footer and more, help bootstrap your application with the proper sizing around <v-content> component. This allows you to create truly unique interfaces without the hassle of managing your layout sizing. The v-app component is REQUIRED for all applications. This is the mount point for many of Vuetify's components and functionality and ensures that. It propagates the default application variant (dark/light) to children components and also ensures proper cross-browser support for certain click events in browsers like Safari. v-app should only be used within your application ONCE.

Let's take the [Default application markup
](https://vuetifyjs.com/en/components/application#default-application-markup) and use it to start changing our layout:


Let's remove the [navigation drawer](https://vuetifyjs.com/en/components/navigation-drawers) and the [footer](https://vuetifyjs.com/en/components/footer). Let's also fill up our [app bar](https://vuetifyjs.com/en/components/app-bars) by giving it the primary color and adding three [icon](https://materialdesignicons.com/) buttons to navigate to the `home`, `about` and `create` route. 

```html
<template>
<v-app>
  <v-app-bar app color="primary">
    <v-toolbar-title>Market Place</v-toolbar-title>

    <div class="flex-grow-1"></div>

    <v-btn icon :to="{name: 'home'}">
      <v-icon>mdi-home</v-icon>
    </v-btn>

    <v-btn icon :to="{name: 'about'}">
      <v-icon>mdi-chat</v-icon>
    </v-btn>
    
    <v-btn icon :to="{name: 'create'}">
      <v-icon>mdi-pencil-plus</v-icon>
    </v-btn>

  </v-app-bar>

  <!-- Sizes your content based upon application components -->
  <v-content>

    <!-- Provides the application the proper gutter -->
    <v-container fluid>

      <!-- If using vue-router -->
      <router-view></router-view>
    </v-container>
  </v-content>
</v-app>
</template>
```
If you navigate to your home page, you should already see a better layout than before.

We can also empty out the `<style>` section and proceed to change the [Theme](https://vuetifyjs.com/en/customization/theme)
Feel free to use the [Theme generator](https://theme-generator.vuetifyjs.com/) to customize your own color scheme, then apply those color to the `src/plugins/vuetify.js` file, which could become something like this:

```js
import Vue from 'vue'
import Vuetify from 'vuetify/lib'

Vue.use(Vuetify)

export default new Vuetify({
  theme: {
    themes: {
      light: {
        'primary': '#ff9800',
        'secondary': '#cddc39',
        'accent': '#8bc34a',
        'error': '#f44336',
        'warning': '#e91e63',
        'info': '#00bcd4',
        'success': '#795548',
        'anchor': '#cddc39'
      }
    }
  },
  icons: {
    iconfont: 'mdi'
  }
})
```

Save and verify that your color scheme is changed.

Time to tackle the `Products` component.


We will align our products into a [Grid](https://vuetifyjs.com/en/components/grids).
We already have a container in the App, so let's create one row and let's wrap each product into a column that spans one third of the row if the screen width is large enough and occupies the whole row if the screen is small. 

```html
  <v-row>
    <v-col
     v-for="product in products" :key="product.id"
     cols="12" md="4"
    >      
    </v-col>
  </v-row>
```

Each product will be displayed into a [Card](https://vuetifyjs.com/en/components/cards).

The inside of the column will have this:

```html
<v-card elevation="10">
    <v-card-title>{{ product.id }} - {{ product.name }} </v-card-title>
    <v-card-text>
    <p>{{ product.description }}</p>
    <p>{{ product.price }}</p>
    </v-card-text>
    <v-card-actions>
        <v-btn text :to="{name: 'details', params: {id: product.id}}">details</v-btn>
        <v-btn text :to="{name: 'update', params: {id: product.id}}">edit</v-btn>
        <v-btn text :to="{name: 'delete', params: {id: product.id}}">delete</v-btn>
    </v-card-actions>
</v-card>
```

Save and verify that the home page has now cards. Also, resize the browser and verify that the cards resize depending on the viewport width.

The template section of the `Details` component will look pretty much like the `Products` component.

```html
<template>
  <v-row>
    <v-col cols="sm">
      <v-card elevation="10">
        <v-card-title>{{ product.id }} - {{ product.name }} </v-card-title>
        <v-card-text>
        <p>{{ product.description }}</p>
        <p>{{ product.price }}</p>
        </v-card-text>
        <v-card-actions>
            <v-btn icon :to="{name: 'update', params: {id: product.id}}"><v-icon>mdi-pencil</v-icon></v-btn>
            <v-btn icon :to="{name: 'delete', params: {id: product.id}}"><v-icon>mdi-delete</v-icon></v-btn>
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>
```

Let's tackle the `Create` View.
We will need a [Form](https://vuetifyjs.com/en/components/forms). 
Inside the form we will need [TextFields](https://vuetifyjs.com/en/components/text-fields) and [TextAreas](https://vuetifyjs.com/en/components/textarea)
We will also transform our link into a [Button](https://vuetifyjs.com/en/components/buttons)

The `template` section will look like this:

```html
<template>
<v-form>
  <v-container>
    <v-row>
      <v-col>
        <v-text-field label="Product Name" v-model="product.name"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-textarea label="Description" v-model="product.description" hint="Product Description"></v-textarea>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-text-field label="Product Price" v-model.number="product.price"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-btn @click="insertProduct" color="primary">INSERT PRODUCT</v-btn>
      </v-col>
    </v-row>
  </v-container>
</v-form>
</template>
```

The `Update` will look more or less the same, we just need to modify the button into update instead of add.

```html
<template>
<v-form>
  <v-container>
    <v-row>
      <v-col>
        <v-text-field label="Product Name" v-model="product.name"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-textarea label="Description" v-model="product.description" hint="Product Description"></v-textarea>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-text-field label="Product Price" v-model.number="product.price"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-btn @click="updateProduct" color="primary">UPDATE PRODUCT</v-btn>
      </v-col>
    </v-row>
  </v-container>
</v-form>
</template>
```

The last template we have to change is the one of the `Delete` View, which will look similar to the `Details` view.

```html
<template>
  <v-row>
    <v-col cols="sm">
      <v-card elevation="10">
        <v-card-title>{{ product.id }} - {{ product.name }} </v-card-title>
        <v-card-text>
        <p>{{ product.description }}</p>
        <p>{{ product.price }}</p>
        </v-card-text>
        <v-card-actions>
            <v-btn @click="deleteProduct" color="warning">DELETE PRODUCT</v-btn>
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>
```
Our styling is complete.  Our next lab will focus on the back end: we're going to build a REST service using ASP.NET Core 3.0.

Go to `Labs/Lab04`, open the `readme.md` and follow the instructions thereby contained.   
