<template>
  <v-app>
    <Singup/>
  </v-app>
</template>

<script>
import Vuetify from 'vuetify'
//import Login from "./components/Auth/Login"
import Singup from "./components/Auth/Singup"

export default {
  name: 'app',
  vuetify: new Vuetify(),
  components: {
    Singup
  }
}
</script>