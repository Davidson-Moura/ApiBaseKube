<template>
    <v-dialog :modelValue="isOpen" @update:modelValue="(value) => this.$emit('update:isOpen', value)" max-width="80%">
        <v-card>
            <v-card-title class="text-h6">
                {{ $t('label.changeLog') }}
            </v-card-title>

            <v-card-text>
                <v-timeline align="start">

                    <v-timeline-item v-for="(log, i) in diffs" :key="i">
                        <div>
                            <div class="text-h6">{{ log.UserName }} - {{ fmt.formatFullDate(log.ChangeDate) }}</div>

                            <p v-for="(change, i) in log.Changes" :key="i">
                                
                                <v-expansion-panels v-if="change.OldValue && typeof change.OldValue === 'object'">
                                    <v-expansion-panel>
                                        <v-expansion-panel-title> 
                                            {{ $t("log." + change.Prop) }} 
                                            <v-chip
                                                class="ml-2"
                                                color="info"
                                                label
                                            >
                                                {{ $t("log." + change.Type) }}
                                            </v-chip>
                                        </v-expansion-panel-title>
                                        <v-expansion-panel-text>
                                            <p v-for="(subChange, j) in change.Changes" :key="j">
                                                {{ $t("log." + subChange.Prop) }}
                                                
                                                <v-chip label > {{ formatValue(subChange.OldValue) }} </v-chip>
                                                <v-chip
                                                class="ma-2"
                                                color="info"
                                                label
                                                >
                                                {{ $t("log." + subChange.Type) }}
                                                <v-icon icon="mdi-arrow-right-bold" end />
                                            </v-chip>
                                            
                                            <v-chip label> {{ formatValue(subChange.NewValue) }} </v-chip>
                                            <!--
                                            <div v-if="subChange.Type == types.Modified">
                                                </div>
                                                <div v-else >
                                                    <p v-for="(key, k) in Object.keys(subChange.Type == types.Removed ? subChange.OldValue : subChange.NewValue)" :key="k">
                                                        <strong>{{ key }}</strong> {{ obj[key] }}
                                                    </p>
                                                </div>
                                                -->
                                                
                                            </p>
                                        </v-expansion-panel-text>
                                    </v-expansion-panel>
                                </v-expansion-panels>

                                <div v-else>
                                    {{ $t("log." + change.Prop) }}

                                    <v-chip label > {{ formatValue(change.OldValue) }} </v-chip>
                                    <v-chip
                                        class="ma-2"
                                        color="info"
                                        label
                                    >
                                        {{ $t("log." + change.Type) }}
                                        <v-icon icon="mdi-arrow-right-bold" end />
                                    </v-chip>

                                    <v-chip label> {{ formatValue(change.NewValue) }} </v-chip>
                                </div>
                                
                            </p>
                        </div>
                    </v-timeline-item>
                </v-timeline>
                <v-data-table-server :items="[]" :headers="[]" :items-length="totalEntities" @update:options="loadEntities"
                    v-model:items-per-page="take" hide-default-header hide-default-body hide-no-data
                    v-model:page="page">
                    <template v-slot:tbody> </template>
                    <template v-slot:no-data> </template>
                </v-data-table-server>
            </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn @click="close" variant="elevated" v-t="'label.cancel'" color="error" />
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import formatter from '@/helps/formatter';

