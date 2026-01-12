<template>
    <v-dialog 
        :modelValue="isOpen" @update:modelValue="(value) => this.$emit('update:isOpen', value)"
         max-width="400">
        <v-card>
            <v-card-title class="headline">{{ ( title ? title : $t('label.confirmOperation') )}}</v-card-title>
            <v-card-text>{{ msg }}</v-card-text>

            <div class="ma-2">
                <slot name="content" >
                </slot>
            </div>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error" variant="elevated" @click="onOption1Internal">{{ ( option1 ? option1 : $t('label.cancel') ) }}</v-btn>
                <v-btn color="success" variant="elevated" @click="onOption2Internal">{{ ( option2 ? option2 : $t('label.continue') ) }}</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
export default {
    props: [
        "title", "msg", "option1", "option2",
        "isOpen", "onOption1", "onOption2"
    ],
    emits: ['update:isOpen'],
    data() {
        return {};
    },
    methods: {
        onOption1Internal(){
            if(this.onOption1) this.onOption1();
            this.close();
        },
        onOption2Internal(){
            if(this.onOption2) this.onOption2();
            this.close();
        },
        close(){
            this.$emit('update:isOpen', false);
        }
    },
};
</script>