import store from '@/stores/store';
import apiSrc from '@/plugins/api';
import axiosApi from '@/plugins/axiosApi';
import { useRouter } from 'vue-router';
import sessionSrc from '@/plugins/session';
import i18n from '@/plugins/i18nBase';

const MyApp = {
    pageSize: 10,
    baseAlert(msg, classType) {
        store.commit('alertModule/setAlert', { message: msg, classType: classType });
    },
    success(msg) {
        this.baseAlert(msg, "success");
    },
    error(msg) {
        if (msg && msg.response && msg.response.data) msg = msg.response.data;
        this.baseAlert(msg, "error");
        console.log(msg);
    },
    info(msg) {
        this.baseAlert(msg, "info");
    },
    getUser() {
        return store.getters['sessionModule/GetSession'];
    },
    isLogged() {
        return store.getters['sessionModule/IsLogged'];
    },
    setLoading(v) {
        let lastValue = store.getters['applicationModule/inLoading'];
        store.commit('applicationModule/setLoadComponent', v);

        return lastValue;
    },

    baseNotify(msg, classType) {
        store.commit('alertModule/setNotify', { message: msg, classType: classType, withNotification: true });
    },
    sendNotification(n) {
        this.baseNotify(n.message, "info");
    },
    readNotification() {
        store.commit('alertModule/setWithNotification', false);
    },

    isSmall: () => MyApp.getScreenSize() == 'sml',
    getScreenSize() {
        const width = window.innerWidth;
        if (width <= 768) return "sml";
        else if (width <= 1024) return "md";
        else return "lg"
    },
    getPageSize() {
        return 10;
    },
    openInNewTab(router, route, params) {
        const url = router.resolve({
            name: route,
            params: params
        }).href
        window.open(url, '_blank')
    },
    printBase64(x) {
        const win = window.open("");
        win.document.write(`
            <html>
            <head><title></title></head>
            <body style="margin:0; display:flex; justify-content:center; align-items:center; height:100vh;">
                <img src="${x}" style="max-width:100%; max-height:100%;" />
                <script>
                window.onload = function() {
                    window.print();
                    window.onafterprint = function() { window.close(); };
                };
                <\/script>
            </body>
            </html>
        `);
        win.document.close();
    },
    getLogoUrl() {
        return "/imgs/Logo.png";
        //let token = store.getters['sessionModule/GetToken'];
        var urlBase = axiosApi.ax.defaults.baseURL;
        if (!urlBase.endsWith("/")) urlBase += "/";

        let url = urlBase + apiSrc.api.logoUrl;

        return url;
    },
    logout() {
        document.cookie = "AuthToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        axiosApi.ax.get("Authentication/Logout/v1").then(() => { });
        store.commit('sessionModule/logout');
        sessionSrc.Session.stopConnection();

        const router = useRouter();
        router.push({ name: "Login" });
    },
    mainMenu: [],
    loadMainMenus(force) {
        if (MyApp.mainMenu && MyApp.mainMenu.length > 0 && !force) return new Promise((rs, rj) => rs(MyApp.mainMenu));

        return axiosApi.ax.get(apiSrc.api.mainMenus).then((response) => {
            MyApp.mainMenu = response.data;

            return MyApp.mainMenu;
        });
    },
    loadCountMainMenus() {
        const menuflat = MyApp.mainMenu.reduce((acc, val) => acc.concat(val.list), []);
        if (MyApp.mainMenu && MyApp.mainMenu.length > 0 && menuflat.some(x => x.count > 0)) return new Promise((rs, rj) => rs(MyApp.mainMenu));

        return axiosApi.ax.get(apiSrc.api.countMainMenus).then((response) => {
            const menuflat = MyApp.mainMenu.reduce((acc, val) => acc.concat(val.list), []);
            response.data?.forEach(cnt => {
                var menu = menuflat.find(x => x.key == cnt.key);
                menu.count = cnt.count;
            });

            return MyApp.mainMenu;
        });
    },
    myPermissions: [],
    loadPermissions(force) {
        //if (MyApp.myPermissions && MyApp.myPermissions.length > 0 && !force) return new Promise((rs, rj) => rs(MyApp.myPermissions));

        let permissions = store.getters['sessionModule/GetPermissions'];
        if (permissions && permissions.length > 0 && !force) return new Promise((rs, rj) => rs(permissions));
        
        return axiosApi.ax.get(apiSrc.api.myPermissions)
            .then((response) => {
            store.commit('sessionModule/setPermissions', response.data);
            //MyApp.myPermissions = response.data;
            return MyApp.myPermissions;
        });
            
    },
    hasPermission(permission) {
        let permissions = store.getters['sessionModule/GetPermissions'] ?? [];
        return permissions.some(x => x == permission);
    },


    setLogoTab(uri){
        const link = document.querySelector("link[rel~='icon']");
        if (link) {
            link.href = uri;
        } else {
            const newLink = document.createElement("link");
            newLink.rel = "icon";
            newLink.href = uri;
            document.head.appendChild(newLink);
        }
    }
};

export default {
    install: (app, options) => {
        app.config.globalProperties.$MyApp = MyApp;
    },
    MyApp
}