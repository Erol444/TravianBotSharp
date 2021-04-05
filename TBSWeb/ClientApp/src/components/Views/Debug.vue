<template>
    <b-overlay
        :show="!waiting"
        rounded="sm"
    >
        <div v-if="login">
            <b-container fluid>
                <b-row>
                    <b-col>
                        <b-table-lite
                            bordered
                            hover
                            fixed
                            responsive
                            sticky-header
                            :items="tasks"
                        />
                    </b-col>
                    <b-col>
                        <b-form-textarea
                            id="textarea"
                            v-model="logger"
                            rows="10"
                            max-rows="10"
                            readonly
                        />
                    </b-col>
                </b-row>
            </b-container>
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
        name: 'ViewDebug',

        data: function () {
            return {
                waiting: false,
                login: false,
                logger: '11:23:15: Chrome will reopen in 60 min',
                tasks: [
                    { Task: 'Sleep', Village: 'VinaTown', Priortity: 'Medium', Stage: 'Start', ExecuteAt: '21/03/2021 11:01:52' },
                    { Task: 'Sleep', Village: 'VinaTown', Priortity: 'Medium', Stage: 'Start', ExecuteAt: '21/03/2021 11:01:52' },
                    { Task: 'Sleep', Village: 'VinaTown', Priortity: 'Medium', Stage: 'Start', ExecuteAt: '21/03/2021 11:01:52' },
                ],
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
