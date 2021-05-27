<template>
  <v-app>
    <Todo/>
  </v-app>
</template>

<script>
import Vuetify from 'vuetify'
//import Login from "./components/Auth/Login"
//import Singup from "./components/Auth/Singup"
import Todo from "./components/Todo"

export default {
  name: 'app',
  vuetify: new Vuetify(),
  components: {
    Todo
  }
}
</script>