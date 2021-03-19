<template>
    <div>
        <b-tabs
            pills
            fill
            content-class="mt-3"
        >
            <b-tab title="General">
                <General />
            </b-tab>
            <b-tab title="Hero">
                hero
            </b-tab>
            <b-tab title="Villages">
                Villages
            </b-tab>
            <b-tab title="Overview">
                Overview
            </b-tab>
            <b-tab title="Troops">
                Troops
            </b-tab>
            <b-tab title="Farming">
                Farming
            </b-tab>
            <b-tab title="New villages">
                New villages
            </b-tab>
            <b-tab title="Defending">
                <p>Defending</p>
            </b-tab>
            <b-tab title="Quests">
                <p>Quests</p>
            </b-tab>
            <b-tab title="Discord">
                <p>Discord</p>
            </b-tab>
            <b-tab title="Debug">
                <p>Debug</p>
            </b-tab>
        </b-tabs>
    </div>
</template>

<script>
    import General from './General';

    export default {
        name: 'Views',
        components: {
            General,
        },
    };
</script>
