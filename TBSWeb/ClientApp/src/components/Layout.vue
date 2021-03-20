<template>
    <table
        class="layout"
        style="margin: 25px 25px; border-left-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-top-width: 1px"
        cellspacing="10"
        cellpadding="10"
    >
        <tbody>
            <tr class="layout">
                <td>Travian Bot Sharp</td>
                <td
                    rowspan="3"
                    class="layout"
                >
                    <Views />
                </td>
            </tr>
            <tr class="layout">
                <td>
                    <Buttons />
                </td>
            </tr>

            <tr class="layout">
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
</style>
