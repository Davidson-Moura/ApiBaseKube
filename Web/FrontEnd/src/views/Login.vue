<template>
  <div class="bg-animate">
    <v-container fluid class="fill-height">
      <v-row justify="center" align="center" class="fill-height">
        <v-col cols="12" sm="8" md="4">
          <v-img src="/imgs/LoginImage.png" v-if="false" />
        </v-col>
        
        <v-col cols="12" sm="8" md="4">
          <v-card class="elevation-12 pa-5">
            <v-card-title class="d-flex align-center justify-center">
              <h3 v-t="'label.loginWelcome'" class="d-inline"></h3>
              <v-icon icon="mdi-plus" color="#A5C749" />
            </v-card-title>
            <v-card-text>
              <v-form ref="form" v-model="valid" @submit.prevent="login">

                <v-text-field v-model="email" :label="$t('label.email')" type="email"
                  prepend-inner-icon="mdi-email-outline" variant="outlined" required :rules="emailRules"></v-text-field>

                <v-text-field class="mt-2" v-model="password" :label="$t('label.password')" required
                  :rules="passwordRules" prepend-inner-icon="mdi-lock-outline" variant="outlined"
                  hide-details="auto" 
                  :append-inner-icon="viewPassword ? 'mdi-eye' : 'mdi-eye-off'"
                  @click:append-inner="viewPassword = !viewPassword"
                  :type="viewPassword ? '' : 'password'"
                  @keyup.enter="login"></v-text-field>

                <div class="d-flex">
                  <v-btn :disabled="!valid" color="success" rounded
                    class="mt-4" style="margin-left: auto;" type="submit"
                    append-icon="mdi-login"> {{ $t('label.toLogin') }} </v-btn>
                </div>
              </v-form>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script>
export default {
  data() {
    return {
      valid: false,
      inRequest: false,

      apps: [],

      email: '',
      password: '',
      appId: '',

      viewPassword: false,

      emailRules: [v => !!v || this.$t('message.EmailIsRequired'),
      v => !v.includes("@") || /.+@.+\..+/.test(v) || this.$t('message.EmailInvalid')],
      passwordRules: [v => !!v || this.$t('message.PasswordIsRequired'), v => (v && v.length >= 6) || this.$t('message.PasswordDoesNotHaveTheMinimumNumberOfCharacters')],
      appIdRules: [v => !!v || this.$t('message.TheFieldIsRequired')]
    };
  },
  mounted() {
    this.onLogout();
  },
  methods: {
    login() {
      if (this.valid) {
        if (this.inRequest) return;

        this.inRequest = true;
        this.$MyApp.setLoading(true);

        let loginData = {
          AppId: this.appId,
          Login: this.email,
          Password: this.password
        };

        this.$axios.post(this.$api.login, loginData, { withCredentials: true }).then(response => {
          this.inRequest = false;
          this.$axios.prototype.Token = response.data.Token;
          this.$store.commit("sessionModule/setSessionManager", response.data);

          this.$MyApp.setLoading(false);
          this.$MyApp.success(this.$t('message.LoginSuccess'));

          Promise.all([
              this.$MyApp.loadMainMenus(true),
              this.$MyApp.loadPermissions(true)
          ])
          .then((values)=>{
            this.afterLogin();
          });
        }).catch((error) => {
          this.inRequest = false;
          this.$MyApp.setLoading(false);
        });
      }
    },
    afterLogin(){
      this.inRequest = false;
      let route = this.$route.params?.redirectUrl;
      let id = this.$route.params?.id;
      if (route && id) {
        this.$router.push({ name: route, params: { id: id } });
      } else {
        this.$router.push({ name: 'Home' });
      }
    },
    navTo(route) {
      this.$router.push({ name: route });
    },
    onLogout() {
      if (!this.$MyApp.isLogged()) return;

      this.$MyApp.logout();
      this.removeCookies();

      this.$MyApp.success(this.$t('message.LogoutSuccess'));
    },
    removeCookies(){
      document.cookie.split(";").forEach(cookie => {
        const eqPos = cookie.indexOf("=");
        const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name.trim() + "=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/";
      });
    }
  }
};
</script>

<style scoped>

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
