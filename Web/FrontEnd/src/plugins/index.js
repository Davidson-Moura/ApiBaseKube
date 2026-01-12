import { loadFonts } from './webfontloader';
import vuetify from './vuetify';
import router from '../router';
import store from '@/stores/store';
import i18n from '@/plugins/i18nBase';
import axiosApi from '@/plugins/axiosApi';
import myApp from '@/plugins/myApp';
import api from '@/plugins/api';
import permission from '@/plugins/permission';
import session from '@/plugins/session';
import VueApexCharts from "vue3-apexcharts";

export function registerPlugins (app) {
  loadFonts();
  
  let port = 7362
  let url = `https://localhost:${port}/`;

  app
  .use(vuetify)
  .use(router)
  .use(store)
  .use(i18n)
  .use(api)
  .use(permission)
  .use(axiosApi,{
    baseUrl: process.env.NODE_ENV === 'development' ?
    url 
    : window.location.origin,
  })
  .use(myApp)
  .use(session)
  .use(VueApexCharts);
  ;
}
