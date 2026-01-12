<template>
  <v-container fluid class="mt-3">
    <v-row>
      <v-col cols="12">
        <v-card class="pa-4" elevation="2" style="text-align: center;">
          <v-card-title class="text-h5 font-weight-bold"> {{ $t('label.welcomeToProject', {
            projectName: $t('label.projectName')
          }) }}</v-card-title>
          <v-card-subtitle class="text-body-1" style="text-wrap: auto;">
            {{ $t('message.HereYouWillFindAllTheToolsYouNeedToManageYourWorksAndResources') }}</v-card-subtitle>
        </v-card>
      </v-col>
    </v-row>

    <!--
    <div class="d-flex flex-wrap ga-2 mt-2 justify-center">
        <template v-for="(tile, index) in groupedTiles" :key="index">
          <v-card class="tile-card" @click="navigate(tile.route)" hover 
            :width="185 * tile.sized + (tile.sized > 1 ? tile.sized * 4 : 0)" height="185"
            style="display: flex; flex-direction: column;">
            <v-card-title class="d-flex align-center justify-start mt-4">
              <v-icon size="large" color="black">{{ tile.icon }}</v-icon>
              <h6 class="text-center font-weight-bold ml-1 text-6" 
                style="text-wrap: auto; font-size: medium; align-items: center;">
                {{ $t(tile.title) }}
              </h6>
            </v-card-title>
            
              <v-card-subtitle >

                
              </v-card-subtitle>
              <v-card-text class="pa-4 d-flex align-end justify-space-between" >
                <h4 class="text-h4">
                  {{tile.count}}
                </h4>
                <v-btn @click.stop.prevent="navigate(tile.actionRoute)" v-if="tile.actionRoute"> 
                  {{ $t(tile.actionTitle) }} 
                </v-btn>
              </v-card-text>
            </v-card>
        </template>

</div>
-->

    <div class="d-flex flex-wrap ga-2">
      <template v-for="(group) in groupedTiles" :key="group.title">
        <div :class="isSmall ? 'ma-1' : 'ma-2'">
          <h3 class="text-h5 font-weight-bold" style="display: flex;">{{ $t(group.title) }}</h3>
          <div class="d-flex flex-wrap ga-2">
            <template v-for="(tile, index) in group.list" :key="index">
  
              <v-card class="tile-card" @click="navigate(tile.route)" hover
                :width="cardSize * tile.sized + (tile.sized > 1 ? tile.sized * 4 : 0)" :height="cardSize"
                style="display: flex; flex-direction: column;">
                <v-card-title class="d-flex align-center justify-start mt-4">
                  <v-icon size="large" color="black">{{ tile.icon }}</v-icon>
                  <h6 class="text-center font-weight-bold ml-1 text-6"
                    style="text-wrap: auto; font-size: medium; align-items: center;">
                    {{ $t(tile.title) }}
                  </h6>
                </v-card-title>
  
                <v-card-subtitle>
  
  
                </v-card-subtitle>
                <v-card-text :class="(isSmall ? '' : 'pa-4') + 'd-flex align-end justify-space-between'">
                  <h4 class="text-h4">
                    {{ tile.count }}
                  </h4>
                  <v-btn @click.stop.prevent="navigate(tile.actionRoute)" v-if="tile.actionRoute">
                    {{ $t(tile.actionTitle) }}
                  </v-btn>
                </v-card-text>
              </v-card>
            </template>
  
          </div>
        </div>
      </template>
    </div>

    <!--
    <template v-for="(group) in groupedTiles" :key="group.title">
      <v-row class="mb-1 mt-1">
        <v-col cols="12">
          <h3 class="text-h5 font-weight-bold">{{ $t(group.title) }}</h3>
        </v-col>
      </v-row>

      <div class="d-flex flex-wrap ga-2">
        <template v-for="(tile, index) in group.list" :key="index">
          <template v-if="tile.list && tile.list.length > 0">
            <v-card @click="navigate(subTile?.route)" color="transparent" elevation="0">

              <v-card-title>
                {{ $t(tile.title) }}
              </v-card-title>

              <v-card-text class="d-flex flex-wrap ga-2">
                <template v-for="(subTile, index) in tile.list" :key="index">
                  <v-card class="tile-card" @click="navigate(subTile.route)" hover width="160" height="160">
                    <v-card-title class="text-center">
                      <v-icon size="x-large">{{ subTile.icon }}</v-icon>
                    </v-card-title>
                    <v-card-subtitle class="text-center text-h6 font-weight-bold" style="text-wrap: auto;">
                      {{ $t(subTile.title) }}
                    </v-card-subtitle>
                  </v-card>
                </template>
              </v-card-text>
            </v-card>
          </template>
          <template v-else>
            <v-card class="tile-card" @click="navigate(tile.route)" hover width="160" height="160">
              <v-card-title class="text-center">
                <v-icon size="x-large">{{ tile.icon }}</v-icon>
              </v-card-title>
              <v-card-subtitle class="text-center text-h6 font-weight-bold" style="text-wrap: auto;">
                {{ $t(tile.title) }}
              </v-card-subtitle>
            </v-card>
          </template>
        </template>

        
      </div>
    </template>
    -->

  </v-container>
</template>

<script setup>
import { ref, getCurrentInstance, onMounted } from "vue";
import { useRouter } from 'vue-router';

const instance = getCurrentInstance();
const global = instance?.appContext.config.globalProperties;
const myApp = global.$MyApp;

const isSmall = ref(myApp.getScreenSize() == 'sml');
const cardSize = ref(isSmall ? 170 : 160);

const router = useRouter();
const mainMenu = ref([]);

const groupedTiles = ref(mainMenu);

const navigate = (route) => {
  if (!route) return;
  router.push({ name: route });
};

const loadCountMainMenus = () => {
  myApp.loadCountMainMenus().then(menu => {
    mainMenu.value = [];
    mainMenu.value = menu;
  });
}

const loadMenu = () => {
  myApp.loadMainMenus().then(menu => {
    mainMenu.value = menu;
    loadCountMainMenus();
  });
}

onMounted(() => {
  loadMenu();
})

</script>

<style scoped>
.tile-card {
  /*
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, rgba(var(--primary-background-color)), rgba(var(--secondary-background-color)));
  color: white;
  */

  flex-direction: column;
  cursor: pointer;
  transition: 0.3s;
}

.background-gradient-default {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, rgba(var(--primary-background-color)), rgba(var(--secondary-background-color)));
  color: white;
}

.tile-card:hover {
  transform: scale(1.05);
}

.font-color {
  background-color: rgb(var(--v-theme-on-surface)) !important;
}
</style>
