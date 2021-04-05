<template>
    <b-table
        :key="update"
        class="mt-2 mb-2 ml-2 mr-4"
        responsive
        bordered
        sticky-header
        hover
        :items="accs"
        selectable
        select-mode="single"
        @row-clicked="rowClickHandler"
    />
</template>

<script>
    import { EventBus } from '@/EventBus';
    import { getListAccounts, current } from '@/controller/AccountController';
    export default {
        name: 'ListAccount',
        data: function () {
            return {
                fields: ['username', 'server'],
                accs: [],
                listaccs: [],
                update: true,
            };
        },
        created: async function () {
            await this.getList();
            this.update = !this.update;
            EventBus.$on('update_accountlist', this.updateList);
        },
        methods: {
            rowClickHandler: function (record, index) {
                current.account = index;
                EventBus.$emit('account_change', index);
            },
            getList: async function () {
                this.listaccs = await getListAccounts();
                this.accs.length = 0;
                this.listaccs.forEach(acc => {
                    this.accs.push({ username: acc.username, server: acc.serverUrl });
                });
                current.account = -1;
            },
            updateList: async function () {
                await this.getList();
                this.update = !this.update;
            },
        },
    };
</script>
