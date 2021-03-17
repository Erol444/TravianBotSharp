<template>
    <h1>Counter</h1>

    <p>This is a simple example of an Vue component.</p>

    <p aria-live="polite">Current count: <strong>{{ currentCount }}</strong></p>

    <button class="btn btn-primary" @click="incrementCounter">Increment</button>
</template>


<script>
    export default {
        name: "Counter",
        data() {
            return {
                currentCount: 0
            }
        },
        methods: {
            incrementCounter() {
                this.currentCount++;
            }
        }
    }
</script>