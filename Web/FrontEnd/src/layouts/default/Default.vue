<template>
  <v-app>
    <default-bar :LogoExtension="setting.LogoExtension" :ApplicationName="setting.ApplicationName"
      v-model:IsDrawer="isDrawn" v-model:IsDrawnRight="isDrawnRight" @expandDrawnRight="() => $refs.drawnRight.initLoad()" />

    <v-navigation-drawer app v-model="isDrawn" v-if="!isAppAdmin && isLogged" :rail="!isMobile"
      color="blue-grey-darken-4" theme="dark"
      @mouseenter="isHovered = true" @mouseleave="isHovered = false" :expand-on-hover="!isMobile" 
      :temporary="isMobile">

      <v-list>
        <v-list-item prepend-icon="mdi-account" :title="currentSession.Name" :subtitle="currentSession.Email"
          @click="openUserDefinitions"></v-list-item>
      </v-list>
      <v-divider></v-divider>

      <v-list density="compact" nav>

        <template v-for="menu, index in mainMenu" :key="index" v-model:opened="open">
          <v-list-group :value="$t(menu.title)">
            <template v-slot:activator="{ props }">
              <v-list-item v-bind="props" :prepend-icon="menu.icon" :title="$t(menu.title)">
              </v-list-item>
            </template>

            <div style="" class="mb-2">
            <template v-for="subMenu, index2 in menu.list" :key="`${index}-${index2}`">

              <template v-if="subMenu.list && subMenu.list.length > 0">
                <v-list-group :value="`${index}-${index2}`">

                  <template v-slot:activator="{ props }">
                    <v-list-item v-bind="props" :prepend-icon="subMenu.icon"
                       :title="$t(subMenu.title)"
                      style="padding-left: 0.5rem !important;">
                    </v-list-item>
                  </template>
                  <div style="border-top: 1px solid black; border-bottom: 1px solid black;" class="mb-2">
                    <div v-for="subMenu2, index2 in subMenu.list" :key="`${index}-${index2}`">
                      <template v-if="subMenu2.list && subMenu2.list.length > 0">
                        <v-list-group :value="`${index}-${index2}`">
                          <template v-slot:activator="{ props }">
                            <v-list-item v-bind="props" :prepend-icon="isHovered ? '' : subMenu2.icon"
                              :append-icon="isHovered ? subMenu2.icon : ''" :title="isHovered ? $t(subMenu2.title) : ''"
                              style="padding-left: 1rem !important;">
                            </v-list-item>
                          </template>
                        </v-list-group>
                      </template>
                      <template v-else>
                        <v-list-item 
                          :prepend-icon="isHovered ? '' : subMenu2.icon"
                          :append-icon="isHovered ? subMenu2.icon : ''" :title="isHovered ? $t(subMenu2.title) : ''" active
                          style="padding-left: 0.6rem !important;" @click="onNav(subMenu2.route)">
                        </v-list-item>
                      </template>

                    </div>
                  </div>

                </v-list-group>
              </template>

              <template v-else>
                <v-list-item 
                :prepend-icon="isHovered ? '' : subMenu.icon"
                  :append-icon="isHovered ? subMenu.icon : ''"
                 :title="$t(subMenu.title)"
                  style="padding-left: 0.5rem !important;" @click="onNav(subMenu.route)">
                </v-list-item>
              </template>
            </template>
          </div>


          </v-list-group>
          <v-divider :thickness="3"></v-divider>
        </template>

      </v-list>

    </v-navigation-drawer>

    <NotificationDrawer v-if="!isAppAdmin && isLogged" v-model="isDrawnRight" ref="drawnRight" />

    <v-main>

      <default-view />
    </v-main>

    <DefaultFooter :setting="setting" />
  </v-app>
</template>

<script>
import DefaultBar from './AppBar.vue';
import DefaultView from './View.vue';
import DefaultFooter from './Footer.vue';
import NotificationDrawer from './NotificationDrawer.vue';

import { computed } from 'vue';

