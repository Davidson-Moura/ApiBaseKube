<template>
  <v-container fluid class="mt-2">

    <v-row>
      <v-col cols="12">
        <v-data-table-server v-model="selected" show-select fixed-header :data-table-select="true" item-value="Id"
          v-model:items-per-page="itemsPerPage" :headers="headers" :items="entities" :items-length="totalEntities"
          :loading="loading" @update:options="loadEntities">

          <template v-slot:top>
            <v-sheet rounded color="primary" class="pa-3 mb-1">
              <v-row style="align-items:center;">
                <v-col cols="12" md="6" class="d-flex" style="align-items:center;">
                  <v-btn variant="text" size="small" @click="navBack">
                    <v-icon icon="mdi-arrow-left" />
                    <v-tooltip activator="parent" location="top">{{ $t('label.back') }}</v-tooltip>
                  </v-btn>
                  <div style="display: flex; align-items: center;">
                    <v-icon icon="mdi-shield-account" />
                    <h2 class="ml-2">
                      {{ $t('label.authorizationGroups') }}
                    </h2>
                  </div>
                </v-col>
                <v-col cols="12" md="6" class="d-flex">
                  <v-text-field v-model="search" :label="$t('label.search')" hide-details="auto" density="compact" dense
                    prepend-inner-icon="mdi-magnify" clearable
                    @keyup.enter="() => loadEntities({ page: 1, itemsPerPage: myApp.pageSize })" />

                  <v-btn color="success" icon="" size="small" class="ml-2" @click="onAddEntity" v-if="$MyApp.hasPermission($permissions.AG_C)">
                    <v-icon icon="mdi-plus" />
                    <v-tooltip activator="parent" location="top">{{ $t('label.add') }}</v-tooltip>
                  </v-btn>

                  <v-btn color="error" icon="" size="small" class="ml-2" @click="onRemove"
                    v-if="$MyApp.hasPermission($permissions.AG_D)">
                    <v-icon icon="mdi-delete-forever" />
                    <v-tooltip activator="parent" location="top">{{ $t('label.delete') }}</v-tooltip>
                  </v-btn>
                </v-col>

              </v-row>
            </v-sheet>
          </template>

          <template v-slot:item.CreateDate="{ item }">
            {{ fmt.formatDate(item.CreateDate) }}
          </template>

          <template v-slot:item.UpdateDate="{ item }">
            {{ fmt.formatDate(item.UpdateDate) }}
          </template>

          <template v-slot:item.actions="{ item }">
            <v-btn color="info" size="small" @click="() => onEditEntity(item)">
              <v-icon icon="mdi-arrow-right-bold" />
              <v-tooltip activator="parent" location="top">{{ $t('label.view') }}</v-tooltip>
            </v-btn>
          </template>
        </v-data-table-server>
      </v-col>
    </v-row>
    <ConfirmDialog v-model:isOpen="isOpenRemoveD" :msg="$t('message.ConfirmRemoveMsg')" :onOption2="remove"
      :onOption1="() => selected = []" />
  </v-container>
</template>

<script setup>
import { ref, onMounted, getCurrentInstance } from 'vue';
import { useRouter } from 'vue-router';
import i18n from '@/plugins/i18nBase';
import ConfirmDialog from "@/components/dialogs/ConfirmDialog.vue";
import formatter from '@/helps/formatter';

const fmt = ref(formatter);
const router = useRouter();

const instance = getCurrentInstance();
const global = instance?.appContext.config.globalProperties;
const myApp = global.$MyApp;
const api = global.$api;
const axios = global.$axios;

const selected = ref([]);

const search = ref('');
const entities = ref([]);
const totalEntities = ref(0);
const loading = ref(false);
const itemsPerPage = ref(myApp.getPageSize());

const isOpenRemoveD = ref(false);

const headers = [
  { title: i18n.global.t('label.name'), key: 'Name', sortable: false },
  { title: i18n.global.t('label.createDate'), key: 'CreateDate', align: "end", sortable: false },
  { title: i18n.global.t('label.updateDate'), key: 'UpdateDate', align: "end", sortable: false },
  { title: "", key: "actions", align: "end", sortable: false },
];

const loadEntities = async ({ page, itemsPerPage, sortBy }) => {
  loading.value = true;

  let route = api.authorizationGroups;

  let take = itemsPerPage <= 0 ? 2147483647: itemsPerPage;

  route += `?page=${page}`;
  route += `&take=${take}`;
  route += search.value ? `&search=${search.value}` : "";

  axios.get(route).then(response => {
    entities.value = response.data.List;
    totalEntities.value = response.data.Count;
  }).catch((error) => { }).finally(() => loading.value = false);
};

const navBack = () => {
  router.push({ name: "Home" });
};

const onAddEntity = () => {
  router.push({ name: "AuthorizationGroupEdit" });
};

const onEditEntity = (obj) => {
  router.push({ name: "AuthorizationGroupEdit", params: { id: obj.Id } });
};

const onRemove = () => {
  if (selected.value.length <= 0) {
    myApp.success(i18n.global.t('message.RemoveSuccessfully'));
    return;
  }
  isOpenRemoveD.value = true;
};
const remove = () => {
  let route = api.authorizationGroups;
  myApp.setLoading(true);
  axios.delete(route, { data: selected.value }).then(response => {
    myApp.success(i18n.global.t('message.RemoveSuccessfully'));
    loadEntities({ page: 1, itemsPerPage: myApp.pageSize });
    selected.value = [];
  }).catch((error) => { }).finally(() => myApp.setLoading(false));
};




onMounted(() => {
  //loadEntities({ page: 1, itemsPerPage: myApp.pageSize }) // já executando no binding update:options
});
</script>

<style scoped></style>