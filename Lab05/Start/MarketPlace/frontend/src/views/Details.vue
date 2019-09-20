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