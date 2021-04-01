import '@babel/polyfill';
import 'mutationobserver-shim';
import './plugins/bootstrap-vue';
import './plugins/vue-material';
import './plugins/vue-excel-editor';
import './plugins/vue-router';
import Vue from 'vue';
import App from './App.vue';

import { router } from './router';
// eslint-disable-next-line no-unused-vars
const app = new Vue({
    router,
    render: h => h(App),
}).$mount('#app');
