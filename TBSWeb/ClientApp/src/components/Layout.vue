<template>
    <b-container
        style="margin: 25px 25px;"
        fluid
    >
        <b-row>
            <b-col md="auto">
                <b-container>
                    <b-row
                        align-h="center"
                        align-v="center"
                    >
                        <h3 class="mt-3">
                            Travian Bot Sharp
                        </h3>
                    </b-row>
                    <b-row
                        align-h="center"
                        class="mt-3"
                    >
                        <Buttons />
                    </b-row>
                    <b-row
                        align-h="center"
                        class="mt-3"
                    >
                        <ListAccount />
                    </b-row>
                </b-container>
            </b-col>
            <b-col>
                <Views />
            </b-col>
        </b-row>
    </b-container>
</template>

<script>
    import Buttons from './Accounts/Buttons';
    import ListAccount from './Accounts/ListAccount';

    import Views from './Views/Views';

    export default {
        name: 'Layout',
        components: {
            Buttons,
            ListAccount,
            Views,
        },
    };

</script>

<style scoped>
    table td {
        vertical-align: top;
        text-align: center;
    }

    .layout {
        border: 1px solid rgb(3, 3, 3);
        border-collapse: collapse;
    }
    .center {
        text-align: center;
        vertical-align: middle;
    }
</style>
