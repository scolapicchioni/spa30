<template>
  <v-row>
    <v-col
     v-for="product in products" :key="product.id"
     cols="12" md="4"
    >
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
    </v-col>
  </v-row>
</template>

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

<style>

</style>