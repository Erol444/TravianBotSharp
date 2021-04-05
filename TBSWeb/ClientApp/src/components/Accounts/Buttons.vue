<template>
    <b-card no-body>
        <b-button-group
            vertical
        >
            <b-button-group class="mb-2 mt-2 ml-2">
                <b-button
                    variant="danger"
                    class="mr-3"
                    @click="account_add()"
                >
                    Add account
                </b-button>
            </b-button-group>
            <b-button-group class="mb-2 ml-2">
                <b-button
                    variant="success"
                    class="mr-1"
                    @click="account_edit()"
                >
                    Edit
                </b-button>
                <b-button
                    variant="success"
                    class="mr-3"
                    @click="account_delete()"
                >
                    Delete
                </b-button>
            </b-button-group>
            <b-button-group class="mb-2 ml-2">
                <b-button
                    variant="warning"
                    class="mr-1"
                    @click="account_login()"
                >
                    Login
                </b-button>
                <b-button
                    variant="warning"
                    class="mr-3"
                    @click="account_logout()"
                >
                    Logout
                </b-button>
            </b-button-group>
            <b-button-group class="mb-2 ml-2">
                <b-button
                    variant="info"
                    class="mr-1"
                >
                    Login all
                </b-button>
                <b-button
                    variant="info"
                    class="mr-3"
                >
                    Logout all
                </b-button>
            </b-button-group>
        </b-button-group>
    </b-card>
</template>
<script>
    import { router } from '@/router';
    import { current, deleteAccount } from '@/controller/AccountController';
    import { login, logout } from '@/controller/DriverController';
    import { EventBus } from '@/EventBus';
    export default {
        name: 'LoginButton',
        methods: {
            account_add: function () {
                router.push({ name: 'add_account' });
            },
            account_edit: function () {
                if (current.account === -1) {
                    return;
                }
                const index = current.account;
                router.push({ name: 'edit_account', params: { id: index } });
            },
            account_delete: async function () {
                if (current.account === -1) {
                    return;
                }
                const index = current.account;
                await deleteAccount(index);
                EventBus.$emit('update_accountlist');
            },
            account_login: async function () {
                if (current.account === -1) {
                    return;
                }
                const index = current.account;
                await login(index);
                EventBus.$emit('driver_login', index);
            },
            account_logout: async function () {
                if (current.account === -1) {
                    return;
                }
                const index = current.account;
                await logout(index);
                EventBus.$emit('driver_logout', index);
            },
        },

    };
</script>
