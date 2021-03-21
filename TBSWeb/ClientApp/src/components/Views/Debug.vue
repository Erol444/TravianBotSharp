<template>
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
</template>

<script>
    export default {
        name: 'ViewDebug',
        data: () => ({
            logger: '11:23:15: Chrome will reopen in 60 min',
            tasks: [
                { Task: 'Sleep', Village: 'VinaTown', Priortity: 'Medium', Stage: 'Start', ExecuteAt: '21/03/2021 11:01:52' },
                { Task: 'Sleep', Village: 'VinaTown', Priortity: 'Medium', Stage: 'Start', ExecuteAt: '21/03/2021 11:01:52' },
                { Task: 'Sleep', Village: 'VinaTown', Priortity: 'Medium', Stage: 'Start', ExecuteAt: '21/03/2021 11:01:52' },
            ],
        }),
    };
</script>
