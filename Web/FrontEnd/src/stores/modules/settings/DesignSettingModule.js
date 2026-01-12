import MyAppSrc from '@/plugins/myApp';

const state = {
    Setting: {
        LogoExtension: ".svg",
        ApplicationName: "Construa+",
        Currency: ""
    }
}

const actions = {
    set({ state, commit, rootState }) {
        commit('setConnectedSignalR', true)
    },
}
function setTheme(data) {
    document.title = data.ApplicationName; //Adicionando tiluto

    setStyle(data);

    /*
    this.$vuetify.theme.themes.customTheme.colors.primary = data.PrimaryColor;
    this.$vuetify.theme.themes.customTheme.colors.secondary = '#f71000';//data.SecondColor;
    //this.$vuetify.theme.themes.customTheme.colors.background = '#f71000'; // cor de fundo
    //this.$vuetify.theme.themes.customTheme.colors['surface-variant'] = '#f71000';
    //this.$vuetify.theme.themes.customTheme.colors['on-surface-variant'] = '#f71000';
    //this.$vuetify.theme.themes.customTheme.colors['primary-darken-1'] = '#f71000';
    //this.$vuetify.theme.themes.customTheme.colors['secondary-darken-1'] = '#f71000';
    //this.$vuetify.theme.themes.customTheme.colors.error = '#f71000';
    this.$vuetify.theme.themes.customTheme.colors.info = '#f71000';
    /*
    this.$vuetify.theme.themes.customTheme.colors.success = 'red';
    this.$vuetify.theme.themes.customTheme.colors.warning = 'red';
    this.$vuetify.theme.themes.customTheme.colors.second = 'red';
    */
    
}
function setStyle(data) {
    if(!data) return;
    var styleTag = document.createElement("style");
    var head = document.getElementsByTagName("head")[0];
    head.appendChild(styleTag);

    var sheet = styleTag.sheet ? styleTag.sheet : styleTag.styleSheet;

    var styleString = ".v-theme--customTheme { --color-teste:red; ";
    
    styleString += data.PrimaryColor ? "--v-theme-primary: " + `${data.PrimaryColor}; ` : "";
    
    styleString += data.PrimaryColor ? "--v-theme-surface: " + `${data.PrimaryColor}; ` : "";
    styleString += data.FontColor ? "--v-theme-on-surface: " + `${data.FontColor}; ` : "";

    styleString += data.SecondaryColor ? "--v-theme-secondary: " + `${data.SecondaryColor}; ` : "";
    styleString += data.FontColor ? "--v-theme-on-secondary: " + `${data.FontColor}; ` : "";

    styleString += data.BackgroundColor ? "--v-theme-background: " + `${data.BackgroundColor}; ` : "";

    styleString += data.FontColor ? "--v-theme-on-surface: " + `${data.FontColor}; ` : "";
    styleString += data.FontColor ? "--v-theme-on-background: " + `${data.FontColor}; ` : "";
    styleString += data.FontColor ? "--v-font-color: " + `${data.FontColor}; ` : "";

    styleString += data.SuccessColor ? "--v-theme-success: " + `${data.SuccessColor}; ` : "";
    styleString += data.FontColor ? "--v-theme-on-success: " + `${data.FontColor}; ` : "";
    
    styleString += data.ErrorColor ? "--v-theme-error: " + `${data.ErrorColor}; ` : "";
    styleString += data.FontColor ? "--v-theme-on-error: " + `${data.FontColor}; ` : "";
    
    styleString += data.InfoColor ? "--v-theme-info: " + `${data.InfoColor}; ` : "";
    styleString += data.InfoColor ? "--v-theme-surface-light: " + `${data.InfoColor}; ` : "";
    styleString += data.FontColor ? "--v-theme-on-info: " + `${data.FontColor}; ` : "";

    styleString += data.WarningColor ? "--v-theme-warning: " + `${data.WarningColor}; ` : "";
    styleString += data.FontColor ? "--v-theme-on-warning: " + `${data.FontColor}; ` : "";

    styleString += data.PrimaryBackgroundColor ? "--primary-background-color: " + `${data.PrimaryBackgroundColor}; ` : "";
    styleString += data.SecondaryBackgroundColor ? "--secondary-background-color: " + `${data.SecondaryBackgroundColor}; ` : "";

    

    styleString += "}";

    //sheet.insertRule(styleString, 0);
    if (sheet.insertRule) {
        sheet.insertRule(styleString, 0);
    }
    else {
        sheet.addRule(styleString, 0);
    }
}
const mutations = {
    setSetting(state, setting) {
        state.Setting = setting;
        setTheme(setting);
    }
}
const getters = {
    GetSetting: (state) => state.Setting
}

export const designSettingModule = {
    namespaced: true,
    state,
    actions,
    mutations,
    getters
}