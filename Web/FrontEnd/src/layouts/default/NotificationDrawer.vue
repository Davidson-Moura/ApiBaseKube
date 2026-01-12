<template>
    <v-navigation-drawer app location="right" :width="400">
        <v-tabs v-model="notificationTab" bg-color="primary">
            <v-tab :value="tabs.notif">{{ $t('label.notifications') }}</v-tab>
            <v-tab :value="tabs.actions">{{ $t('label.actions') }}</v-tab>
        </v-tabs>
        <v-tabs-window v-model="notificationTab">
            <v-tabs-window-item :value="tabs.notif">
                <v-text-field v-model="search" :label="$t('label.search')" hide-details="auto" density="compact" dense
                    prepend-inner-icon="mdi-magnify" clearable @keyup.enter="loadNotifications" />
                <v-list>
                    <template v-if="notificationsInLoading">
                        <v-skeleton-loader
                            type="list-item-avatar"
                            class="ma-2"
                            elevation="5"

                            v-for="i in 5"
                            :key="i"
                            height="5rem"
                        />
                    </template>

                    <template v-else>
                        <v-list-item v-for="(notification, index) in notifications" :key="index" >
                            

                            <v-list-item-title>
                                <v-icon color="info">
                                    mdi-message-text-fast
                                </v-icon>
                                {{ notification.Title }}
                            </v-list-item-title>

                            <v-list-item-subtitle>
                                {{ fmt.formatFullDate(notification.CreateDate) }} - {{ notification.Message }}
                            </v-list-item-subtitle>

                            <template #append>
                                <v-chip v-if="!notification.IsRead" size="x-small" color="success" text-color="white" label>
                                    {{ $t('label.new') }}
                                </v-chip>
                            </template>
                        </v-list-item>
                    </template>
                </v-list>
            </v-tabs-window-item>

            <v-tabs-window-item :value="tabs.actions">
                <v-text-field v-model="search" :label="$t('label.search')" hide-details="auto" density="compact" dense
                    prepend-inner-icon="mdi-magnify" clearable @keyup.enter="loadActions">
                    <template #append>
                        <v-menu location="bottom">
                            <template v-slot:activator="{ props }">
                                <v-btn color="primary" v-bind="props">
                                    <v-icon>mdi-tune-vertical</v-icon>
                                </v-btn>
                            </template>

                            <v-list v-model:selected="actionStatus" select-strategy="leaf">
                                <v-list-item v-for="(item, index) in fmt.ActionRequestStatusEnumModel" 
                                    :key="index"
                                    :value="item.ID" :base-color="fmt.ActionRequestStatusColor(item.ID)"
                                    
                                    >
                                    <v-list-item-title>{{ item.Name }}</v-list-item-title>

                                    <template v-slot:prepend="{ isSelected, select }">
                                        <v-list-item-action start>
                                            <v-checkbox-btn :model-value="isSelected"
                                                @update:model-value="select"></v-checkbox-btn>
                                        </v-list-item-action>
                                    </template>
                                </v-list-item>
                            </v-list>
                        </v-menu>
                    </template>
                </v-text-field>
                <v-list>
                    <template v-if="actionInLoading">
                        <v-skeleton-loader
                            type="list-item-avatar"
                            class="ma-2"
                            elevation="5"

                            v-for="i in 5"
                            :key="i"
                            height="5rem"
                        />
                    </template>

                    <template v-else>
                    <v-list-item v-for="(action, index) in actions" :key="index" class="mb-2">

                        <v-list-item-title>
                            {{ action.Description }}
                        </v-list-item-title>

                        <v-list-item-subtitle>
                            <v-btn size="small" class="mr-1" variant="elevated" color="info" @click="() => { openActionRequest = true; actionRequest = action; }">
                                <v-icon>mdi-open-in-app</v-icon>
                            </v-btn>
                            <v-chip prepend-icon="mdi-lightning-bolt"
                                :color="fmt.ActionRequestStatusColor(action.Status)"
                                :text="fmt.ActionRequestStatusName(action.Status)" />

                            {{ fmt.formatFullDate(action.CreateDate) }}
                        </v-list-item-subtitle>

                        <template #append>
                            <v-btn size="small" variant="elevated" color="red" @click="(e) => cancelAction(action)"
                                v-if="action.Status == 1 || action.Status == 4">
                                {{ $t('label.cancel') }}
                            </v-btn>
                        </template>
                    </v-list-item>
                    </template>
                </v-list>
            </v-tabs-window-item>
        </v-tabs-window>

        <v-dialog v-model="openActionRequest" max-width="700">
            <v-card>
                <v-card-title class="text-h6 font-weight-bold">
                    {{ $t('label.actionDetails') }}
                </v-card-title>

                <v-card-text>
                    <v-table density="compact">
                        <tbody>

                            <tr>
                                <td><strong>{{ $t('label.description') }}</strong></td>
                                <td>{{ actionRequest.Description }}</td>
                            </tr>
                            <tr>
                                <td><strong>{{ $t('label.action') }}</strong></td>
                                <td>{{ fmt.ActionRequestName(actionRequest.Action) }}</td>
                            </tr>
                            <tr>
                                <td><strong>{{ $t('label.status') }}</strong></td>
                                <td><v-chip :color="fmt.ActionRequestStatusColor(actionRequest.Status)"
                                        :text="fmt.ActionRequestStatusName(actionRequest.Status)" /></td>
                            </tr>
                            <tr>
                                <td><strong>{{ $t('label.user') }}</strong></td>
                                <td>{{ actionRequest.UserName }}</td>
                            </tr>
                            <tr>
                                <td><strong>{{ $t('label.createDate') }}</strong></td>
                                <td>{{ fmt.formatFullDate(actionRequest.CreateDate) }}</td>

                            </tr>
                            <tr>
                                <td><strong>{{ $t('label.message') }}</strong></td>
                                <td>{{ actionRequest.Message }}</td>
                            </tr>
                        </tbody>
                    </v-table>
                </v-card-text>

                <v-card-actions>
                    <v-btn variant="elevated" color="error" @click="() => cancelAction(actionRequest)" v-if="actionRequest.Status == 1 || actionRequest.Status == 4">{{ $t('label.cancel') }}</v-btn>
                    <v-btn variant="elevated" color="info" @click="() => reprocessAction(actionRequest)" v-if="actionRequest.Status == 4">{{ $t('label.reprocess') }}</v-btn>
                    <v-spacer></v-spacer>
                    <v-btn variant="elevated" color="success" @click="openActionRequest = false">{{ $t('label.onClose') }}</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-navigation-drawer>
