<template>
  <v-container fluid class="fill-height bg-animate">
    <v-row justify="center" align="center" class="fill-height">
      <v-col cols="12" sm="8" md="4">
        <v-card class="elevation-12 pa-5">
          <v-card-title v-t="'label.areaAdmin'"></v-card-title>
          <v-card-text>
            <v-form ref="form" v-model="valid" @submit.prevent="onLogin">
              <v-text-field v-model="loginData.Login" :label="$t('label.login')" type="login"
                prepend-inner-icon="mdi-email-outline" variant="outlined" required :rules="loginRules"></v-text-field>

              <v-text-field class="mt-2" v-model="loginData.Password" :label="$t('label.password')" type="password"
                required :rules="passwordRules" prepend-inner-icon="mdi-lock-outline" variant="outlined"
                @keyup.enter="onLogin"></v-text-field>

              <div class="d-flex">
                <v-btn :disabled="!valid" color="success" class="mt-4" style="margin-left: auto;" type="submit"
                  append-icon="mdi-login"> {{ $t('label.toLogin') }}</v-btn>
              </div>

            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>

</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  components: {
  },
  data() {
    return {
      valid: false,
      inRequest: false,

      loginData: {
        Login: "",
        Password: ""
      },
      loginRules: [
        value => {
          if (value?.length >= 5) return true;
          return this.$t("message.LoginDoesNotHaveTheMinimumNumberOfCharacters");
        },
      ],
      passwordRules: [
        value => {
          if (value?.length > 4) return true;

          return this.$t('message.PasswordDoesNotHaveTheMinimumNumberOfCharacters');
        },
      ],
    };
  },
  methods: {
    onLogin() {
      if (this.inRequest) return;

      if (!this.loginData.Login || !this.loginData.Password) {
        this.$MyApp.error(this.$t('message.LoginOrPasswordIsEmpty'));
        return;
      }

      this.inRequest = true;
      this.$MyApp.setLoading(true);
      this.$axios.post(this.$api.adminLogin, this.loginData).then(response => {
        this.inRequest = false;
        this.$MyApp.setLoading(false);

        this.$axios.prototype.Token = response.data.Token;

        this.$store.commit("sessionModule/setSessionManager", response.data);
        this.$MyApp.success(this.$t('message.LoginSuccess'));
        this.$router.push({ name: 'AdminHome' });
      }).catch((error) => {
        this.inRequest = false;
        this.$MyApp.setLoading(false);
      });
    },
    onLogout() {
      if (!this.$MyApp.isLogged()) return;

      this.$axios.get("Authentication/Logout/v1").then(() => { });
      this.$store.commit('sessionModule/logout');

      this.$MyApp.success(this.$t('message.LogoutSuccess'));
    }
  },
  mounted() {
    this.onLogout();
  },
  computed: {
  }
});
</script>

<style lang="scss" scoped>

.bg-animate {
  margin: 0;
  height: 100vh;
  background: linear-gradient(-45deg, #0e1218, #00be95, #3d69a3, #1a3c77);
  background-size: 400% 400%;
  animation: gradient 15s ease infinite;
}

@keyframes gradient {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

</style>