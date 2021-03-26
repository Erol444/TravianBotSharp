<template>
    <div>
        <b-table
            responsive
            sticky-header
            hover
            :items="accs"
        />
    </div>
</template>

<script>
    import { getListAccounts } from '../../controller/AccountController';
    export default {
        name: 'ListAccount',
        data () {
            return {
                fields: ['username', 'server'],
                accs: [],
                listaccs: [],
            };
        },
        async mounted () {
            this.listaccs = await getListAccounts();

            this.listaccs.forEach(acc => {
                this.accs.push({ username: acc.username, server: acc.serverUrl.replace(/(^\w+:|^)\/\//, '') });
            });
        },
    };
</script>

<!--<style lang="scss" scoped>
    .md-field {
        max-width: 300px;
    }
</style>-->