</template>

<script>
import formatter from '@/helps/formatter';

export default {
    components: {},
    props: {
    },
    data: () => ({
        search: '',

        tabs: {
            notif: "notifications",
            actions: "actions"
        },

        notificationTab: 'notifications',

        notificationsInLoading: false,
        notifications: [],

        actionStatus: [ 1, 2, 4 ],
        actionInLoading: false,
        actions: [],
        openActionRequest: false,
        actionRequest: null,


        fmt: formatter
    }),
    methods: {
        initLoad() {
            //if (!this.notifications || this.notifications.length <= 0) 
            this.loadNotifications();
            if (!this.actions || this.actions.length <= 0) this.loadActions();
        },
        loadNotifications() {
            this.notificationsInLoading = true;
            let url = this.$api.notifications;
            url += `?take=${10}`;
            if (this.search) url += `&search=${this.search}`;

            this.$axios.get(url).then((response) => {
                this.notificationsInLoading = false;
                this.notifications = response.data.List;
            }).catch((error) => {
                this.notificationsInLoading = false;
                this.$MyApp.error(error);
            });
        },
        loadActions() {
            this.actionInLoading = true;
            let url = this.$api.actionRequests;
            url += `?take=${10}`;
            if (this.search) url += `&search=${this.search}`;

            this.actionStatus.forEach(s => url += `&status=${s}` );

            this.$axios.get(url).then((response) => {
                this.actionInLoading = false;
                this.actions = response.data.List;
            }).catch((error) => {
                this.actionInLoading = false;
                this.$MyApp.error(error);
            });
        },
        cancelAction(action) {
            this.$MyApp.setLoading(true);
            let url = this.$api.actionRequestCancel.replace("{id}", action.Id);

            this.$axios.post(url).then((response) => {
                this.$MyApp.setLoading(false);
                this.loadActions();
                this.$MyApp.success(this.$t('message.CancelledSuccessfully'));
            }).catch((error) => {
                this.$MyApp.setLoading(false);
                this.$MyApp.error(error);
            });
        },
        reprocessAction(action) {
            this.$MyApp.setLoading(true);
            let url = this.$api.actionRequestReprocess.replace("{id}", action.Id);
            
            this.$axios.post(url).then((response) => {
                this.$MyApp.setLoading(false);
                this.loadActions();
                this.$MyApp.success(this.$t('message.SentForReprocessingSuccessfully'));
            }).catch((error) => {
                this.$MyApp.setLoading(false);
                this.$MyApp.error(error);
            });
        },
    },
    mounted() {
    },
}
</script>

<style scoped>
::v-deep .v-skeleton-loader__bone::after{
    background-color: rgba(var(--v-theme-info)) !important;
}
</style>