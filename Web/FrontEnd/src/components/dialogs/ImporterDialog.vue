<template>
    <v-dialog 
        :modelValue="isOpen" 
        @update:modelValue="(value) => this.$emit('update:isOpen', value)"
        max-width="600px">
        <v-card>
            <v-card-title class="text-h6">
                {{ $t('label.spreadsheetImport') }}
            </v-card-title>

            <v-card-text>
                <p>
                    {{ $t('label.toImportSelectAFileInTheFormat') }}
                    <strong>.xlsx</strong> {{ $t('label.or') }} <strong>.csv</strong>.
                </p>
                <v-file-input v-model="file" label="Selecione o arquivo" accept=".xlsx, .csv"
                    prepend-icon="mdi-file-excel"></v-file-input>

                <v-alert type="info" class="mt-4" prominent>
                    <p class="font-weight-bold">{{$t('label.expectedSpreadsheetStructure')}}:</p>
                    <ul>
                        <li v-for="column in columns">{{ $t('label.column') }} <strong>{{ column.Key }}</strong> → {{ column.Name }}</li>
                    </ul>
                    <p> {{ $t('label.theFirstLineShouldContainTheHeadersExactlyAsAbove') }} </p>
                </v-alert>
            </v-card-text>

            <v-card-actions>
                <v-btn v-t="'label.downloadSample'" @click="onGenerateSample" color="success" variant="elevated" />
                <v-spacer></v-spacer>
                <v-btn @click="close" variant="elevated" v-t="'label.cancel'" color="error" />
                <v-btn color="success" :disabled="!file" @click="onImport" variant="elevated" v-t="'label.toImport'" />
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import ExcelJS from "exceljs";
import { saveAs } from "file-saver";

export default {
    props: [
        "columns", "objectTypeEnum",
        "isOpen"
    ],
    emits: ['update:isOpen'],
    data() {
        return {
            file: null,

            isBusy: false
        };
    },
    methods: {
        onImport() {
            if (this.isBusy) return;
            
            if (!this.file) return;

            this.isBusy = true;
            this.$MyApp.setLoading(true);
            
            let form = new FormData();
            form.append("File", this.file);

            let header = { headers: { 'Content-Type': 'multipart/form-data' } };

            this.$axios
            .post(this.$api.importerFile.replace("{objType}", this.objectTypeEnum), form, header)
            .then((response) => {
                this.isBusy = false;
                this.$MyApp.setLoading(false);

                this.$MyApp.success(this.$t('message.UploadCompleted'));
                this.close();
            }).catch((error) => {
                this.isBusy = false;
                this.$MyApp.setLoading(false);
            });
        },
        close() {
            this.$emit('update:isOpen', false);
        },

        async onGenerateSample() {
            const workbook = new ExcelJS.Workbook();
            const worksheet = workbook.addWorksheet("Planilha");

            worksheet.columns = this.columns.map(c=> {
                return { header: c.Key, key: c.Key, width: (c.Name.length * 1.7) / (c.Name.length * 0.09) }
            });

            let items = [{}];
            items.forEach(item => worksheet.addRow(item));

            worksheet.getRow(1).eachCell(cell => {
                cell.font = { bold: true, color: { argb: "FFFFFFFF" } }
                cell.fill = {
                    type: "pattern",
                    pattern: "solid",
                    fgColor: { argb: "FF228B22" }
                }
                cell.border = {
                    top: { style: "thin" },
                    left: { style: "thin" },
                    bottom: { style: "thin" },
                    right: { style: "thin" }
                }
            });

            const buffer = await workbook.xlsx.writeBuffer();
            saveAs(new Blob([buffer]), "planilha.xlsx");
        }
    },
};
</script>