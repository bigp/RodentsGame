<template>
    <div>
        {{test}}
        <i class="red">What is this?</i>
        <slot></slot>
    </div>
</template>

<script type="text/babel">
    export default {
        data() {
            return {
                test: 'batman'
            }
        }
    }
</script>