<template>
  <v-app-bar app color="primary" >

    <v-app-bar-nav-icon v-if="isLogged && $vuetify.display.mobile && !IsAppAdmin" 
     @click="() => this.$emit('update:IsDrawer', !IsDrawer)"></v-app-bar-nav-icon>

     <img style="cursor:pointer; margin: 0.5rem;width: 3rem;"
      v-on:click="() => isLogged && !IsAppAdmin ? onNav('Home') : onNav('Login')" 
      :src="isLogged && !IsAppAdmin ? $MyApp.getLogoUrl() : $MyApp.getLogoUrl()">

    <v-app-bar-title style="width:100%; cursor:pointer;"
      v-on:click="() => isLogged && !IsAppAdmin ? onNav('Home') : onNav('Login')"> {{ ApplicationName }} </v-app-bar-title>
    
    <v-spacer></v-spacer>

    <v-btn icon v-if="isLogged" @click="openNotification">
      <v-icon 
        :icon="!withNotification ? 'mdi-bell' : 'mdi-bell-badge'" 
        :color="!withNotification ? '' : 'info'"></v-icon>
      <v-tooltip activator="parent" location="top">{{ $t('label.notifications') }}</v-tooltip>
    </v-btn>

    <v-btn icon v-if="!isLogged" @click="onNav('AdminLogin')">
      <v-icon icon="mdi-login" ></v-icon>
      <v-tooltip activator="parent" location="top">{{ $t('label.adminLogin') }}</v-tooltip>
    </v-btn>

    

    <v-btn icon @click="onLogout" v-if="isLogged">
      <v-icon icon="mdi-logout"></v-icon>
      <v-tooltip activator="parent" location="top">{{ $t('label.logout') }}</v-tooltip>
    </v-btn>
  </v-app-bar>
</template>

<script>
export default {
  components: {
  },
  props: {
    LogoExtension: {
      type: String,
      required: true
    },
    ApplicationName: {
      type: String,
      required: true
    },
    IsDrawer: {
      type: Boolean,
      required: true
    },
    IsDrawnRight: {
      type: Boolean,
      required: true
    },
  },
  emits: [ 'update:IsDrawer', 'update:IsDrawnRight', 'expandDrawnRight' ],
  data: () => ({
    screen: {},
    loading: false,
  }),
  methods: {
    onNav(name) {
      this.$router.push({ name: name });
    },
    onLogout() {
      if(!this.$MyApp.getUser()) return;
      document.cookie = "AuthToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
      this.$axios.get("Authentication/Logout/v1").then(() => {
        this.$store.commit('sessionModule/logout');
      });

      this.onNav('Login');

      this.$MyApp.success(this.$t('message.LogoutSuccess'));
    },
    openNotification(){
      this.$MyApp.readNotification();
      this.$emit('update:IsDrawnRight', !this.IsDrawnRight);

      if(!this.IsDrawnRight) 
      setTimeout(() => {
        this.$emit('expandDrawnRight');
      }, 1000);
    }
  },
  mounted() {
    this.screen = screen;
  },
  computed: {
    countCart() {
      let count = this.$store.state.cartModule.cart.Products?.length;
      return count;
    },
    notIsSuperAndIsLogged() { return !this.isSuper && this.isLogged; },
    isSuper() { return this.$store?.getters['sessionModule/IsSuper'] },
    isLogged() { return this.$store?.getters['sessionModule/IsLogged'] },
    IsAppAdmin() { return this.$store?.getters['sessionModule/IsAppAdmin'] },
    withNotification() { return this.$store?.getters['alertModule/WithNotification'] }
  },
}
</script>
<style scoped>
</style>