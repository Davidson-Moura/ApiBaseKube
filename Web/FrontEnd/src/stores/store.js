import { createStore } from 'vuex';
import { sessionModule } from '@/stores/modules/sessions/SessionModule';
import { alertModule } from '@/stores/modules/components/AlertModule';
import { applicationModule } from '@/stores/modules/ApplicationModule';
import { designSettingModule } from '@/stores/modules/settings/DesignSettingModule';


export default createStore({
  modules: {
    sessionModule: sessionModule,
    alertModule: alertModule,
    applicationModule: applicationModule,
    designSettingModule: designSettingModule
  }
})