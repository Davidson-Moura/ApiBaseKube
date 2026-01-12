<template>
    <v-container fluid>
        <v-card class="pa-4">
            <v-card-title class="d-flex">
                <v-btn variant="text" size="small" @click="navBack">
                    <v-icon icon="mdi-arrow-left" />
                    <v-tooltip activator="parent" location="top">{{ $t('label.back') }}</v-tooltip>
                </v-btn>
                <div style="display: flex; align-items: center; margin-left: auto;">
                    <v-icon icon="mdi-account-multiple" />
                    <h2 class="ml-2">
                        {{ $t('label.user') }}
                    </h2>
                </div>
            </v-card-title>

            <v-form v-model="isValid" ref="form">
                <v-card class="pa-4">
                    <v-row>
                        <v-col cols="12" md="4">
                            <v-text-field :label="$t('label.name')" v-model="entity.Name" density="compact" required
                                hide-details="auto" :rules="[rules.required]"></v-text-field>
                        </v-col>
                        <v-col cols="12" md="8">
                            <v-text-field v-model="entity.Email" :label="$t('label.email')" required density="compact"
                                hide-details="auto" :rules="[rules.required, rules.email]"></v-text-field>
                        </v-col>
                        <v-col cols="12" md="4">
                            <v-text-field :label="$t('label.login')" v-model="entity.LoginName" density="compact"
                                required hide-details="auto" :rules="[rules.required]"></v-text-field>
                        </v-col>
                        <v-col cols="12" md="4" v-if="!entity.Id">
                            <v-text-field :append-inner-icon="viewPassword ? 'mdi-eye' : 'mdi-eye-off'"
                                hide-details="auto" @click:append-inner="viewPassword = !viewPassword"
                                v-model="entity.Password" :label="$t('label.password')"
                                :type="viewPassword ? '' : 'password'" density="compact" :rules="passwordRulesComputed"
                                prepend-inner-icon="mdi-lock-outline"></v-text-field>
                        </v-col>
                        <v-col cols="12" md="4" v-if="!entity.Id">
                            <v-text-field :append-inner-icon="viewPassword ? 'mdi-eye' : 'mdi-eye-off'"
                                hide-details="auto" @click:append-inner="viewPassword = !viewPassword"
                                v-model="entity.ConfirmPassword" :label="$t('label.confirmPassword')"
                                :type="viewPassword ? '' : 'password'" density="compact"
                                :rules="confirmPasswordRulesComputed"
                                prepend-inner-icon="mdi-lock-outline"></v-text-field>
                        </v-col>
                        <v-col cols="12" md="4">
                            <v-checkbox :label="$t('label.superUser')" v-model="entity.Super" :readonly="entity.System"
                                hide-details="auto" />
                        </v-col>
                        <v-col cols="12" md="4">
                            <v-checkbox :label="$t('label.locked')" v-model="entity.Locked" :readonly="entity.System"
                                hide-details="auto" />
                        </v-col>
                        <v-col cols="12" md="4" v-if="entity.System">
                            <v-checkbox :label="$t('label.systemUser')" v-model="entity.System" readonly
                                hide-details="auto" />
                        </v-col>
                    </v-row>
                </v-card>

                <v-card class="pa-4 mt-3">
                    <v-row>
                        <v-col cols="12" md="4">
                            <v-text-field compact readonly :rules="[rules.required]" hide-details="auto"
                                :label="$t('label.authorizationGroup')" density="compact"
                                v-model="entity.AuthorizationGroupName" append-icon="mdi-magnify"
                                @click:append="onSearchAuthorizationGroup" append-inner-icon="mdi-close"
                                @click:append-inner="onClearAuthorizationGroup" 
                                prepend-inner-icon="mdi-open-in-new"
                                @click:prependInner="$MyApp.openInNewTab($router, 'AuthorizationGroupEdit', { id: entity.AuthorizationGroupId })"
                                />
                            <SearchListDialog :apiUrl="$api.authorizationGroups"
                                :entityName="$t('label.authorizationGroups')" :headers="authorizationGroupHeaders"
                                v-model:isOpen="isOpenAuthorizationGroupSelect"
                                :onSelected="onSelectedAuthorizationGroup" />
                        </v-col>
                        <v-col cols="12" md="4">
                            <v-text-field compact readonly :rules="[]" hide-details="auto"
                                :label="$t('label.branches')" density="compact"
                                v-model="empty"
                                :clearable="false" 
                                >
                                <v-chip v-if="entity.Branchs && entity.Branchs.length <= 0" variant="outlined" class="mt-1">
                                    {{ $t('label.all') }}
                                </v-chip>

                                <v-chip v-for="(v, i) in entity.Branchs" :key="v.BranchId" 
                                    >{{ v.BranchName }} </v-chip>
                            </v-text-field>
                        </v-col>
                        <v-col cols="12" md="4">
                            <v-select hide-details="auto" :label="$t('label.mainBranch')" :items="entity.Branchs"
                                item-title="BranchName" item-value="BranchId" v-model="entity.CurrentBranch">
                            </v-select>
                        </v-col>
                        <v-col cols="12" md="4">
                            <v-text-field compact readonly :rules="[rules.required]" 
                                hide-details="auto"
                                :label="$t('label.collaborator')" density="compact"
                                v-model="entity.CollaboratorName" append-icon="mdi-magnify"
                                @click:append="onSearchCollaborator" append-inner-icon="mdi-close"
                                @click:append-inner="onClearCollaborator" 
                                prepend-inner-icon="mdi-open-in-new"
                                @click:prependInner="$MyApp.openInNewTab($router, 'CollaboratorEdit', { id: entity.CollaboratorId })"
                                />
                            <SearchListDialog :apiUrl="$api.collaborators"
                                :entityName="$t('label.collaborators')" :headers="collaboratorHeaders"
                                v-model:isOpen="isOpenCollaboratorSelect"
                                :onSelected="onSelectedCollaborator" />
                        </v-col>
                    </v-row>
                </v-card>

            </v-form>
            <v-card-actions class="d-flex mt-auto">
                <v-btn style="margin-right: auto;" color="info" v-if="entity.Id && $MyApp.hasPermission($permissions.U_UPWD)" @click="onChangePassword"
                    v-t="'label.changePassword'" variant="elevated"></v-btn>

                <v-btn style="margin-left: auto;" color="success" @click="onSave" v-t="'label.save'"
                    v-if="entity ? $MyApp.hasPermission($permissions.U_U) : $MyApp.hasPermission($permissions.U_C)"
                    variant="elevated"></v-btn>
            </v-card-actions>
        </v-card>

        <v-dialog v-model="dialogChangePassword" max-width="400px">
            <v-card>
                <v-card-title class="headline">{{ $t('label.changePassword') }}</v-card-title>
                <v-card-text>
                    <v-form ref="formChangePassword" v-model="isValidChangePassword">
                        <v-text-field v-model="changeModel.Password" :label="$t('label.newPassword')"
                            :type="viewPassword ? '' : 'password'"
                            :append-inner-icon="viewPassword ? 'mdi-eye' : 'mdi-eye-off'"
                            @click:append-inner="viewPassword = !viewPassword"
                            :rules="[rules.required, rules.passwordDialogRules]"></v-text-field>
                        <v-text-field v-model="changeModel.ConfirmPassword" :label="$t('label.confirmPassword')"
                            :type="viewPassword ? '' : 'password'"
                            :append-inner-icon="viewPassword ? 'mdi-eye' : 'mdi-eye-off'"
                            @click:append-inner="viewPassword = !viewPassword"
                            :rules="[rules.required, rules.confirmPasswordDialogRules]"></v-text-field>
                    </v-form>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="error" text @click="dialogChangePassword = false" variant="elevated">{{ $t('label.cancel') }}</v-btn>
                    <v-btn color="success" @click="changePassword" variant="elevated">{{ $t('label.save') }}</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-container>
