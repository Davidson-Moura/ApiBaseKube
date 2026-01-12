import axios from 'axios';
import router from '@/router/index';
import i18n from '@/plugins/i18nBase';
import store from '@/stores/store';
import myApp from '@/plugins/myApp';

const ax = axios.create({
    baseURL: "",
    headers: {
        "Access-Control-Allow-Origin": "*",
        'Access-Control-Allow-Methods': '*',
        'Authorization': '',
        'Access-Control-Expose-Headers': 'access-control-allow-origin,access-control-allow-methods,access-control-allow-headers',
        
        get: {
            'Accepts': "application/json",
            "Content-Type": "application/json"
        }
    },
    timeout: 5000
});

ax.interceptors.request.use(
    settings => {
        Promise.resolve(true);
        if(settings.customHeaders){
            settings.headers = settings.customHeaders;
        }
        if(!settings.externalURL){
            settings.headers.Authorization = `bearer ${store.getters['sessionModule/GetToken']}`;
            settings.url = settings.url.startsWith('/') ? settings.url : "/" + settings.url
        }
        return settings;
    },
    erro => {
        return Promise.reject(erro);
    }
);

export default {
    install: (app, options) => {
        ax.defaults.baseURL = options.baseUrl;
        ax.defaults.timeout = 300000;
        const handleResponse = (response) => { //se deu certo
            return Promise.resolve(response);
        };

        const handleError = (err) => { // se deu errado
            if (err?.response?.status == 403) {
                if(!err.response) err.response = {};
                err.response.data = err.message = i18n.global.t("message.YouDoNotHavePermissionToPerformTheAction");
                router.push("/app");
            }
            if (err?.response?.status == 401) {
                router.push("/login");
                if(!err.response) err.response = {};
                err.response.data = err.message = i18n.global.t("message.YouNotAuthorizationAcess");
                
                store.commit("sessionModule/setSessionManager", {
                    Token: "",
                    IsSuper: false
                });
            }else if(err?.code == "ERR_NETWORK"){
                if(!err.response) err.response = {};
                err.response.data = err.message = i18n.global.t("message.CouldNotConnectToServer");
            }

            let msg = err;
            if(typeof(msg) !== 'string') msg = err?.response;
            if(typeof(msg) !== 'string') msg = err?.response?.data;
            if(typeof(msg) !== 'string') msg = err?.response?.data?.Description;

            myApp.MyApp.error(msg);
            console.log(err.response.data);

            return Promise.reject(err);
        }
        ax.interceptors.response.use(handleResponse, handleError);
        app.config.globalProperties.$axios = ax;
    },
    ax
}