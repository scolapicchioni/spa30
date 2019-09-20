<template>
  <div>
    <Products/>
  </div>
</template>

<script>
// @ is an alias to /src
import Products from '@/components/Products.vue'
import datalayer from '@/datalayer'

export default {
  name: 'home',
  components: {
    Products
  },
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
