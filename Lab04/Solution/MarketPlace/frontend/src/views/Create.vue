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
        <v-btn @click="insertProduct" color="primary">INSERT PRODUCT</v-btn>
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
  methods: {
    insertProduct () {
      datalayer.insertProduct(this.product)
      this.$router.push({name: 'home'})
    }
  }
}
</script>

<style>

</style>