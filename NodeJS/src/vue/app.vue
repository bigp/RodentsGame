<template>
    <div class="margin">
        <h3 class="test">{{title}}</h3>

        <div v-io.world-bounds="onWorldBounds">

        </div>

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
        onWorldBounds(data) {
            trace("RECEIVED WORLD BOUNDS!!!");
            trace(data);
        }
    },

    created() {
        BIGP.io.emit('web-client');
    }
}
    //BIGP.io.on('world-bounds', JSON)
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
