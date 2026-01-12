import * as signalR from "@microsoft/signalr";
import axiosApi from '@/plugins/axiosApi';
import myApp from '@/plugins/myApp';
import apiSrc from '@/plugins/api';
import store from '@/stores/store';

const Session = {
    init(){
        Session.configConnection();
        Session.configMethods();
        Session.startConnection();
    },
    verifyConnection(){
        if(Session.connection && Session.connection.state == signalR.HubConnectionState.Disconnected) Session.startConnection();
    },

    //Connection
    connection: null,
    configConnection(){
        let url = axiosApi.ax.defaults.baseURL;
        if(!url.endsWith("/")) url += "/";
        
        let resource = apiSrc.api.sessionHub;

        let token = store.getters['sessionModule/GetToken'];
        let accessToken = { access_token: token };
        const searchParams = Session.encodeQueryData(accessToken);

        let uri = `${url}${resource}?${searchParams}`;

        Session.connection = new signalR.HubConnectionBuilder()
            .withUrl(uri //, {transport: signalR.HttpTransportType.WebSockets }
                )
            .withAutomaticReconnect([1000, 2000, 5000, 10000])
            .configureLogging(signalR.LogLevel.None)
            //.configureLogging(signalR.LogLevel.Information)
            .build();            
    },
    encodeQueryData(data) {
        const searchParams = new URLSearchParams(data);
        return searchParams.toString();
    },
    configMethods(){
        Session.connection.on(Session.methods.ForcedDisconnect, Session.onForcedDisconnect);
        Session.connection.on(Session.methods.UserNotification, Session.onUserNotification);
    },
    startConnection(){
        Session.connection
        .start()
        .then(() => {})
        .catch((err) => console.error("SignalR connection error:", err));
    },
    stopConnection(){
        Session.connection?.stop();
        Session.connection = null;
    },
    //Methods
    methods: {
        ForcedDisconnect: "ForcedDisconnect",
        None: 'None',
        UserSigned: 'UserSigned',
        UserUnsigned: 'UserUnsigned',
        UserNotification: 'UserNotification'
    },
    onForcedDisconnect(msg){
        myApp.MyApp.logout();
        myApp.MyApp.error(msg);
    },
    onUserNotification: (notification) => myApp.MyApp.sendNotification(notification)
};

export default {
    install: (app, options) => {
        app.config.globalProperties.$Session = Session;
    },
    Session
}