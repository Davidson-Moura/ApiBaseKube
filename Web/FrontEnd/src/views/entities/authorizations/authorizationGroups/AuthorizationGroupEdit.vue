<template>
  <v-container fluid>
    <v-card class="pa-4">
      <v-card-title class="d-flex">
        <v-btn variant="text" size="small" @click="navBack">
          <v-icon icon="mdi-arrow-left" />
          <v-tooltip activator="parent" location="top">{{ $t('label.back') }}</v-tooltip>
        </v-btn>
        <div style="display: flex; align-items: center; margin-left: auto;">
          <v-icon icon="mdi-shield-account" />
          <h2 class="ml-2">
            {{ $t('label.authorizationGroup') }}
          </h2>

          <v-btn @click=" isOpenLogHistoryDialog = true" icon="mdi-clipboard-text-clock" color="info" rounded size="small" class="ml-2" />
        </div>
      </v-card-title>

      <v-form v-model="isValid" ref="form">
        <v-row>
          <v-col cols="12" md="4">
            <v-text-field :label="$t('label.name')" v-model="entity.Name" density="compact" required
              :rules="[rules.required]"></v-text-field>
          </v-col>
          <v-col cols="12" md="8">
            <v-text-field v-model="entity.Description" :label="$t('label.description')" required density="compact"
              :rules="[rules.required]"></v-text-field>
          </v-col>
          <v-col cols="12" md="4" v-if="entity.System">
            <v-checkbox :label="$t('label.systemUser')" v-model="entity.System" readonly />
          </v-col>
        </v-row>


        <v-card>
          <v-card-title>
            <v-row>
              <v-col cols="9" class="d-flex align-center">
                {{ $t('label.authorizations') }}

                <v-btn icon="" variant="text" class="ml-2" size="small" @click="() => openAll = true">
                  <v-icon icon="mdi-arrow-expand-vertical" />
                  <v-tooltip activator="parent" location="top">{{ $t('label.expand') }}</v-tooltip>
                </v-btn>
                <v-btn icon="" variant="text" class="ml-2" size="small" @click="() => openAll = false">
                  <v-icon icon="mdi-arrow-collapse-vertical" />
                  <v-tooltip activator="parent" location="top">{{ $t('label.collapse') }}</v-tooltip>
                </v-btn>

              </v-col>

              <v-col cols="12" md="3" class="d-flex align-center">
                <v-btn icon="" color="success" size="small" @click="onTotalAuthorization">
                  <v-icon icon="mdi-check-all" />
                  <v-tooltip activator="parent" location="top">{{ $t('label.totalAuthorization') }}</v-tooltip>
                </v-btn>

                <v-btn icon="" color="error" class="ml-2" size="small" @click="onWithoutAuthorization">
                  <v-icon icon="mdi-close" />
                  <v-tooltip activator="parent" location="top">{{ $t('label.withoutAuthorization') }}</v-tooltip>
                </v-btn>

                <v-btn icon="" color="info" class="ml-2" size="small" @click="onViewOnly">
                  <v-icon icon="mdi-eye" />
                  <v-tooltip activator="parent" location="top">{{ $t('label.viewOnly') }}</v-tooltip>
                </v-btn>
              </v-col>
            </v-row>
          </v-card-title>
          <v-card-text>
            <v-treeview :open-all="openAll" :items="permissionsTreeFiltered" item-key="Code" item-value="Code"
              item-children="Roles" open-on-click>

              <template v-slot:item="{ item }">

                <v-card :color="item.Selected ? 'success' : 'error'" class="d-flex align-end mb-2 pa-2" dark
                  @click="() => toggleAuthorization(item)">
                  <v-scroll-y-transition>
                    <v-row>
                      <v-col cols="1" class="d-flex align-center">
                        <v-icon size="large" :icon="item.Selected ? 'mdi-check-bold' : 'mdi-close'" />
                      </v-col>
                      <v-col cols="11" class="text-h6 text-start flex-grow-1">
                        {{ item.Name }}
                      </v-col>

                    </v-row>
                  </v-scroll-y-transition>
                </v-card>
              </template>
              <template v-slot:title="{ item }">
                <div class="d-flex align-center">
                  <h4 class="mr-2">
                    {{ item.Name }}
                  </h4>
                  <v-icon :icon="item.Selecteds == item.Roles.length ? 'mdi-check-all' :
                    (item.Selecteds ? 'mdi-check' : '')" />
                </div>
              </template>
            </v-treeview>


          </v-card-text>
        </v-card>

        <v-toolbar class="mt-2">
          <v-toolbar-title>{{ $t('label.itemGroups') }}</v-toolbar-title>

          <v-btn class="mr-2" icon="mdi-plus" color="success" variant="elevated" rounded size="small"
            @click="() => isOpenItemGroupSelect = true"></v-btn>
          <SearchListDialog :apiUrl="$api.itemGroups" :entityName="$t('label.itemGroups')" :headers="itemGroupHeaders"
            v-model:isOpen="isOpenItemGroupSelect" :onSelected="onSelectedItemGroup" />
        </v-toolbar>
        <v-list dense>
          <v-list-item v-for="(item, index) in entity.ItemGroups" :key="index">
            <v-list-item-title>{{ item.texto }}</v-list-item-title>
            <v-row class="mb-1">
              <v-col cols="3" md="4">
                <v-text-field :label="$t('label.code')" v-model="item.Code" density="compact" required
                  prepend-inner-icon="mdi-open-in-new"
                  @click:prependInner="$MyApp.openInNewTab($router, 'ItemGroupEdit', { id: item.Id })"
                  :clearable="false" hide-details="auto" readonly></v-text-field>
              </v-col>
              <v-col cols="8" md="7">
                <v-text-field :label="$t('label.name')" v-model="item.Name" density="compact" required
                  :clearable="false" hide-details="auto" readonly></v-text-field>
              </v-col>
              <v-col cols="1" md="1">
                <v-btn icon="mdi-delete" @click="removeItem(index)" color="error" variant="elevated" rounded
                  size="small" />
              </v-col>

            </v-row>
            <v-divider></v-divider>
          </v-list-item>
        </v-list>

      </v-form>
      <v-card-actions class="d-flex mt-auto">

        <v-btn style="margin-left: auto;" color="success" @click="onSave" v-t="'label.save'" 
          v-if="(entity.Id ? $MyApp.hasPermission($permissions.AG_U) : $MyApp.hasPermission($permissions.AG_C))"
          variant="elevated"></v-btn>
      </v-card-actions>
    </v-card>

    <LogHistoryDialog 
      v-model:isOpen="isOpenLogHistoryDialog" 
      :objectTypeEnum="fmt.ObjectTypeEnum.AuthorizationGroups"
      :objectId="entity.Id"
      />

  </v-container>
