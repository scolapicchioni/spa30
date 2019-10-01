import applicationUserManager from './applicationusermanager'

const checksecmixin = {
  data () {
    return {
      user: {
        name: '',
        isAuthenticated: false
      }
    }
  },
  async created () {
    await this.refreshUserInfo()
  },
  watch: {
    async '$route' (to, from) {
      await this.refreshUserInfo()
    }
  },
  methods: {
    async refreshUserInfo () {
      const user = await applicationUserManager.getUser()
      if (user) {
        this.user.name = user.profile.name
        this.user.isAuthenticated = true
      } else {
        this.user.name = ''
        this.user.isAuthenticated = false
      }
    }
  }
}

export default checksecmixin
