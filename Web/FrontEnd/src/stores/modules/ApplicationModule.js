const state = {
    ExecuteLoadComponent: false
}

const actions = {
}
const mutations = {
    setLoadComponent(state, isLoad) {
        state.ExecuteLoadComponent = isLoad;
    }
}
const getters = {
    inLoading: (state) => state.ExecuteLoadComponent
}

export const applicationModule = {
    namespaced: true,
    state,
    actions,
    mutations,
    getters
}