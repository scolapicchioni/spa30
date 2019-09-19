import Vue from 'vue'
import Vuetify from 'vuetify/lib'

Vue.use(Vuetify)

export default new Vuetify({
  theme: {
    themes: {
      light: {
        'primary': '#ff9800',
        'secondary': '#cddc39',
        'accent': '#8bc34a',
        'error': '#f44336',
        'warning': '#e91e63',
        'info': '#00bcd4',
        'success': '#795548',
        'anchor': '#cddc39'
      }
    }
  },
  icons: {
    iconfont: 'mdi'
  }
})