export default {
  data() {
    return {
      isMobile: false,
      isDrawn: false,
      isHovered: false,

      isDrawnRight: false,

      mainMenu: [],


      setting: {
        LogoExtension: ".svg",
        ApplicationName: "Clinic+",
        Currency: ""
      },

      designLoaded: false,
    }
  },
  components: {
    DefaultBar,
    DefaultView,
    DefaultFooter,
    NotificationDrawer
  },
  provide() {
    return {
      currency: computed(() => this.setting.Currency),
      setShowCateoryDrawer: () => this.showCateoryDrawer = !this.showCateoryDrawer,
      showCateoryDrawer: computed(() => this.showCateoryDrawer)
    }
  },
  computed: {
    isSuper() { return this.$store?.getters['sessionModule/IsSuper'] },
    isAppAdmin() { return this.$store?.getters['sessionModule/IsAppAdmin'] },
    isLogged() { return this.$store?.getters['sessionModule/IsLogged'] },
    currentSession() { return this.$store?.getters['sessionModule/GetSession'] },
  },
  created() {
    this.isMobile = this.$vuetify.display.mobile;
    this.isDrawn = !this.isMobile;
  },
  updated(){
    if(this.isLogged) {
      this.verifyMenu();
      this.verifyPermissions();
    }
  },
  methods: {
    verifyMenu(){
      this.$MyApp.loadMainMenus().then((menu)=>{
        this.mainMenu = menu;
      });
    },
    verifyPermissions(){
      this.$MyApp.loadPermissions();
    },
    loadViewDefault() {
      let publicSetting = {
                  PrimaryColor: '0, 40, 40',//'7, 40, 74', //'65,154,232',
                  SecondaryColor: '26, 60, 119', // #1A3C77 // rgb(26, 60, 119)
                  FontColor: "255, 255, 255",
        
                  PrimaryBackgroundColor: "14, 18, 24",
                  SecondaryBackgroundColor: "22, 28, 36",
        
                  SuccessColor: '40, 167, 69',
                  ErrorColor: '226, 0, 3',
                  InfoColor: '23, 162, 184',
                  WarningColor: '255, 193, 7',
        
                  ApplicationName: "Clinic+",
                  LogoExtension: ".png"
                };
      this.setSetting(publicSetting);
      return;
      const logoExt = this.setting.LogoExtension;
      this.setting.LogoExtension = "";

      this.$MyApp.setLogoTab("");

      let uri = this.$api.designSettings;
      this.$axios.get(uri).then(response => {
        /*
                let publicSetting = {
                  PrimaryColor: '0, 40, 40',//'7, 40, 74', //'65,154,232',
                  SecondaryColor: '26, 60, 119', // #1A3C77 // rgb(26, 60, 119)
                  FontColor: "255, 255, 255",
        
                  PrimaryBackgroundColor: "244, 246, 248",
                  SecondaryBackgroundColor: "244, 246, 248",
        
                  SuccessColor: '40, 167, 69',
                  ErrorColor: '226, 0, 3',
                  InfoColor: '23, 162, 184',
                  WarningColor: '255, 193, 7',
        
                  ApplicationName: "Construa+",
                  LogoExtension: ".png"
                };
                */

        let publicSetting = response.data;

        this.setSetting(publicSetting);
        this.setting.LogoExtension = this.setting.LogoExtension;
        this.$MyApp.setLogoTab(this.$MyApp.getLogoUrl());
      }).catch((error) => {

      });
    },

    setSetting(data) {
      if (!data) return;

      this.setting = data;
      this.$store.commit('designSettingModule/setSetting', data);
      this.$vuetify.theme.change('customTheme');
    },
    onNav(route) {
      this.$router.push({ name: route });
    },

    openUserDefinitions() {
      this.oUserDialog = true;
    },
    onLoadDesign(){
      this.loadViewDefault();
      if (this.isLogged && !this.designLoaded) {
        
        this.designLoaded = true;
      }
    }
  },
  mounted() {
    this.onLoadDesign();
    if(this.isLogged) {
      this.verifyMenu();
      this.verifyPermissions();
    }else this.$MyApp.setLogoTab(this.$MyApp.getLogoUrl());
  },
  updated(){
    this.onLoadDesign();
    if (
      this.isLogged
      &&
      (!this.mainMenu || this.mainMenu.length == 0)
    ) this.verifyMenu();
  }
}
</script>

<style lang="scss" scoped>
*,
.v-toolbar__content {
/*
  color: rgba(var(--v-font-color)) !important;
  */
}

.v-navigation-drawer__content {
  overflow: auto !important;
  scrollbar-width: none;
  /* Para Firefox */
}

.v-navigation-drawer__content {
  //display: none !important; /* Para Chrome, Safari e Edge */
}



</style>