export default {
    props: [
        "objectTypeEnum",
        "objectId",
        "isOpen"
    ],
    emits: ['update:isOpen'],
    data() {
        return {
            types: {
                Add: 'Add',
                Modified: 'Modified',
                Removed: 'Removed'
            },
            entities: [],
            totalEntities: 0,
            diffs: [],

            page: 1,
            take: 10,
            search: '',

            isBusy: false,
            fmt: formatter
        };
    },
    methods: {
        loadEntities({ page, itemsPerPage, sortBy }) {
            if (this.isBusy == true) return;
            this.isBusy = true;

            let url = this.$api.log.replace("{objType}", this.objectTypeEnum);

            this.take = itemsPerPage <= 0 ? 2147483647 : itemsPerPage;

            url += `?page=${page}`;
            url += `&take=${this.take + 1}`;
            url += this.search ? `&search=${this.search}` : "";

            this.$axios.get(url).then(response => {
                this.entities = response.data.List;
                this.totalEntities = response.data.Count;

                let current = null;
                this.diffs = [];
                this.entities.forEach(prev => {
                    if (current) this.diffs.push({
                        Changes: this.getDiffObjects(prev.Entity, current.Entity, "Changes"),
                        UserName: prev.UserName,
                        ChangeDate: prev.DateCreate
                    });
                    current = prev;
                });

                console.log(this.diffs);
            }).catch((error) => { }).finally(() => this.isBusy = false);
        },
        close() {
            this.$emit('update:isOpen', false);
        },
        getDiffObjects(a, b, key) {
            var diffs = [];
            if (typeof a == 'object' && a && b) {
                if (Array.isArray(a)) {
                    var modifies = this.diffLinesUnordered(a, b, key);
                    modifies.forEach(modify => {
                        modify.Changes = this.getDiffObjects(modify.OldValue, modify.NewValue, key);
                    });
                    diffs = diffs.concat(modifies);
                } else {
                    Object.keys(a).forEach(objKey => {
                        diffs = diffs.concat(this.getDiffObjects(a[objKey], b[objKey], objKey));
                    });
                }
            } else {
                if (a === null && b) this.addDiffCompatedNullAdd(b, key, diffs); 
                else if (b === null && a) this.addDiffCompatedNullRemove(a, key, diffs);
                else if (a != b) diffs.push({ Prop: key, OldValue: a, NewValue: b, Type: "Modified" });
            }
            return diffs;
        },
        addDiffCompatedNullAdd(a, key, diffs){
            if (typeof a == 'string' || typeof a == 'number') diffs.push({ Prop: key, OldValue: null, NewValue: a, Type: this.types.Add });
            else Object.keys(a).forEach(kA => { diffs.push({ Prop: kA, OldValue: null, NewValue: a[kA], Type: this.types.Add }); });
        },
        addDiffCompatedNullRemove(a, key, diffs){
            if (typeof a == 'string' || typeof a == 'number') diffs.push({ Prop: key, OldValue: a, NewValue: null, Type: this.types.Removed });
            else Object.keys(a).forEach(kA => { diffs.push({ Prop: kA, OldValue: a[kA], NewValue: null, Type: this.types.Removed }); });
        },
        isObjectsDifferent(a, b) {
            return JSON.stringify(a) !== JSON.stringify(b);
        },
        diffLinesUnordered(oldLines, newLines, propName) {
            const used = new Set();
            const changes = [];

            for (let i = 0; i < oldLines.length; i++) {
                const oldLine = oldLines[i];
                let bestMatchIndex = -1;
                let bestScore = -1;

                for (let j = 0; j < newLines.length; j++) {
                    if (used.has(j)) continue;

                    const score = this.lineSimilarity(oldLine, newLines[j]);

                    if (score > bestScore) {
                        bestScore = score;
                        bestMatchIndex = j;
                    }
                }

                if (bestMatchIndex === -1) {
                    changes.push({
                        Type: "Removed",
                        Similarity: bestScore,
                        Prop: propName,
                        OldValue: oldLine, NewValue: null
                    });
                    continue;
                }

                used.add(bestMatchIndex);

                const newLine = newLines[bestMatchIndex];

                if (!this.deepEqual(oldLine, newLine)) {
                    changes.push({
                        Type: "Modified",
                        Similarity: bestScore,
                        Prop: propName,
                        OldValue: oldLine, NewValue: newLine
                    });
                }
            }

            // linhas adicionadas que não têm correspondência
            for (let i = 0; i < newLines.length; i++) {
                if (!used.has(i)) {
                    changes.push({
                        Type: "Added",
                        Similarity: bestScore,
                        Prop: propName,
                        OldValue: null, NewValue: newLines[i]
                    });
                }
            }

            return changes;
        },
        lineSimilarity(a, b) {
            const keys = new Set([...Object.keys(a), ...Object.keys(b)]);
            let total = 0;
            let equalCount = 0;

            for (const key of keys) {
                total++;
                if (this.deepEqual(a[key], b[key])) {
                    equalCount++;
                }
            }

            return equalCount / total; // número entre 0 e 1
        },
        deepEqual(a, b) {
            if (a === b) return true;

            if (typeof a !== "object" || typeof b !== "object" || a === null || b === null)
                return a === b;

            const keysA = Object.keys(a);
            const keysB = Object.keys(b);

            if (keysA.length !== keysB.length) return false;

            for (const key of keysA) {
                if (!keysB.includes(key)) return false;
                if (!this.deepEqual(a[key], b[key])) return false;
            }

            return true;
        },
        formatValue(v){
            if (v === true || v === "true") return this.$t('label.yes');
            if (v === false || v === "false") return this.$t('label.no');
            if (!isNaN(Number(v))) return v;
            if (this.fmt.isValidDateString(v)) return this.fmt.formatFullDate(v);

            return v;
        }
    },
    mounted() {
        if (this.isOpen){
            this.loadEntities({ page: 1, itemsPerPage: 10, sortBy: null });
        }
    }
};
</script>