<template>
    <div class="margin">
        <h3 class="test">{{title}}</h3>

        <p>This is a test!</p>

        <custom1 class="cyan">testing</custom1>
    </div>
</template>

<script type="text/babel">
    import custom1 from './custom1.vue';

export default {
    components: { custom1 },

    data() {
        return {
            title: 'Hello world!',
            test: 'batman'
        }
    },

    methods: {

    }
}
</script>

<style lang="sass">
    @import '../css/main.scss';

    .margin {
        background: #8f8;
        display: block;
        margin: 10px;
        padding: 10px;
    }

    .red, .cyan {
        margin: 5px;
        padding: 10px;
        border-radius: 8px;
        display: inline-block;
        color: #fff;
    }
    .red {
        background: red;
    }

    .cyan {
        background: purple;
    }
</style>
