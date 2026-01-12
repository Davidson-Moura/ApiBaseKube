/**
 * plugins/vuetify.js
 *
 * Framework documentation: https://vuetifyjs.com`
 */

// Styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import { pt } from 'vuetify/locale'

// Composables
import { createVuetify } from 'vuetify'
import { VFileUpload } from 'vuetify/labs/VFileUpload'

const customTheme = {
  dark: false,
  colors: {
    background: '#FFFFFF',
    surface: '#FFFFFF',
    primary: '#002244',
    'primary-darken-1': '#FFFFFF',
    secondary: '#4F4F4F',
    'secondary-darken-1': '#FFFFFF',
    error: '#E20003',
    info: '#2196F3',
    success: '#4CAF50',
    warning: '#FB8C00'
  },
}

// https://vuetifyjs.com/en/introduction/why-vuetify/#feature-guides
export default createVuetify({
  locale: {
    locale: 'pt',
    messages: { pt },
  },
  components: {
    VFileUpload
  },
  theme: {
    defaultTheme: 'customTheme',
    themes: {
      customTheme,
    },
  },
  defaults: {
    VTextField: {
      variant: 'solo-filled',
      "hide-details": "auto",
      density: "compact",
      dense: true,
      clearable: true
    },
    VTextarea: {
      variant: 'solo-filled',
      "hide-details": "auto",
      density: "compact"
    },
    VSelect:{
      variant: 'solo-filled',
      "hide-details": "auto",
      density: "compact"
    },
    VCheckbox:{
      variant: 'solo-filled',
      "hide-details": "auto",
      density: "compact"
    },
    VNumberInput:{
      variant: 'solo-filled',
      "hide-details": "auto",
      density: "compact"
    },
  },
})
