<template>
    <b-overlay
        :show="!waiting"
        rounded="sm"
    >
        <div v-if="login">
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
                                lazy
                            >
                                <b-tab
                                    v-for="(tab, index) in tabs"
                                    :key="index"
                                    :title="tab.title"
                                >
                                    <component :is="tab.component" />
                                </b-tab>
                            </b-tabs>
                        </b-card>
                    </b-col>
                </b-row>
            </b-container>
        </div>
        <div v-else>
            Please login first
        </div>
    </b-overlay>
</template>

<script>
    import Build from './VIllagesViews/Building';
    import Market from './VIllagesViews/Market';
    import Troops from './VIllagesViews/Troops';
    import Attack from './VIllagesViews/Attack';
    import Farming from './VIllagesViews/Farming';
    import Info from './VIllagesViews/Info';

    import { isLogin, sleep } from '@/utilities';
    import { current } from '@/controller/AccountController';
    import { EventBus } from '@/EventBus';
    export default {
        name: 'ViewVillages',
        data: function () {
            return {
                waiting: false,
                login: false,
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
        created: async function () {
            const index = current.account;
            await this.checkLogin(index);

            EventBus.$on('account_change', this.checkLogin);
            EventBus.$on('driver_login', async (index) => {
                await sleep(10);
                await this.checkLogin(index);
            });
            EventBus.$on('driver_logout', this.checkLogin);
        },
        methods: {
            checkLogin: async function (index) {
                this.waiting = false;
                this.login = await isLogin(index);
                this.waiting = true;
            },
        },

    };
</script>
