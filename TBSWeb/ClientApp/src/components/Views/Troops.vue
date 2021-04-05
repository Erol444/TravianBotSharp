<template>
    <b-overlay
        :show="!waiting"
        rounded="sm"
    >
        <div v-if="login">
            <div>Troops</div>
        </div>
        <div v-else>
            Please login first
        </div>
    </b-overlay>
</template>

<script>
    import { isLogin, sleep } from '@/utilities';
    import { current } from '@/controller/AccountController';
    import { EventBus } from '@/EventBus';

    export default {
        name: 'ViewTroops',
        data: function () {
            return {
                waiting: false,
                login: false,
            };
        },
        created: async function () {
            const index = current.account;
            await this.checkLogin(index);

            EventBus.$on('account_change', this.checkLogin);
            EventBus.$on('driver_login', async (index) => {
                await sleep(10);
                await this.checkLogin(index);
            });
            EventBus.$on('driver_logout', this.checkLogin);
        },
        methods: {
            checkLogin: async function (index) {
                this.waiting = false;
                this.login = await isLogin(index);
                this.waiting = true;
            },
        },
    };
</script>
