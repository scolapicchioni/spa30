<template>
  <h3>Please wait, you are being redirected to the Home page</h3> 
</template>

<script>
import applicationUserManager from '../applicationusermanager'

export default {
  async created () {
    try {
      await applicationUserManager.signinRedirectCallback()
      this.$router.push({name: 'home'})
    } catch (e) {
      console.log(e)
    }
  }
}
</script>

<style>

</style>