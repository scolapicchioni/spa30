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
    async updateProduct () {
      await datalayer.updateProduct(+this.$route.params.id, this.product)
      this.$router.push('/')
    }
  }
}
</script>

<style>

</style>