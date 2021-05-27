import Vue from "vue"
import "./plugins/vuetify.js"
import VueRouter from 'vue-router'
import App from "./App.vue"

import Login from './components/Auth/Login'

Vue.config.productionTip = false

Vue.use(VueRouter);

const routes = [
  {
    path: '/login',
    component: Login,
    name: 'login'

  }

]

const router = new VueRouter({
  mode: 'history',
  routes,
  base: '/'

})

new Vue({
  router,
  render: h => h(App),
}).$mount('#app')
