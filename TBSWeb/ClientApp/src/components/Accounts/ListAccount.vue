<template>
    <div>
        <b-table
            responsive
            sticky-header
            striped
            hover
            :items="accs"
        />
    </div>
</template>

<script>

    export default {
        name: 'ListAccount',
        data: () => ({
            fields: ['username', 'server'],
            accs: [
                { username: 'vinaghost', server: 'ts4' },
                { username: 'adasdsad', server: 'tsasdas4' },
                { username: 'adasdsad', server: 'tsasdas4' },
                { username: 'adasdsad', server: 'tsasdas4' },
                { username: 'adasdsad', server: 'tsasdas4' },
            ],
        }),
    };
</script>

<!--<style lang="scss" scoped>
    .md-field {
        max-width: 300px;
    }
</style>-->
