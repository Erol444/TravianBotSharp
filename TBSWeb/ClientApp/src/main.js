import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import { BootstrapVue } from 'bootstrap-vue'

import { h, createApp } from 'vue'
import App from './App.vue'

createApp({ render: () => h(App) }).use(BootstrapVue).mount('#app')