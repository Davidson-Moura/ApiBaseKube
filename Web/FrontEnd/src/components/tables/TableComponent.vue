<template>
    <v-card color="primary">
        <v-overlay :model-value="inLoading" contained class="align-center justify-center">
            <v-progress-circular color="secondary" indeterminate size="64"></v-progress-circular>
        </v-overlay>
        <div class="d-flex pa-2">
            <v-text-field
                v-if="funcSearch"
                color="error"
                clearable
                density="compact" variant="outlined" append-inner-icon="mdi-magnify"
                :label="$t('label.search')"
                class="mr-3"
                single-line 
                hide-details
                style="width: 14rem;"
                v-model="table.Search"
                @click:clear="funcSearch"
                @keydown.enter="funcSearch ? funcSearch(table.Search) : null;">
            </v-text-field>

            <v-spacer></v-spacer>

            <div>
                <v-btn v-show="funcAdd" variant="elevated" color="success" density="comfortable" icon="mdi-plus"
                    class="mx-1" @click="funcAdd ? funcAdd() : null" />
                <v-btn v-show="funcRemove" variant="elevated" color="error" density="comfortable" icon="mdi-delete"
                    class="mx-1" @click="funcRemove ? funcRemove(selectedsIndex()) : null" />
            </div>
        </div>
        <v-table fixed-header>
            <thead>
                <tr>
                    <th class="th-td" width="40" style="padding: 0;" :class="!disableSelect ? '' : 'd-none'" >

                        <v-checkbox 
                            v-if="!isOneSelect" class="d-flex" style="align-items: end;"
                            @click="selectAll">
                        </v-checkbox>
                    </th>
                    <th class="text-left" v-for="col in columns">
                        {{ col }}
                    </th>
                    <th width="40" v-show="funcEdit"></th>
                </tr>
            </thead>
            <tbody >
                <tr v-if="table" v-for="(obj, index) in table.List" :key="index" @click="() => internalLineClick(obj)">
                    <td width="40" style="padding: 0;" :class="!disableSelect ? '' : 'd-none'">
                        <v-checkbox class="d-flex" style="align-items: end;" v-model="obj.Selected"></v-checkbox>
                    </td>
                    <td v-for="(prop, i) in properties">{{ formatters && formatters[i] ? 
                        formatters[i]( (prop ? obj[prop] : prop) ) : 
                        (prop ? obj[prop] : obj) }}</td>
                    <td v-show="funcEdit">
                        <v-btn variant="elevated" color="info" density="comfortable" icon="mdi-pencil" class="mx-1"
                            @click="() => funcEdit(index, obj)" />
                    </td>
                </tr>
                <tr v-if="list" v-for="(obj, index) in list" :key="index" @click="() => internalLineClick(obj)">
                    <td width="40" style="padding: 0;" :class="!disableSelect ? '' : 'd-none'">
                        <v-checkbox class="d-flex" style="align-items: end;" v-model="obj.Selected"></v-checkbox>
                    </td>
                    <td v-for="prop in properties">{{ obj[prop] }}</td>
                    <td v-show="funcEdit">
                        <v-btn variant="elevated" color="info" density="comfortable" icon="mdi-pencil" class="mx-1"
                            @click="() => funcEdit(index, obj)" />
                    </td>
                </tr>
            </tbody>
        </v-table>
        <v-pagination v-if="table" v-model="table.Page" :length="table.Page ? Math.ceil(table.Total / table.Take) : 1"
            rounded="0" @update:modelValue="funcSearch ? funcSearch(table.Search) : null;"></v-pagination>
    </v-card>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
    name: 'TableComponent',
    props: [
        "inLoading",
        "columns",
        "properties",
        "formatters",

        "table",
        "list",

        "funcAdd",
        "funcEdit",
        "funcRemove",
        "funcSearch",
        "funcSelect",

        "disableSelect",
        "isOneSelect",
        "lineClick"
    ],

    emits: ['change', 'next', 'prev', 'onUpdate:table'],
    data() {
        return {
            selectedAll: false
        }
    },
    methods: {
        formatList(list) {
            list.forEach(element => {
                element.Selected = false;
            });
            return list;
        },
        selectAll() {
            this.selectedAll = !this.selectedAll;
            this.table.List.forEach(element => {
                element.Selected = this.selectedAll;
            });
        },
        selectedsIndex() {
            return this.table.List.filter(x => x.Selected == true);
        },
        internalLineClick(obj) {
            let isSelected = obj.Selected;
            if (this.table) {
                this.table.List.forEach(element => {
                    element.Selected = false;
                });
            } else if (this.List) {
                this.list.forEach(element => {
                    element.Selected = false;
                });
            }
            obj.Selected = !isSelected;
            if (this.lineClick) this.lineClick(obj.Selected ? obj : null);
        }
    }
});
</script>
<style scoped>
th, td{
    background-color: rgba(var(--v-theme-primary)) !important;
}
button {
    box-shadow: 0px 3px 1px -2px var(--v-shadow-key-umbra-opacity, rgba(0, 0, 0, 0.2)), 0px 2px 2px 0px var(--v-shadow-key-penumbra-opacity, rgba(0, 0, 0, 0.14)), 0px 1px 5px 0px var(--v-shadow-key-penumbra-opacity, rgba(0, 0, 0, 0.12));
}

tr:hover {
    cursor: pointer;
    box-shadow: 0px 0px 0px 2px rgb(var(--v-theme-primary)) inset;
}
</style>