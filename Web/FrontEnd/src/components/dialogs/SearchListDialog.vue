<template>
    <v-dialog persistent :modelValue="isOpen" @update:modelValue="(value) => this.$emit('update:isOpen', value)"
        max-width="1000px">
        <v-card flat>
            <v-card-text>
                <v-data-table-server 
                    :headers="headers" :items="list" :loading="loading" :page="page"
                    select-strategy="all" :show-select="multiSelect" 
                    v-model="selecteds" return-object
                    @update:options="onSearch" :items-length="totalEntity" 
                    @click:row="onSelectedRow"
                    v-model:items-per-page="itemsPerPage" class="elevation-1" dense>

                    <template v-slot:top>
                        <div style="display: flex; justify-content: center; align-items: start;">
                            <span class="text-h6 ma-1 hover-row"> {{ entityName }}</span>

                            <v-text-field class="ma-1" height="2" density="compact" variant="solo-filled"
                                v-model="search" :label="$t('label.search')" :placeholder="entityName"
                                append-inner-icon="mdi-magnify" clearable @keydown.enter="onSearch"></v-text-field>

                            <v-btn class="ml-1 mt-1" color="error" style="margin-left: auto;"
                                @click="() => this.$emit('update:isOpen', false)" icon="mdi-close" size="small"></v-btn>
                        </div>
                        <slot name="top">
                            
                        </slot>
                    </template>
                    
                    <!--
                    <template v-slot:item.actions="{ item }">
                        <v-btn color="primary" small @click="() => onSelected ? onSelected(item) : ''">
                            <v-icon icon="mdi-backburger" />
                        </v-btn>
                    </template>
                    -->
                </v-data-table-server>
            </v-card-text>
            <v-card-actions v-if="multiSelect">
                <v-btn variant="elevated" prepend-icon="mdi-check" color="success" @click="onConfirmSelection">{{ $t('label.ok') }} </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>

/*
headers: [
    { title: this.$t('label.name'), key: "CompanyName", sortable: false },
    { title: this.$t('label.aliasName'), key: "AliasName", sortable: false },
    { title: this.$t('label.taxIdAll'), key: "TaxId", sortable: false },
    { title: this.$t('label.email'), key: "Email", sortable: false },
    { title: "", key: "actions", sortable: false },
],
*/

export default {
    name: "SearchListDialog",

    props: [
        "apiUrl", "filter",
        "entityName", "headers",
        "isOpen", "onSelected",
        "multiSelect"
    ],
    emits: ['update:isOpen'],
    components: {},

    data() {
        return {
            hoveredRow: null,

            search: "",
            itemsPerPage: 10,
            page: 1,
            pageTotal: 0,
            totalEntity: 0,
            loading: false,

            selecteds: [],

            list: []
        };
    },
    computed: {

    },
    methods: {
        onSearch(options) {
            let route = this.apiUrl;
            this.loading = true;
            this.page = options?.page ?? this.page;
            let take = this.itemsPerPage > 0 ? this.itemsPerPage: 2147483647;

            route += `?page=${this.page}`;
            if (this.filter) route += "&" + this.filter;
            route += `&take=${take}`;
            route += this.search ? `&search=${this.search}` : "";

            this.$axios.get(route).then(response => {
                this.loading = false;
                this.list = response.data.List;
                this.totalEntity = response.data.Count;
                this.pageTotal = Math.ceil(this.totalEntity / this.itemsPerPage);
            }).catch((error) => {
                this.loading = false;
            });
        },
        onSelectedRow(event, params) {
            if (this.multiSelect) this.selecteds.push(params.item);
            else if (this.onSelected) this.onSelected(params.item);
        },
        onConfirmSelection(){
            if (this.onSelected) this.onSelected(this.selecteds);
            this.selecteds = [];
        }
    },
    mounted() {
        if(this.isOpen){
            this.selecteds = [];
            this.onSearch();
        }
    }
};
</script>

<style>
.v-data-table__tr--clickable:hover {
    background-color: rgba(var(--v-theme-success)) !important; 
}
</style>