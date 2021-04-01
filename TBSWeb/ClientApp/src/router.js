import VueRouter from 'vue-router';
import AccountForm from '@/components/Accounts/AccountForm';
import Layout from '@/components/Layout';

export const router = new VueRouter({
    routes: [
        { path: '/', name: 'home', component: Layout },
        { path: '/account_info/', name: 'add_account', component: AccountForm },
        { path: '/account_info/:id', name: 'edit_account', component: AccountForm, props: true },
    ],
});
