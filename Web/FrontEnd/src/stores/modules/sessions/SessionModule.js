const state = {
    sessionManager: {
        UserId: "",
        UserName: "",
        UserEmail: "",
        Token: "",
        ExpiresAt: "",
        IsAdmin: false,
        Permissions: []
    }
}

const actions = {
    loadLocalStorage(context){
        const sessionManagerString = localStorage.getItem("sessionManager");
        if(sessionManagerString)
          context.state.sessionManager = JSON.parse(sessionManagerString);
    }
}
const mutations = {
    setSessionManager(state, sessionManager) {
        state.sessionManager = sessionManager;
        localStorage.setItem("sessionManager", JSON.stringify(state.sessionManager));
    },
    setPermissions(state, permissions) {
        state.sessionManager.Permissions = permissions;
    },
    logout(state){
        state.sessionManager = {
            UserId: "",
            UserName: "",
            UserEmail: "",
            Token: "",
            ExpiresAt: "",
            IsAdmin: false,
            Permissions: []
        };
        localStorage.removeItem("sessionManager");
    }
}
const getters = {
    IsAdmin: (state) => state.sessionManager.IsAdmin,
    GetToken: (state) => state.sessionManager.Token,
    IsLogged: (state) => !(!state.sessionManager.Token),
    GetSession: (state) => state.sessionManager,
    GetPermissions: (state) => state.sessionManager.Permissions,
}

export const sessionModule = {
    namespaced: true,
    state,
    actions,
    mutations,
    getters
}