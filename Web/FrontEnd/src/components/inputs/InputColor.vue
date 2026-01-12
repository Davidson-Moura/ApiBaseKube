<template>
    <div class="d-flex" style="margin: 1rem; align-items: center;">
        <v-btn class="inputColor mx-2" :style="`background-color: ${colorRGB};`" @click="()=> open = !open">
            <v-overlay activator="parent" location-strategy="connected" scroll-strategy="none" >
                <v-color-picker :modelValue="internalColor" @update:modelValue="updateColor" :mode="'rgba'" show-swatches></v-color-picker>
            </v-overlay>
        </v-btn>
        <label class="mx-2" >{{label}}</label>
    </div>
</template>
<script>
import { defineComponent } from 'vue';
import formatter from '@/helps/formatter';

export default defineComponent({
    name: 'InputColor',
    props: [
        "color",
        "label"
    ],

    emits: ['update:color'],
    data() {
        return {
            open: false,
            internalColor: '#00000000'
        }
    },
    methods: {
        updateColor(c) {
            this.internalColor = c;
            this.$emit('update:color', formatter.hexToRgba(this.internalColor));
        },
    },
    computed:{
        colorRGB(){
            return formatter.rgbaToHex(this.color);
        }
    }
});
</script>
<style>
.inputColor {
    min-width:1px;
    appearance: none;
    width: 2.2rem !important;
    height: 2.2rem !important;
    cursor: pointer;
    background-color: transparent;
    border: 1px solid rgba(var(--v-theme-on-surface), var(--v-high-emphasis-opacity));
}
</style>