</template>

<script>
import SearchListDialog from "@/components/dialogs/SearchListDialog.vue";

export default {
    components: {
        SearchListDialog
    },
    data() {
        return {
            isValid: false,
            empty: " ",

            entity: {},

            branchs: [],

            dialogChangePassword: false,
            changeModel: {
                Email: "",
                Password: "",
                ConfirmPassword: ""
            },
            isValidChangePassword: false,

            viewPassword: false,
            rules: {
                required: (value) => !!value || this.$t('message.TheFieldIsRequired'),
                email: (value) => /.+@.+\..+/.test(value) || this.$t('message.EmailInvalid'),
                passwordRules: (value) => !(!this.entity.Id) || (value.length >= 6 || this.$t('message.PasswordDoesNotHaveTheMinimumNumberOfCharacters', { num: 6 })),
                confirmPasswordRules: (value) => !(!this.entity.Id) || (this.entity.Password == value || this.$t('message.PasswordAndPasswordConfirmationDoNotMatch')),

                passwordDialogRules: (value) => (value.length >= 6 || this.$t('message.PasswordDoesNotHaveTheMinimumNumberOfCharacters', { num: 6 })),
                confirmPasswordDialogRules: (value) => (this.changeModel.Password == value || this.$t('message.PasswordAndPasswordConfirmationDoNotMatch'))
            },

            isOpenAuthorizationGroupSelect: false,
            authorizationGroupHeaders: [
                { title: this.$t('label.name'), key: "Name", sortable: false },
                { title: this.$t('label.description'), key: "Description", sortable: false },
            ],

            isOpenCollaboratorSelect: false,
            collaboratorHeaders: [
                { title: this.$t('label.name'), key: "Name", sortable: false },
                { title: this.$t('label.surname'), key: "Surname", sortable: false },
            ],
            

        };
    },
    computed: {
        passwordRulesComputed() {
            return this.entity.Id ? [] : [this.rules.required, this.rules.passwordRules];
        },
        confirmPasswordRulesComputed() {
            return this.entity.Id ? [] : [this.rules.required, this.rules.confirmPasswordRules];
        },
    },
    created() {
        this.loadEntity(this.$route.params.id);
        //this.loadBranchs();
    },
    methods: {
        loadEntity(id) {
            if (!id) return;

            this.$MyApp.setLoading(true);
            let url = this.$api.userByKey.replace("{0}", id);
            this.$axios.get(url).then((response) => {
                this.$MyApp.setLoading(false);
                this.entity = response.data;
            }).catch((error) => {
                this.$MyApp.setLoading(false);
            });
        },
        onSave() {
            this.$refs.form.validate().then(() => {
                if (!this.isValid) {
                    this.$MyApp.error(this.$t('message.ThereAreInvalidFields'));
                    return;
                }
                this.$MyApp.setLoading(true);
                let url = this.$api.users;

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
            this.$router.push({ name: "Users" });
        },
        onChangePassword() {
            this.dialogChangePassword = true;
        },
        changePassword() {
            this.$refs.formChangePassword.validate().then(() => {
                if (!this.isValidChangePassword) {
                    this.$MyApp.error(this.$t('message.ThereAreInvalidFields'));
                    return;
                }
                this.$MyApp.setLoading(true);
                let url = this.$api.userChangePassword;

                this.changeModel.Id = this.entity.Id;

                this.$axios.put(url, this.changeModel).then((response) => {
                    this.$MyApp.setLoading(false);
                    this.$MyApp.success(this.$t('message.ChangePasswordSuccessfully'));
                    this.dialogChangePassword = false;
                }).catch((error) => {
                    this.$MyApp.setLoading(false);
                });
            });
        },

        onSearchAuthorizationGroup() {
            this.isOpenAuthorizationGroupSelect = true;
        },
        onSelectedAuthorizationGroup(obj) {
            this.entity.AuthorizationGroupId = obj.Id;
            this.entity.AuthorizationGroupName = obj.Name;

            this.isOpenAuthorizationGroupSelect = false;
        },
        onClearAuthorizationGroup() {
            this.entity.AuthorizationGroupId =
                this.entity.AuthorizationGroupName = undefined;
        },
        /*
        loadBranchs() {
            this.$MyApp.setLoading(true);
            let url = this.$api.branchs;
            url += `?take=${2147483647}`;
            this.$axios.get(url).then((response) => {
                this.$MyApp.setLoading(false);
                this.branchs = response.data.List.map(x => {
                    return {
                        BranchId: x.Id,
                        BranchName: x.Name
                    }
                });
            }).catch((error) => {
                this.$MyApp.setLoading(false);
            });
        },
        */
        onChangeBranch() {
            if (!this.entity.Branchs?.some(x => x.BranchId == this.entity.CurrentBranch)) this.entity.CurrentBranch = null;
            if (!this.entity.CurrentBranch && this.entity.Branchs && this.entity.Branchs?.length > 0) this.entity.CurrentBranch = this.entity.Branchs[0].BranchId;
        },

        onSearchCollaborator() {
            this.isOpenCollaboratorSelect = true;
        },
        onSelectedCollaborator(obj) {
            this.entity.CollaboratorId = obj.Id;
            this.entity.CollaboratorName = obj.Name;

            this.branchs = 
            this.entity.Branchs = obj.Branchs;

            this.onChangeBranch();

            this.isOpenCollaboratorSelect = false;
        },
        onClearCollaborator() {
            this.entity.CollaboratorId =
            this.entity.CollaboratorName = undefined;

            this.entity.Branchs = [];
            this.entity.CurrentBranch = undefined;
        },
    },
};
</script>