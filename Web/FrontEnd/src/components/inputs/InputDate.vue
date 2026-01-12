<template>
    <v-text-field 
        :modelValue="dateFormatted"
        @update:modelValue="(value) => $emit('update:date', value)"
        :label="label" hide-details="auto" readonly
        :clearable="!disable" 
        @click:clear="$emit('update:date', null)"
        @click="openDatePiker">
        <v-dialog :model-value="open" @update:model-value="setCurrentTime(date)"  max-width="400px" persistent>
            <v-card>
                <v-tabs v-model="tab" fixed-tabs>
                    <v-tab key="date">{{$t('label.date')}}</v-tab>
                    <v-tab key="hour">{{$t('label.hour')}}</v-tab>
                </v-tabs>

                <v-tabs-window v-model="tab">
                    <v-tabs-window-item value="date">
                        <v-date-picker 
                            :model-value="date" 
                            @update:model-value="(v) => { setCurrentTime(date); $emit('update:date', v); }" />
                    </v-tabs-window-item>

                    <v-tabs-window-item value="hour">
                        <v-time-picker 
                            v-model="currentTime" 
                            scrollable format="24h"
                            @update:model-value="updateDate" />
                    </v-tabs-window-item>

                </v-tabs-window>

                <v-card-actions>
                    <v-spacer />
                    <v-btn color="success" variant="elevated" text @click="close">{{$t('label.ok')}}</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-text-field>
</template>
<script>
import { defineComponent } from 'vue';
import formatter from '@/helps/formatter';

export default defineComponent({
    name: 'InputDate',
    props: [
        'disable',
        "date",
        "label"
    ],

    emits: ['update:date'],
    data() {
        return {
            open: false,
            tab: 'date',
            currentTime: null,
            dateFormatted: null,

            fmt: formatter
        }
    },
    computed:{
        
    },
    methods: {
        setFormatedDate(){
            this.dateFormatted = this.fmt.formatFullDate(this.date);
        },
        openDatePiker(){
            if (this.date && typeof this.date == 'string') 
                this.$emit('update:date', new Date(this.date));
            if (!this.disable) {
                this.open = true;
                this.fillDefault();
            }
            
            this.setCurrentTime(this.date);
            this.setFormatedDate();
        },
        fillDefault(){
            if (!this.date) this.$emit('update:date', new Date());
        },
        setCurrentTime(date) {
            if (!this.date) return "";
            this.currentTime = `${(this.date.getHours() + "").padStart(2, "0")}:${(this.date.getMinutes() + "").padStart(2, "0")}`;
        },
        updateDate(){
            this.$emit('update:date', new Date(this.getDate(this.currentTime, this.date)));
        },
        getDate(v, date) {
            var parts = v.split(":");
            return date.setHours(parseInt(parts[0]), parseInt(parts[1]));
        },
        close(){
            this.open = false;
        }
    },
    mounted(){
        document.addEventListener(
            "touchmove",
            () => {},
            { passive: false }
        );
        this.setFormatedDate();
    },
    updated(){
        this.setFormatedDate();
    }
});
</script>
<style>
</style>