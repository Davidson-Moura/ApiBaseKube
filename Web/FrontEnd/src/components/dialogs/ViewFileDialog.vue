<template>
    <v-dialog :modelValue="isOpen" @update:modelValue="(value) => this.$emit('update:isOpen', value)" width="auto">
        <v-card flat>
            <v-card-text>
                <img v-if="imgSrc" :src="imgSrc" ref="previewImg" alt="Preview" />
                <embed v-else :src="pdfSrc" ref="pdfViewer" type="application/pdf" style="min-height: 70vh; min-width: 70vw;" />
            </v-card-text>
        </v-card>
    </v-dialog>
</template>

<script>
import formatter from '@/helps/formatter';

export default {
    name: "ViewFileDialog",

    props: [
        "fileUrl", "extension",
        "isOpen"
    ],
    emits: ['update:isOpen'],
    components: {},

    data() {
        return {
            imgSrc: undefined,
            pdfSrc: undefined,

            fmt: formatter
        };
    },
    computed: {

    },
    methods: {
        loadFile() {
            this.imgSrc = undefined;
            this.pdfSrc = undefined;
            const isPDF = this.extension.endsWith(".pdf");

            this.$axios.get(this.fileUrl, { responseType: 'blob' })
                .then((response) => {
                    this.fmt.blobToBase64(response.data).then(x=> {
                        if (isPDF) this.pdfSrc = x;
                        else this.imgSrc = x;
                    }).finally(()=>{
                        this.$MyApp.setLoading(false);
                    });
                }).catch((error) => {
                    this.$MyApp.setLoading(false);
                });
        },
    },
    mounted() {
        if(this.isOpen){
            this.loadFile();
        }
    }
};
</script>

<style>
.v-data-table__tr--clickable:hover {
    background-color: rgba(var(--v-theme-success)) !important; 
}
</style>