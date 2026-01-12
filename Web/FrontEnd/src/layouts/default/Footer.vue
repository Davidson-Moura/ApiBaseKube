<template>
   <v-footer app elevation="10" color="primary">
      <v-col class="text-center" @click="navAdmin">
        &copy; {{new Date().getFullYear()}}. Todos os direitos reservados.
      </v-col>
    </v-footer>
</template>
<script>
export default {
  props: [
    'setting'
  ],
  data: () => ({

  }),
  methods:{
    navAdmin(){
      if(this.isLogged) return;
      this.$router.push({name: 'AdminLogin'});
    }
  },
  computed: {
    /*
    iframeMap() {
      if (!this.setting?.MapIFrame) return "";
      document.getElementById('mapIFrame').innerHTML = this.setting.MapIFrame;
      return "";
    },
    */

    //notIsSuperAndIsLogged(){ return !this.isSuper && this.isLogged; },
    //isSuper(){ const store = useStore(); return store.getters['sessionModule/IsSuper'] },
    isLogged() { return this.$store?.getters['sessionModule/IsLogged'] },
  }
}
</script>
<style>
</style>