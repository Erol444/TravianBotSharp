<template>
    <b-container fluid>
        <b-row>
            <b-col sm="auto">
                <div class="mt-3">
                    <b-table
                        hover
                        bordered
                        :items="villages"
                    />
                </div>
            </b-col>
            <b-col>
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
            </b-col>
        </b-row>
    </b-container>
</template>

<script>
    import Build from './VIllagesViews/Building';
    import Market from './VIllagesViews/Market';
    import Troops from './VIllagesViews/Troops';
    import Attack from './VIllagesViews/Attack';
    import Farming from './VIllagesViews/Farming';
    import Info from './VIllagesViews/Info';
    export default {
        name: 'ViewVillages',
        data () {
            return {
                villages: [
                    { name: 'VinaTown'/*, coord: '2|9', type: '4446', resource: 'FFFF' */ },
                    { name: 'VinaVillage'/*, coord: '19|45', type: '4446', resource: 'FFFF' */ },
                    { name: 'VinaPhalanx'/*, coord: '30|4', type: '4446', resource: 'FFFF' */ },
                    { name: 'VinaDruider'/*, coord: '19|75', type: '4446', resource: 'FFFF' */ },
                ],
                tabs: [
                    { title: 'Build', component: Build },
                    { title: 'Market', component: Market },
                    { title: 'Troops', component: Troops },
                    { title: 'Attack', component: Attack },
                    { title: 'Farming', component: Farming },
                    { title: 'Info', component: Info },
                ],
            };
        },

    };
</script>
