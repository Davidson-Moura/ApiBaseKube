<template>
    <v-snackbar 
        location="top right" 
        :color="this.notify?.classType"
        multi-line
        v-model="showSnackbarNotify"
        @click="closeNotify"
        >
        {{ notify.message }}
    </v-snackbar>

    <v-snackbar 
        location="bottom" 
        :color="this.alert?.classType" 
        multi-line 
        v-model="showSnackbar" 
        @click="close">
        {{alert.message}}
    </v-snackbar>
</template>
  
<script>
import { mapState } from 'vuex';
import { defineComponent } from 'vue'

export default defineComponent({
    data() {
        return {
        }
    },
    methods: {
        close() {
            this.$store.commit('alertModule/setShowAlert', false);
        },
        open() {
            this.$store.commit('alertModule/setShowAlert', true);
        },

        closeNotify() {
            this.$store.commit('alertModule/setShowNotify', false);
        },
    },
    computed: {
        ...mapState({
            alert: (state) => {
                return state.alertModule.alert;
            },
            notify: (state) => {
                return state.alertModule.notify;
            }
        }),
        showSnackbar: {
            get() {
                return this.alert.show;
            },
            set(value) {
                
            }
        },
        showSnackbarNotify: {
            get() {
                return this.notify.show;
            },
            set(value) {
                
            }
        }
        
    }
})

</script>
<style scoped>
</style>
  