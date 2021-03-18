<template>
    <div>
        <b-tabs content-class="mt-3">
            <b-tab title="General"><General /></b-tab>
            <b-tab title="Second"><p>I'm the second tab</p></b-tab>
            <b-tab title="Disabled"><p>I'm a disabled tab!</p></b-tab>
        </b-tabs>
    </div>
</template>

<script>
    import General from './General'

    export default {
        name: 'Views',
        components: {
            General
        }
    };
</script>