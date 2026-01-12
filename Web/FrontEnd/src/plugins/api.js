const api = {
    adminLogin: 'Authentication/AdminLogin/v1',
    login: 'Authentication/Login/v1',
    myPermissions: 'Authentication/MyPermissions/v1',
    
    sessionHub: 'SessionHub',

    users: 'User/v1',
    userByKey: 'User/{0}/v1',
    userChangePassword: 'User/v1/ChangePassword',
    userGetCurrent: 'User/Current/v1',
    userUpdateCurrent: 'User/Current/v1',
    userChangePasswordCurrent: 'User/Current/ChangePassword/v1',

    authorizationGroups: 'AuthorizationGroup/v1',
    authorizationGroupByKey: 'AuthorizationGroup/{0}/v1',
    authorizationGroupGetTree: 'AuthorizationGroup/GetTree/v1',
    
    mainMenus: 'Menu/MainMenus/v1',
    countMainMenus: 'Menu/CountMainMenus/v1',
}
export default {
    install: (app, options) => {
        app.config.globalProperties.$api = api;
    },
    api
}