<template>
    <b-card no-body>
        <b-button-group
            vertical
        >
            <b-button-group class="mb-1">
                <b-button
                    variant="danger"
                >
                    Add account
                </b-button>
            </b-button-group>
            <b-button-group class="mb-1">
                <b-button
                    variant="success"
                    class="mr-1"
                >
                    Edit
                </b-button>
                <b-button
                    variant="success"
                >
                    Delete
                </b-button>
            </b-button-group>
            <b-button-group class="mb-1">
                <b-button
                    variant="warning"
                    class="mr-1"
                >
                    Login
                </b-button>
                <b-button
                    variant="warning"
                >
                    Logout
                </b-button>
            </b-button-group>
            <b-button-group class="mb-1">
                <b-button
                    variant="info"
                    class="mr-1"
                >
                    Login all
                </b-button>
                <b-button
                    variant="info"
                >
                    Logout all
                </b-button>
            </b-button-group>
        </b-button-group>
    </b-card>
</template>
<script>
    export default {
        name: 'LoginButton',
    };
</script>