</template>

<script>
import SearchListDialog from "@/components/dialogs/SearchListDialog.vue";
import LogHistoryDialog from "@/components/dialogs/entities/logs/LogHistoryDialog.vue";
import formatter from '@/helps/formatter';

export default {
  components: {
    SearchListDialog,
    LogHistoryDialog
  },
  data() {
    return {
      isValid: false,

      entity: {},
      permissionsTreeFiltered: [],
      permissionsTree: [],

      openAll: false,

      isOpenItemGroupSelect: false,
      itemGroupHeaders: [
        { title: this.$t('label.code'), key: "Code", sortable: false },
        { title: this.$t('label.name'), key: "Name", sortable: false },
      ],

      rules: {
        required: (value) => !!value || this.$t('message.TheFieldIsRequired'),
      },

      isOpenLogHistoryDialog: false,
      fmt: formatter
    };
  },
  created() {
    Promise.all([
      this.loadEntity(this.$route.params.id),
      this.loadTree()
    ]).then(pms => {
      this.fillTreeAuthorizations();
      this.searchAuthorizations();
    })
  },
  methods: {
    loadEntity(id) {
      if (!id) return;

      this.$MyApp.setLoading(true);
      let url = this.$api.authorizationGroupByKey.replace("{0}", id);

      return this.$axios.get(url).then((response) => {
        this.$MyApp.setLoading(false);
        this.entity = response.data;
      }).catch((error) => {
        this.$MyApp.setLoading(false);
      });
    },

    loadTree() {
      let url = this.$api.authorizationGroupGetTree;

      return this.$axios.get(url).then((response) => {
        this.permissionsTree = response.data;
      }).catch((error) => { });
    },
    fillTreeAuthorizations() {
      this.permissionsTree.forEach(p => {
        let count = 0;
        p.Roles.forEach(role => {
          role.Selected = this.entity.Roles?.some(x => x == role.Code);
          if (role.Selected) count++;
        });
      });
      this.fillAdditionalInfoTreeAuthorizations();
    },
    fillAdditionalInfoTreeAuthorizations() {
      this.permissionsTree.forEach(p => {
        let count = 0;
        p.Roles.forEach(role => {
          if (role.Selected) count++;
        });

        p.Selecteds = count;
      });
    },
    searchAuthorizations() {
      this.permissionsTreeFiltered = this.permissionsTree;
    },
    toggleAuthorization(item) {
      item.Selected = !item.Selected;
      this.fillAdditionalInfoTreeAuthorizations();
    },
    fillBySave() {
      this.entity.Roles = [];
      this.permissionsTree.forEach(p => {
        p.Roles.forEach(role => {
          if (role.Selected) this.entity.Roles.push(role.Code);
        });
      });
    },
    onSave() {
      this.fillBySave();
      this.$refs.form.validate().then(() => {
        if (!this.isValid) {
          this.$MyApp.error(this.$t('message.ThereAreInvalidFields'));
          return;
        }
        this.$MyApp.setLoading(true);
        let url = this.$api.authorizationGroups;

        let method = this.entity.Id ? "put" : "post"

        this.$axios[method](url, this.entity).then((response) => {
          this.$MyApp.setLoading(false);
          this.$MyApp.success(this.$t('message.SaveSuccessfully'));
          this.navBack();
        }).catch((error) => {
          this.$MyApp.setLoading(false);
        });
      });
    },
    navBack() {
      this.$router.push({ name: "AuthorizationGroups" });
    },
    onTotalAuthorization() {
      this.permissionsTree.forEach(p => {
        p.Roles.forEach(role => {
          role.Selected = true;
        });
      });
      this.fillAdditionalInfoTreeAuthorizations();
    },
    onWithoutAuthorization() {
      this.permissionsTree.forEach(p => {
        p.Roles.forEach(role => {
          role.Selected = false;
        });
      });
      this.fillAdditionalInfoTreeAuthorizations();
    },
    onViewOnly() {
      this.permissionsTree.forEach(p => {
        p.Roles.forEach(role => {
          let split = role.Code.split("_");
          if (split[1] == "V") role.Selected = true;
          else role.Selected = false;
        });
      });
      this.fillAdditionalInfoTreeAuthorizations();
    },


    onSelectedItemGroup(obj) {
      if (!this.entity.ItemGroups) this.entity.ItemGroups = [];

      if (!this.entity.ItemGroups.some(x => x.ItemFeatureId == obj.Id))
        this.entity.ItemGroups.push({
          Id: obj.Id,
          Name: obj.Name,
          Code: obj.Code,

        });

      this.isOpenItemGroupSelect = false;
    },
    removeItem(index) {
      if (!this.entity.ItemGroups) this.entity.ItemGroups = [];

      this.entity.ItemGroups.splice(index, 1);
    },
  },
};
</script>