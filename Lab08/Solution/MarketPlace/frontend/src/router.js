import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import Create from './views/Create.vue'
import Details from './views/Details.vue'
import Update from './views/Update.vue'
import Delete from './views/Delete.vue'
import CallBack from './views/LoginCallBack.vue'

Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/create',
      name: 'create',
      component: Create
    },
    {
      path: '/details/:id',
      name: 'details',
      component: Details
    },
    {
      path: '/update/:id',
      name: 'update',
      component: Update
    },
    {
      path: '/delete/:id',
      name: 'delete',
      component: Delete
    },
    {
      path: '/callback',
      name: 'callback',
      component: CallBack
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "about" */ './views/About.vue')
    }
  ]
})
