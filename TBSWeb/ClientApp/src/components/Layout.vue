<template>
    <table
        style="margin: 25px 25px;"
        cellspacing="10"
        cellpadding="10"
    >
        <tbody>
            <tr>
                <td>Travian Bot Sharp</td>
                <td rowspan="3">
                    <Views />
                </td>
            </tr>
            <tr class=" center">
                <td>
                    <center>
                        <Buttons />
                    </center>
                </td>
            </tr>

            <tr>
                <td>
                    <ListAccount />
                </td>
            </tr>
        </tbody>
    </table>
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
