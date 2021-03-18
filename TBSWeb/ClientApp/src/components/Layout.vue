<template>
    <table style="margin: 25px 25px; border-left-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-top-width: 1px" cellspacing="10" cellpadding="10">
        <tbody>
            <tr>
                <td>Travian Bot Sharp</td>
                <td rowspan="3"><Views/></td>
            </tr>
            <tr>
                <td>Account list</td>
            </tr>
            <tr>
                <td><LoginButton/></td>
            </tr>
        </tbody>
    </table>
</template>

<script>
    import LoginButton from "./LoginButton"
    import Views from "./Views/Views"

    export default {
        name: 'Layout',
        components: {
            LoginButton,
            Views
        }
    }
</script>