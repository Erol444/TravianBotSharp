<template>
    <b-card
        no-body
        class="mt-5 ml-5 mr-5 mb-5"
    >
        <b-form-group>
            <div class="mt-2 ml-3 mr-5">
                <h5>Username:</h5>
                <b-input v-model="acc.username" />
            </div>
            <div class="mt-2 ml-3 mr-5">
                <h5>Server:</h5>
                <b-input v-model="acc.serverUrl" />
            </div>
            <div class="mt-2 ml-3 mr-5">
                <h5>Accesses:</h5>
                <div class="mb-2">
                    *Press Tab at the end of row for new row
                </div>
                <vue-excel-editor
                    v-model="acc.accesses"
                    new-if-bottom
                    no-paging
                    no-footer
                >
                    <vue-excel-column
                        field="password"
                        label="Password"
                    />
                    <vue-excel-column
                        field="proxy"
                        label="Proxy"
                    />
                    <vue-excel-column
                        field="proxyPort"
                        label="Proxy port"
                        type="number"
                    />
                    <vue-excel-column
                        field="proxyUsername"
                        label="Proxy username"
                    />
                    <vue-excel-column
                        field="proxyPassword"
                        label="Proxy password"
                    />
                    <vue-excel-column
                        field="status"
                        label="Status"
                    />
                </vue-excel-editor>
            </div>
        </b-form-group>
        <div class="mb-3">
            <b-button
                variant="info"
                class="ml-3"
                @click="check"
            >
                Check all proxies
            </b-button>
        </div>
        <div class="mb-3">
            <b-button
                v-if="checkNewAccount(id)"
                variant="success"
                class="ml-3 mr-2"
                @click="add()"
            >
                Add
            </b-button>
            <b-button
                v-if="!checkNewAccount(id)"
                variant="success"
                class="ml-3 mr-2"
                @click="edit()"
            >
                Update
            </b-button>
            <b-button
                variant="danger"
                class="ml-3 mr-2"
                @click="goBack()"
            >
                Cancel
            </b-button>
        </div>
    </b-card>
</template>

<script>
    import { getAccount, addAccount, editAccount } from '../../controller/AccountController';
    import { router } from '@/router';

    export default {
        name: 'AccountForm',
        props: {
            id: {
                type: Number,
                default: -1,
            },

        },
        data: function () {
            return {
                loading: true,
                acc: { },
            };
        },

        mounted: async function () {
            if (this.checkNewAccount(this.id)) {
                this.loading = false;

                this.acc = {
                    username: '',
                    serverUrl: '',
                    accesses: [
                        { password: '', proxy: '', port: 0, proxyusername: '', proxypassword: '', status: false },
                    ],
                };

                return;
            }

            await this.loadAccountData(this.id);
        },

        methods: {
            checkNewAccount: function (index) {
                switch (index) {
                case -1: return true;
                default: return false;
                }
            },

            loadAccountData: async function (index) {
                this.acc = await getAccount(index);
            },

            add: async function () {
                this.removeAccessBlank();
                await addAccount(this.acc);
                router.push('/');
            },

            edit: async function () {
                this.removeAccessBlank();
                await editAccount(this.id, this.acc);
                router.push('/');
            },

            goBack: function () {
                router.push('/');
            },

            removeAccessBlank: function () {
                this.acc.accesses = this.acc.accesses.filter(access => access.password !== '');
            },
        },

    };
</script>
