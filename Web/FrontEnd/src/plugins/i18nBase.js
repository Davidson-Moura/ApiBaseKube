import { createI18n } from 'vue-i18n'
import PtBr from '@/i18n/ptBr'
import En from '@/i18n/en'

const i18n = createI18n({
    locale: 'pt-br',
    messages: {
        'pt-br': PtBr,
        en: En
    }
});


export default i18n
