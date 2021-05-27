<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-flex xs12 sm8 md8>
        <v-card class="elevation-12">
          <v-toolbar dark color="blue">
            <v-toolbar-title>Signup form</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            
          </v-card-text>
          <v-divider light></v-divider>
          <v-card-actions>
            <v-btn to="/login" round color="black" dark>Login</v-btn>
            <v-spacer></v-spacer>
            <v-btn round color="success" @click.prevent="register()">
              Register
              <v-icon>keyboard_arrow_up</v-icon>
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
};
</script>
