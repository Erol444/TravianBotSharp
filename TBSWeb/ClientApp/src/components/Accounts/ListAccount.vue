<template>
    <b-card no-body>
        <md-table
            v-model="accs"
            md-card
            md-fixed-header
        >
            <md-table-row
                slot="md-table-row"
                slot-scope="{ item }"
            >
                <md-table-cell md-label="username">
                    {{ item.username }}
                </md-table-cell>
                <md-table-cell md-label="server">
                    {{ item.server }}
                </md-table-cell>
            </md-table-row>
        </md-table>
    </b-card>
</template>

<script>

    export default {
        name: 'ListAccount',
        data: () => ({
            accs: [
                {
                    username: 'vinaghost',
                    server: 'ts4',
                },
                {
                    username: 'adasdsad',
                    server: 'tsasdas4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
                {
                    username: 'vinsadsadsadaghost',
                    server: 'tssadsa4',
                },
            ],
        }),
    };
</script>

<!--<style lang="scss" scoped>
    .md-field {
        max-width: 300px;
    }
</style>-->
