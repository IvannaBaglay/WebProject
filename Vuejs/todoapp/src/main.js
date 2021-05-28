import Vue from "vue";
import "./plugins/vuetify.js";
import VueRouter from "vue-router";

import axios from "axios";

import App from "./App.vue";
import Login from "./components/Auth/Login";
import Signup from "./components/Auth/Singup";
import Todo from "./components/Todo";
import Tasks from "./components/Tasks";
import NotesModal from "./components/NotesModal";

Vue.config.productionTip = false;

Vue.use(VueRouter);

axios.defaults.baseURL = "http://localhost:8000/api/";
axios.defaults.withCredentials = true;

const routes = [
  {
    path: "/",
    component: Todo,
    name: "todo",
    children: [
      {
        path: "list/:id",
        components: { tasks: Tasks },
        name: "tasks",
        children: [
          {
            path: "task/:taskId",
            components: { notes: NotesModal },
            name: "notes"
          }
        ]
      }
    ]
  },
  {
    path: "/login",
    component: Login,
    name: "login"
  },
  {
    path: "/signup",
    component: Signup,
    name: "signup"
  }
];

const router = new VueRouter({
  mode: "history",
  routes,
  base: "/"
});

new Vue({
  router,
  render: h => h(App)
}).$mount("#app");