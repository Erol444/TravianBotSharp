<template>
    <div>
        <b-card no-body>
            <b-tabs
                pills
                fill
                card
            >
                <b-tab
                    v-for="tab in tabs"
                    :key="tab"
                    :title="tab.title"
                >
                    <component :is="tab.component" />
                </b-tab>
            </b-tabs>
        </b-card>
    </div>
</template>

<script>
    import General from './General';
    import Hero from './Hero';
    import Villages from './Villages';
    import Overview from './Overview';
    import Troops from './Troops';
    import Farming from './Farming';
    import NewVillages from './NewVillages';
    import Defending from './Defending';
    import Quests from './Quests';
    import Discord from './Discord';
    import Debug from './Debug';

    export default {
        name: 'Views',
        components: {
            General,
            Hero,
            Villages,
            Overview,
            Troops,
            Farming,
            NewVillages,
            Defending,
            Quests,
            Discord,
            Debug,
        },
        data: () => ({
            tabs: [
                {
                    title: 'General',
                    component: General,
                },
                {
                    title: 'Hero',
                    component: Hero,
                },
                {
                    title: 'Villages',
                    component: Villages,
                },
                {
                    title: 'Overview',
                    component: Overview,
                },
                {
                    title: 'Troops',
                    component: Troops,
                },
                {
                    title: 'Farming',
                    component: Farming,
                },
                {
                    title: 'New Villages',
                    component: NewVillages,
                },
                {
                    title: 'Defending',
                    component: Defending,
                },
                {
                    title: 'Quests',
                    component: Quests,
                },
                {
                    title: 'Discord',
                    component: Discord,
                },
                {
                    title: 'Debug',
                    component: Debug,
                },
            ],
        }),
    };
</script>
