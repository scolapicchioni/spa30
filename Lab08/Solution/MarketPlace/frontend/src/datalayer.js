import applicationUserManager from './applicationusermanager'
const datalayer = {
  serviceUrl: 'https://localhost:44361/api/products',
  async getProducts () {
    const response = await fetch(this.serviceUrl)
    return response.json()
  },

  async getProductById (id) {
    const response = await fetch(`${this.serviceUrl}/${id}`)
    return response.json()
  },

  async insertProduct (product) {
    const user = await applicationUserManager.getUser()
    const response = await fetch(this.serviceUrl, {
      method: 'POST',
      body: JSON.stringify(product),
      headers: new Headers({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + (user ? user.access_token : '')
      })
    })
    let result
    if (response.status !== 201) {
      result = response.statusText
    } else {
      result = await response.json()
    }
    return result
  },

  async updateProduct (id, product) {
    const user = await applicationUserManager.getUser()
    fetch(`${this.serviceUrl}/${id}`, {
      method: 'PUT',
      body: JSON.stringify(product),
      headers: new Headers({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + (user ? user.access_token : '')
      })
    })
  },

  async deleteProduct (id) {
    const user = await applicationUserManager.getUser()
    return fetch(`${this.serviceUrl}/${id}`, {
      method: 'DELETE',
      headers: new Headers({
        'Authorization': 'Bearer ' + (user ? user.access_token : '')
      })
    })
  }
}

export default datalayer
