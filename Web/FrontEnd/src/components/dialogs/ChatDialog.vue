<!-- src/components/Chat.vue -->
<template>
  <v-card class="chat-card" height="40rem" elevation="8" >
        <!--
        <v-card-title class="bg-primary text-white">Chat</v-card-title>
        <v-card-text class="chat-messages">
          <div v-for="(msg, index) in messages" :key="index" class="mb-2">
            <strong>{{ msg.sender }}:</strong> {{ msg.text }}
          </div>
        </v-card-text>
        <v-card-actions>
          <v-text-field
          v-model="newMessage"
          placeholder="Digite uma mensagem"
          dense
          hide-details
          class="flex-grow-1"
          @keyup.enter="sendMessage"
          />
          <v-btn icon @click="sendMessage">
            <v-icon>mdi-send</v-icon>
          </v-btn>
        </v-card-actions>
        -->
      <iframe src="https://copilotstudio.microsoft.com/environments/Default-81811d0e-aaf3-433c-b65e-096fedf82c36/bots/cr987_explicadorDeSistema/webchat?__version__=2" frameborder="0" style="width: 100%; height: 100%;"></iframe>
    </v-card>
</template>

<script setup>
import { ref } from 'vue';

const messages = ref([
  { sender: 'Bot', text: 'Olá! Como posso ajudar?' }
]);

const newMessage = ref('');

function sendMessage() {
  if (newMessage.value.trim() !== '') {
    messages.value.push({ sender: 'Você', text: newMessage.value });
    newMessage.value = '';
  }
}
</script>

<style scoped>
.chat-card {
  position: fixed;
  bottom: 16px;
  right: 16px;
  width: 300px;
  max-height: 400px;
  display: flex;
  flex-direction: column;
  z-index: 9999;
}
.chat-messages {
  overflow-y: auto;
  height: 250px;
}
</style>
