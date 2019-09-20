<template>
  <v-row>
    <v-col
     v-for="product in products" :key="product.id"
     cols="12" md="4"
    >
      <product :product="product" details update requestdelete />
    </v-col>
  </v-row>
</template>

<script>
import datalayer from '@/datalayer'
import Product from "@/components/Product.vue"

export default {
  components: {
    Product
  },
  data () {
    return {
      products: []
    }
  },
  async created () {
    this.products = await datalayer.getProducts()
  },
  watch: {
    async '$route' (to, from) {
      this.products = await datalayer.getProducts()
    }
  }
}
</script>

<style>

</style>