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
    import { current, getTasks } from '@/controller/AccountController';
    import { EventBus } from '@/EventBus';
    import { signalR } from '@aspnet/signalr';

    export default {
        name: 'ViewDebug',

        data: function () {
            return {
                waiting: false,
                login: false,
                logging: '',
                tasks: [],
                connection: null,
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

            this.connection = new signalR.HubConnectionBuilder()
                .withUrl('/realtime/log')
                .configureLogging(signalR.LogLevel.Information)
                .build();
            this.connection.start().catch(function (err) {
                return console.error(err.toSting());
            });
        },
        mounted: function () {
            const thisVue = this;
            thisVue.connection.start();
            thisVue.connection.on('LogUpdate', function (index, message) {
                thisVue.logging += message;
            });
        },
        methods: {
            checkLogin: async function (index) {
                this.waiting = false;
                this.login = await isLogin(index);

                if (this.login) {
                    await this.getList(index);
                }

                this.waiting = true;
            },

            getList: async function (index) {
                this.tasks = await getTasks(index);
            },
        },
    };
</script>
