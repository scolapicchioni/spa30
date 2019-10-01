<template>
  <v-row>
    <v-col cols="sm">
      <product :product="product" confirmdelete />
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
      product: {
        id: 0,
        name: '',
        description: '',
        price: 0
      }
    }
  },
  async created () {
    this.product = await datalayer.getProductById(+this.$route.params.id)
  }
}
</script>

<style>

</style>