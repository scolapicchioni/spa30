import { UserManager } from 'oidc-client'

class ApplicationUserManager extends UserManager {
  constructor () {
    super({
      authority: 'http://localhost:5000',
      client_id: 'frontend',
      redirect_uri: 'http://localhost:8080/callback',
      response_type: 'code',
      scope: 'openid profile backend',
      post_logout_redirect_uri: 'http://localhost:8080'
    })
  }

  async login () {
    await this.signinRedirect()
  }

  async logout () {
    return this.signoutRedirect()
  }
}

const applicationUserManager = new ApplicationUserManager()
export { applicationUserManager as default }
