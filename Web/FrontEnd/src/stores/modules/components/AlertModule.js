const state = {
    alert: {
        show: false,
        message: "",
        classType: ""
    },
    notify: {
        withNotification: false,
        show: false,
        message: "",
        classType: ""
    }
}

const actions = {
}
const mutations = {
    setAlert(state, alert) {
        state.alert = alert;
        state.alert.show = true;
        setTimeout(() => {
            if (state.alert)
                state.alert.show = false;
        }, 8000)
    },
    setShowAlert(state, show) {
        state.alert.show = show;
    },

    setNotify(state, notify) {
        state.notify = notify;
        state.notify.show = true;
        state.notify.withNotification = true;
        setTimeout(() => {
            if (state.notify)
                state.notify.show = false;
        }, 8000)
    },
    setShowNotify(state, show) {
        state.notify.show = show;
    },
    setWithNotification(state, withNot) {
        state.notify.withNotification = withNot;
    }
}
const getters = {
    WithNotification: (state) => state.notify.withNotification,
}

export const alertModule = {
    namespaced: true,
    state,
    actions,
    mutations,
    getters
}