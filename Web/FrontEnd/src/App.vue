<template>
  <v-overlay :persistent="true" :open-delay="1" :close-delay="1" :no-click-animation="true" :eager="true"
    :model-value="inLoading" class="align-center justify-center">
    <v-progress-circular color="primary" indeterminate size="64"></v-progress-circular>
  </v-overlay>
  
  <router-view />

  <!--
  <Chat v-show="showChat" />
  <v-btn icon color="info" :class="showChat ? 'chat-toggle-btn-2' : 'chat-toggle-btn'" @click="showChat = !showChat"
    size="small">
    <v-icon>{{ showChat ? 'mdi-close' : 'mdi-chat' }}</v-icon>
  </v-btn>
  -->
  <Alert />
</template>

<script>
import { defineComponent } from 'vue';
import Alert from '@/layouts/default/components/Alert.vue';
//import Chat from '@/components/dialogs/ChatDialog.vue';

export default defineComponent({
  components: {
    Alert,
    //Chat
  },
  data() {
    return {
      showChat: false,
      overlay: true
    }
  },
  created() {
    this.$store.dispatch('sessionModule/loadLocalStorage');
  },
  mounted() {
    this.overlay = false;
  },
  computed: {
    inLoading() {
      return (this?.$store?.state?.applicationModule?.ExecuteLoadComponent??false) 
        || 
        this.overlay;
    }
  }
})

</script>

<style scoped>
.chat-toggle-btn {
  position: fixed;
  bottom: 6rem;
  right: 16px;
  z-index: 10000;
}

.chat-toggle-btn-2 {
  position: fixed;
  bottom: 23.4rem;
  right: 16px;
  z-index: 10000;
}
</style>
