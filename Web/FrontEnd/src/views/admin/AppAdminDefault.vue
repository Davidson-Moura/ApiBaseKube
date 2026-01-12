<template>
<router-view />
</template>

<script>
import { defineComponent } from 'vue'

export default defineComponent({
    components:{
    },
    data(){
    return {
    }
    },
    created(){
        this.$Session.init();
    },
    updated(){
        this.$Session.verifyConnection();
    },
})

</script>
