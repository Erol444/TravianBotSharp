<template>
    <b-container fluid>
        <b-row>
            <b-col>
                <h5 class="mt-5">
                    Bot controller
                </h5>

                <div class="mb-2">
                    Status:
                </div>
                <b-button
                    id="Restart"
                    variant="warning"
                >
                    Restart
                </b-button>
            </b-col>

            <b-col>
                <h5 class="mt-5">
                    Watch ads
                </h5>
                Watch ad when building takes above
                <b-form-spinbutton
                    id="off"
                    v-model="MinAds"
                    inline
                /> min(s)
            </b-col>
            <b-col>
                <h5 class="mt-5">
                    Troop training
                </h5>
                Fill troops above
                <br>
                <b-form-spinbutton
                    id="off"
                    v-model="TroopsTrain"
                    inline
                /> hours(s)
                <br>
                <div class="mt-2">
                    When below, continue fill for
                </div>
                <b-form-spinbutton
                    id="off"
                    v-model="TroopsTrainContinue"
                    inline
                /> hours(s)
            </b-col>
            <b-col>
                <h5 class="mt-5">
                    Ally donation
                </h5>
                When resource above
                <br>
                <b-form-spinbutton
                    id="off"
                    v-model="AllyAbove"
                    inline
                    min="1"
                    max="100"
                /> %
                <br>
                <div class="mt-2 mb-2">
                    leave
                </div>
                <b-form-spinbutton
                    id="off"
                    v-model="AllyLeft"
                    min="1"
                    max="100"
                    inline
                /> % <br>
                <div class="mt-2">
                    of resouce after donating
                </div>
            </b-col>
        </b-row>
        <b-row>
            <b-col>
                <h5 class="mt-5">
                    Chrome settings
                </h5>
                <b-form-checkbox-group
                    v-model="ChromeSetting"
                    style="text-align: left; padding-left: 10%;"
                    class="mt-1"
                >
                    <b-form-checkbox value="headless">
                        Headless mode
                    </b-form-checkbox>
                    <b-form-checkbox
                        value="image"
                        class="mt-1"
                    >
                        Not load images
                    </b-form-checkbox>
                    <b-form-checkbox
                        value="minimized"
                        class="mt-1"
                    >
                        Open minimized
                    </b-form-checkbox>
                    <b-form-checkbox
                        value="reopen"
                        class="mt-1"
                    >
                        Close and reopen <br>
                        if there is no task in next 5 mins
                    </b-form-checkbox>
                </b-form-checkbox-group>
            </b-col>
            <b-col>
                <h5 class="mt-5">
                    Auto tasks
                </h5>
                <b-form-checkbox-group
                    v-model="AutoTask"
                    style="text-align: left; padding-left: 20%;"
                    class="mt-1"
                >
                    <b-form-checkbox value="ReadIGMs">
                        Read IGMs
                    </b-form-checkbox>
                    <b-form-checkbox
                        value="RandomTask"
                        class="mt-1"
                    >
                        Random task
                    </b-form-checkbox>
                    <b-form-checkbox
                        value="ExtendProtection"
                        class="mt-1"
                    >
                        Extend Protection
                    </b-form-checkbox>
                </b-form-checkbox-group>
            </b-col>
            <b-col>
                <h5 class="mt-5">
                    Resources uses priority
                </h5>
                <draggable
                    :list="ResourcePriorityList"
                >
                    <div
                        v-for="element in ResourcePriorityList"

                        :key="element.name"
                        class="list-group-item"
                    >
                        {{ element.name }}
                    </div>
                </draggable>
            </b-col>
            <b-col>
                <h5 class="mt-5">
                    Ally donation priority
                </h5>
                <draggable
                    :list="DonationPriorityList"
                >
                    <div
                        v-for="element in DonationPriorityList"

                        :key="element.name"
                        class="list-group-item"
                    >
                        {{ element.name }}
                    </div>
                </draggable>
            </b-col>
        </b-row>
        <b-row>
            <b-col md="6">
                <h5 class="mt-5">
                    Bot activity settings <br>
                    (in mins)
                </h5>
                <b-container
                    fluid
                    class="mt-2"
                >
                    <b-row>
                        <b-col />
                        <b-col>
                            Min
                        </b-col>
                        <b-col>
                            Max
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            Working time
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                v-model="TimeWorkMin"
                                inline
                            />
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                v-model="TimeWorkMax"
                                inline
                            />
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            Sleep time
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                v-model="TimeSleepMin"
                                inline
                            />
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                v-model="TimeSleepMax"
                                inline
                            />
                        </b-col>
                    </b-row>
                </b-container>
            </b-col>

            <b-col />

            <b-col />
        </b-row>
    </b-container>
</template>

<script>
    import draggable from 'vuedraggable';

    export default {
        name: 'ViewGeneral',
        components: {
            draggable,
        },
        data () {
            return {
                DonationPriorityList: [
                    { name: 'Recruitment', id: 0 },
                    { name: 'Philosophy', id: 1 },
                    { name: 'Metallurgy', id: 2 },
                    { name: 'Commerce', id: 3 },
                ],
                ResourcePriorityList: [
                    { name: 'Celebrations', id: 0 },
                    { name: 'Building', id: 1 },
                    { name: 'Troops', id: 2 },
                ],
            };
        },
    };

</script>
