<template>
    <b-card no-body>
        <b-button-group
            vertical
            class="mb-1"
        >
            <b-button-group>
                <b-button
                    variant="danger"
                    block="true"
                >
                    Add account
                </b-button>
            </b-button-group>
            <b-button-group>
                <b-button
                    variant="success"
                >
                    Edit
                </b-button>
                <b-button
                    variant="success"
                >
                    Delete
                </b-button>
            </b-button-group>
            <b-button-group
                variant="warning"
            >
                <b-button class="mr-1">
                    Login
                </b-button>
                <b-button>
                    Logout
                </b-button>
            </b-button-group>
            <b-button-group>
                <b-button
                    variant="info"
                    class="mb-1 mr-1"
                >
                    Login all
                </b-button>
                <b-button
                    variant="info"
                    class="mb-1"
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
