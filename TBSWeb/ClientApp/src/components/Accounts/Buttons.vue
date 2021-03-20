<template>
    <b-button-group
        vertical
    >
        <b-button-group>
            <b-button
                variant="danger"
                block="true"
                class="mb-1"
            >
                Add account
            </b-button>
        </b-button-group>
        <b-button-group>
            <b-button
                variant="success"
                class="mb-1 mr-1"
            >
                Edit
            </b-button>
            <b-button
                variant="success"
                class="mb-1"
            >
                Delete
            </b-button>
        </b-button-group>
        <b-button-group>
            <b-button
                variant="warning"
                class="mb-1 mr-1"
            >
                Login
            </b-button>
            <b-button
                variant="warning"
                class="mb-1"
            >
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
</template>
<script>
    export default {
        name: 'LoginButton',
    };
</script>
