import '@babel/polyfill';
import 'mutationobserver-shim';
import './plugins/bootstrap-vue';
import Vue from 'vue';
import App from './App.vue';

// eslint-disable-next-line no-unused-vars
const app = new Vue({
    render: h => h(App),
}).$mount('#app');
