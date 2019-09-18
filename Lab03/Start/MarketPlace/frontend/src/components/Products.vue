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