<template>
    <v-btn icon @click="login" v-if="!user.isAuthenticated">
      <v-icon>mdi-account</v-icon>
    </v-btn>
    <v-btn text @click="logout" v-else>
        {{ user.name }}
      <v-icon>mdi-account-off</v-icon>
    </v-btn>
</template>

<script>
import checkSecMixin from "../checksec"
import applicationUserManager from '../applicationusermanager'
export default {
  mixins:[checkSecMixin],
  methods: {
    async login () {
      try {
        await applicationUserManager.login()
      } catch (error) {
        console.log(error)
      }
    },
    async logout () {
      try {
        await applicationUserManager.logout()
      } catch (error) {
        console.log(error)
      }
    }
  }
}
</script>

<style>

</style>