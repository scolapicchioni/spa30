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
    
    <login/>

    <v-btn icon :to="{name: 'create'}"  v-if="user.isAuthenticated">
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

<script>
import Login from "@/components/Login.vue"
import checkSecMixin from "./checksec"

export default {
  mixins:[checkSecMixin],
  components: {
    Login
  }
}
</script>

<style lang="scss">
</style>
