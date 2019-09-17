<template>
    <div>
        <h1>Details {{ $route.params.id }}</h1>
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