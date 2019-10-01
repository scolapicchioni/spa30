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
  async created () {
    this.product = await datalayer.getProductById(+this.$route.params.id)
  },
  methods: {
    async deleteProduct () {
      await datalayer.deleteProduct(+this.$route.params.id)
      this.$router.push({name: 'home'})
    }
  }
}
</script>

<style>

</style>