<template>
    <v-card class="tile" @click="navTo">
        <div class="tile-header">
            <h3 class="tile-title"> {{ title }}</h3>
            <v-icon :icon="icon" />
        </div>
        <div class="tile-content">
            <p> {{ message }}</p>
        </div>
    </v-card>
</template>

<script>
import { defineComponent } from 'vue'

export default defineComponent({
    props: ["title", "icon", "message", "route"],
    methods: {
        navTo() {
            if (this.route)
                this.$router.push({ name: this.route });
        }
    }
});
</script>

<style lang="css" scoped>
.tile {
    cursor: pointer;
    margin: 1rem;
}

.tile:hover {
    transform: scale(1.05);
}

.tile-header {
    display: flex;
    align-items: top;
    margin: 1rem;
}

.tile-title {
    display: inline;
    margin-right: 1rem;
}

.tile-content {
    margin: 1rem;
}

@media screen and (max-width: 1023px) {
    .tile {
        width: 100%;
        height: 13rem;
    }
}

@media screen and (min-width: 1024px) {
    .tile {
        width: 13rem;
        height: 13rem;
    }
}
</style>