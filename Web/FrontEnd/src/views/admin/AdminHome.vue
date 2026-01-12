<template>
  <v-container fluid class="pa-6" style="min-height: 100vh;">
    
    <v-row mb-6>
      <v-col cols="12">
        <v-card flat class="bg-transparent">
          <div class="d-flex align-center">
            <v-avatar color="primary" size="64" class="elevation-2 mr-4">
              <v-icon icon="mdi-hospital-building" color="white" size="32"></v-icon>
            </v-avatar>
            <div>
              <h1 class="text-h4 font-weight-bold text-primary">
                {{ $t('label.welcomeToProject', { projectName: $t('label.projectName') }) }}
              </h1>
              <p class="text-subtitle-1 text-grey-darken-1">
                {{ $t('message.HereYouWillFindAllTheToolsYouNeedToManageYourWorksAndResources') }}
              </p>
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <template v-for="group in groupedTiles" :key="group.title">
      <v-row class="mt-6">
        <v-col cols="12">
          <div class="d-flex align-center mb-4">
            <v-divider class="mr-4"></v-divider>
            <h2 class="text-h6 font-weight-medium text-grey-darken-2 text-uppercase" style="letter-spacing: 1px;">
              {{ $t(group.title) }}
            </h2>
            <v-divider class="ml-4"></v-divider>
          </div>
        </v-col>
      </v-row>

      <v-row>
        <v-col 
          v-for="(tile, index) in group.list" 
          :key="index"
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <v-card
            class="rounded-xl pa-2 transition-swing"
            elevation="1"
            hover
            @click="navigate(tile.route)"
            style="border: 1px solid #e0e0e0;"
          >
            <v-list-item class="mb-2">
              <template v-slot:prepend>
                <v-avatar :color="tile.color || 'blue-lighten-5'" rounded="lg">
                  <v-icon :color="tile.iconColor || 'primary'">{{ tile.icon }}</v-icon>
                </v-avatar>
              </template>
              <v-list-item-title class="font-weight-bold text-grey-darken-3">
                {{ $t(tile.title) }}
              </v-list-item-title>
            </v-list-item>

            <v-card-text class="d-flex align-center justify-space-between pt-0">
              <div>
                <span class="text-h4 font-weight-black text-primary">{{ tile.count }}</span>
                <span class="text-caption d-block text-grey">Registros ativos</span>
              </div>
              
              <v-btn
                v-if="tile.actionRoute"
                variant="tonal"
                color="primary"
                rounded="pill"
                size="small"
                append-icon="mdi-arrow-right"
                @click.stop.prevent="navigate(tile.actionRoute)"
              >
                {{ $t(tile.actionTitle || 'Ver mais') }}
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </template>

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
