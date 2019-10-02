<template>
  <v-card elevation="10">
    <v-img class="white--text" :src="product.picture">
        <v-card-title class="align-end fill-height">{{ product.id }} - {{ product.name }}</v-card-title>
    </v-img>

    <v-card-title></v-card-title>
    <v-card-text>
        <p>{{ product.description }}</p>
        <p>{{ product.price }}</p>
        <p>{{ product.userName }}</p>
    </v-card-text>
    <v-card-actions>
        <v-btn icon :to="{name: 'details', params: {id: product.id}}" v-if="details===true"><v-icon>mdi-card-bulleted</v-icon></v-btn>
        <v-btn icon :to="{name: 'update', params: {id: product.id}}" v-if="update===true && user.name===product.userName"><v-icon>mdi-pencil</v-icon></v-btn>
        <v-btn icon :to="{name: 'delete', params: {id: product.id}}" v-if="requestdelete===true && user.name===product.userName"><v-icon>mdi-delete</v-icon></v-btn>
        <v-btn @click="deleteProduct" color="warning" v-if="confirmdelete===true && user.name===product.userName">DELETE PRODUCT</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import checkSecMixin from "@/checksec"
import datalayer from '@/datalayer'
export default {
    mixins:[checkSecMixin],
    props: {
        product : Object,
        details: {type: Boolean, default: false},
        update: {type: Boolean, default: false},
        requestdelete: {type: Boolean, default: false},
        confirmdelete: {type: Boolean, default: false}
    },
    methods: {
        async deleteProduct () {
            await datalayer.deleteProduct(+this.product.id)
            this.$router.push({name: 'home'})
        }
    }
}
</script>

<style>

</style>