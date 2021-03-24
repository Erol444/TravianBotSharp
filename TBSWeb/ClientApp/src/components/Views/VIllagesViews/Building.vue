<template>
    <b-card-group deck>
        <b-container fluid>
            <b-row>
                <b-col>
                    <b-container>
                        <b-row>
                            <b-card
                                no-body
                                class="mt-2"
                            >
                                <h5 class="ml-auto mr-auto mt-1">
                                    Build
                                </h5>
                                <b-form-group>
                                    <b-form-select
                                        v-model="Build"
                                        style="width:auto;"
                                        :options="BuildOptions"
                                    /><br>
                                    <div class="mt-2">
                                        Level: <b-form-spinbutton
                                            v-model="AutoFieldsLevel"
                                            inline
                                        />
                                    </div><br>
                                    <div>
                                        <b-button
                                            variant="success"
                                            class="mt-2"
                                        >
                                            Build
                                        </b-button>
                                        <b-button
                                            variant="danger"
                                            class="ml-2 mt-2"
                                        >
                                            Demolish
                                        </b-button>
                                    </div>
                                    <b-form-group />
                                </b-form-group>
                            </b-card>
                        </b-row>
                        <b-row>
                            <b-card
                                no-body
                                class="mt-2"
                            >
                                <b-table
                                    :items="BuildingList"
                                    sticky-header
                                    hover
                                />
                            </b-card>
                        </b-row>
                    </b-container>
                </b-col>
                <b-col>
                    <b-container fluid>
                        <b-row>
                            <b-card no-body>
                                <h5 class="ml-auto mr-auto mt-1">
                                    Current building list
                                </h5>
                                <b-table-lite
                                    hover
                                    :items="CurrentList"
                                />
                            </b-card>
                        </b-row>
                        <b-row>
                            <b-card
                                no-body
                                class="mt-2"
                            >
                                <h5 class="ml-auto mr-auto mt-1">
                                    Building queue
                                </h5>
                                <b-table-lite
                                    sticky-header
                                    hover
                                    :items="BuildingQueue"
                                />
                            </b-card>
                        </b-row>
                    </b-container>
                </b-col>
                <b-col>
                    <b-container fluid>
                        <b-row>
                            <b-card no-body>
                                <h5 class="ml-auto mr-auto mt-1">
                                    Auto build fields resource
                                </h5>
                                <b-form-group>
                                    <b-form-select
                                        v-model="AutoFieldsBuild"
                                        style="width:auto;"
                                        :options="AutoFieldsOptions"
                                    /><br>
                                    <div class="mt-2">
                                        Level: <b-form-spinbutton
                                            v-model="AutoFieldsLevel"
                                            inline
                                        /><br>
                                    </div>
                                    <div class="mt-2">
                                        <b-button variant="success">
                                            Build
                                        </b-button>
                                    </div>
                                </b-form-group>
                            </b-card>
                        </b-row>
                        <b-row>
                            <b-card
                                no-body
                                class="mt-2"
                            >
                                <h5 class="ml-auto mr-auto mt-1">
                                    Auto build building
                                </h5>
                                <b-form-group>
                                    <b-form-select
                                        v-model="AutoBuilding"
                                        style="width:auto;"
                                        :options="AutoBuildingOptions"
                                    /><br>
                                    <div>
                                        <b-button
                                            variant="success"
                                            class="mt-2"
                                        >
                                            Build
                                        </b-button>
                                    </div>
                                </b-form-group>
                            </b-card>
                        </b-row>
                        <b-row>
                            <b-card
                                no-body
                                class="mt-2"
                            >
                                <h5 class="ml-auto mr-auto mt-1">
                                    Insta upgrade
                                </h5>
                                <b-form-group>
                                    <b-button variant="danger">
                                        Off
                                    </b-button> <br>
                                    <div class="mt-2">
                                        use when above
                                    </div>
                                    <b-form-spinbutton
                                        v-model="InstaBuildTime"
                                        inline
                                        class="mt-2"
                                    /> <br>
                                    <div class="mt-2">
                                        hours
                                    </div>
                                </b-form-group>
                            </b-card>
                        </b-row>
                    </b-container>
                </b-col>
            </b-row>
        </b-container>
    </b-card-group>
</template>

<script>
    export default {
        name: 'ViewBuild',
        data () {
            return {
                CurrentList: [
                    { building: 'Cropland', level: 11, time: '09:30 30/04/1975' },
                ],
                BuildingQueue: [
                    { building: 'Cropland', level: 11, slot: 1 },
                    { building: 'Cropland', level: 11, slot: 2 },
                    { building: 'Cropland', level: 21, slot: 3 },
                    { building: 'Cropland', level: 11, slot: 4 },

                ],
                BuildingList: [
                    { slot: 1, type: 'Cropland', level: 10 },
                    { slot: 1, type: 'Cropland', level: 10 },
                    { slot: 1, type: 'Cropland', level: 10 },
                    { slot: 1, type: 'Cropland', level: 10 },
                ],
                AutoFieldsOptions: [
                    'Stonemasons Lodge',
                ],
                AutoBuildingOptions: [
                    'Barrack',
                ],
                BuildOptions: [
                    'Market',
                ],
            };
        },
    };
</script>
