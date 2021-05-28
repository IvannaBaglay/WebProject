<template>
  <v-navigation-drawer permanent style="width: 100%; overflow: hidden">
    <v-toolbar color="blue" dark>
      <v-toolbar-title v-if="!DISPLAY_SEARCH_LIST">Your lists</v-toolbar-title>
        <SearchBar v-if="DISPLAY_SEARCH_LIST"/>
      <v-spacer></v-spacer>
      <v-btn icon @click.prevent="toggleSearchList()">
        <v-icon>search</v-icon>
      </v-btn>
    </v-toolbar>

    <v-list>
      <v-list-tile color="blue" @click.prevent="openNewListForm()" v-if="!isOpen">
        <v-list-tile-content>
          <v-list-tile-title>Create a new list</v-list-tile-title>
        </v-list-tile-content>

        <v-list-tile-action>
          <v-list-tile-title>
            <v-icon>add</v-icon>
          </v-list-tile-title>
        </v-list-tile-action>
      </v-list-tile>

    </v-list>
    <v-divider></v-divider>
    <v-list style="height: calc(100% - 128px); overflow-y: scroll">
      <v-list-tile
        :to="{ name: 'tasks', params: { id: list.id} }"
        v-for="(list, key) in lists"
        v-bind:key="key"
      >
        <v-list-tile-content>
          <v-list-tile-title>{{ list.title }}</v-list-tile-title>
        </v-list-tile-content>
        <v-list-tile-action>
            <v-list-tile-title>{{ list.tasks }}</v-list-tile-title>
        </v-list-tile-action>
      </v-list-tile>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
import SearchBar from "./SearchBar";

export default {
  name: "lists",
  components: { SearchBar},
  data: () => ({
      lists: [
        {id: 1,
        title: "List1",
        sub_title: "Tested description",
        tasks: 1
        },
        {
        id: 2,
        title: "List2",
        sub_title: "Tested description",
        tasks: 1
        },
        {
        id: 3,
        title: "List3",
        sub_title: "Tested description",
        tasks: 10
        },
      ]


  }),
  
};
</script>
