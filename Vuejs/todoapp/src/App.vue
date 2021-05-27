<template>
  <v-app>
    <Login/>
  </v-app>
</template>

<script>
import Vuetify from 'vuetify'
import Login from "./components/Auth/Login"

export default {
  name: 'app',
  vuetify: new Vuetify(),
  components: {Login
  }
}
</script>