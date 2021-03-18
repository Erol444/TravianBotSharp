<template>
    <table style="border-left-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-top-width: 1px">
        <tbody>
            <tr>
                <td colspan="2">
                    <button type="button" class="btn btn-primary btn-block">Add account</button>

                </td>
            </tr>
            <tr>
                <td>
                    <button type="button" class="btn btn-success btn-block">Login</button>

                </td>
                <td>
                    <button type="button" class="btn btn-dark btn-block">Logout</button>

                </td>
            </tr>
            <tr>
                <td>
                    <button type="button" class="btn btn-info btn-block">Edit</button>

                </td>
                <td>
                    <button type="button" class="btn btn-danger btn-block">Delete</button>

                </td>
            </tr>
            <tr>
                <td>
                    <button type="button" class="btn btn-success btn-block">Login all</button>
                </td>
                <td>
                    <button type="button" class="btn btn-dark btn-block">Logout all</button>
                </td>
            </tr>
        </tbody>
    </table>
</template>
<script>
    export default {
        name: 'LoginButton'
    }
</script>
