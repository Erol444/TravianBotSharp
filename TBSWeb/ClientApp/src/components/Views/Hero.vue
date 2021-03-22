<template>
    <b-container fluid>
        <b-row>
            <b-col>
                <!-- Hero info -->
                <h5 class="mt-2">
                    Hero info
                </h5>
                <b-form-textarea
                    id="textarea"
                    v-model="Info"
                    rows="10"
                    max-rows="10"
                    readonly
                />
                <b-button class="mt-2">
                    Refersh
                </b-button>
            </b-col>
            <b-col>
                <!-- Hero items -->
                <h5 class="mt-2">
                    Hero items
                </h5>
                <b-table-lite
                    bordered
                    hover
                    fixed
                    responsive
                    sticky-header
                    :items="Items"
                />
            </b-col>
            <b-col>
                <h5 class="mt-2">
                    Auto
                </h5>

                <b-form-group>
                    <b-form-checkbox-group
                        id="CheckboxAuto"
                        v-model="AutoSelected"
                        name="CheckboxAuto"
                        stacked
                        switches
                        style="text-align: left; padding-left: 10%;"
                    >
                        <b-form-checkbox value="adventures">
                            Send to adventures
                        </b-form-checkbox>
                        <b-form-checkbox value="revive">
                            Revive hero
                        </b-form-checkbox>
                        <b-form-checkbox value="info">
                            Refersh hero info
                        </b-form-checkbox>
                        <b-form-checkbox value="items">
                            Equip items
                        </b-form-checkbox>
                        <b-form-checkbox value="helmets">
                            Equip helmets
                        </b-form-checkbox>
                        <b-form-checkbox value="points">
                            Set hero point
                        </b-form-checkbox>
                    </b-form-checkbox-group>
                </b-form-group>
            </b-col>
            <b-col>
                <h5 class="mt-2">
                    Auto hero point
                </h5>
                <b-container>
                    <b-row>
                        <b-col>
                            <label>Strength</label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="strength"
                                v-model="StrengthPoint"
                                min="1"
                                max="100"
                                inline
                            />
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            <label>Off bouns</label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="off"
                                v-model="OffPoint"
                                min="1"
                                max="100"
                                inline
                            />
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            <label>Def bouns</label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="def"
                                v-model="DefPoint"
                                min="1"
                                max="100"
                                inline
                            />
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            <label>Resources</label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="res"
                                v-model="ResPoint"
                                min="1"
                                max="100"
                                inline
                            />
                        </b-col>
                    </b-row>
                </b-container>
            </b-col>
            <b-col>
                <h5 class="mt-2">
                    Auto adventures
                </h5>
                <b-container>
                    <b-row>
                        <b-col>
                            <label>Minimum health</label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="health"
                                v-model="HealthMin"
                                min="15"
                                max="100"
                                inline
                            />
                        </b-col>
                    </b-row>
                    <b-row>
                        <b-col>
                            <label>Max distance</label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="off"
                                v-model="MaxDistance"
                                min="1"
                                inline
                            />
                        </b-col>
                    </b-row>
                </b-container>
            </b-col>
        </b-row>
        <b-row>
            <b-col>
                <!-- Hero adventures -->
                <h5 class="mt-2">
                    Hero adventures
                </h5>
                <b-table-lite
                    bordered
                    hover
                    fixed
                    responsive
                    sticky-header
                    :items="Adventures"
                />
                <b-button class="mt-2">
                    Refersh
                </b-button>
            </b-col>

            <b-col>
                <!-- Hero equipment -->
                <h5 class="mt-2">
                    Hero equipment
                </h5>
                <b-table-lite
                    bordered
                    hover
                    fixed
                    responsive
                    sticky-header
                    :items="Equipment"
                />
            </b-col>
            <b-col>
                <h5 class="mt-2">
                    Update hero frequency
                </h5>
                <b-container>
                    <b-row>
                        <b-col>
                            <label>Min</label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="health"
                                v-model="MinUpdate"
                                min="1"
                                inline
                            />
                        </b-col>
                        <b-col>
                            min(s)
                        </b-col>
                    </b-row>
                    <b-row class="mt-4">
                        <b-col>
                            <label>Max </label>
                        </b-col>
                        <b-col>
                            <b-form-spinbutton
                                id="off"
                                v-model="MaxUpdate"
                                min="1"
                                inline
                            />
                        </b-col>
                        <b-col>
                            min(s)
                        </b-col>
                    </b-row>
                </b-container>
            </b-col>
            <b-col>
                <h5 class="mt-2">
                    Auto revive
                </h5>
                <b-dropdown
                    id="revive"
                    variant="primary"
                    text="Choose village"
                >
                    <b-dropdown-item>VinaTown</b-dropdown-item>
                    <b-dropdown-item>VinaVillage</b-dropdown-item>
                </b-dropdown>
            </b-col>
            <b-col />
        </b-row>
    </b-container>
</template>

<script>
    export default {
        name: 'ViewHero',
    };
</script>
