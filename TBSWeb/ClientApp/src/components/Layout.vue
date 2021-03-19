<template>
    <table class = "layout" style="margin: 25px 25px; border-left-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-top-width: 1px" cellspacing="10" cellpadding="10">
        <tbody>
            <tr class = "layout">
                <td>Travian Bot Sharp</td>
                <td rowspan="3" class = "layout"><Views/></td>
            </tr>
            <tr class = "layout">
                <td>Account list</td>
            </tr>
            <tr class = "layout">
                <td><LoginButton/></td>
            </tr>
        </tbody>
    </table>
</template>

<script>
import LoginButton from './LoginButton';
import Views from './Views/Views';

export default {
    name: 'Layout',
    components: {
        LoginButton,
        Views,
    },
};
</script>

<style scoped>
table td {
    vertical-align: top;
}
.layout {
    border: 1px solid black;
    border-collapse: collapse;
}
</style>
