<template>
    <v-dialog :modelValue="isOpen" @update:modelValue="(value) => this.$emit('update:isOpen', value)" width="auto">
        <v-card>
            <v-card-title class="text-h6">
                {{ $t('label.qrCode') }}
            </v-card-title>

            <v-card-text class="d-flex justify-center align-center">
                <img v-if="imgSrc" :src="imgSrc" ref="previewImg" alt="Preview" style="max-width: 90%;" />
            </v-card-text>

            <v-card-actions>
                <v-btn @click="onPrint" variant="elevated" icon="mdi-printer" color="info" rounded />
                <v-btn @click="onDownloadBLX" variant="elevated" color="info" rounded text="lbx" icon="" />
                <v-spacer></v-spacer>
                <v-btn @click="close" variant="elevated" v-t="'label.ok'" color="success" rounded icon="" />
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import * as JSZipLib from "@/libs/JSZip.js";
import BLX_QR_TagXml from "@/resources/BLX_QR_Tag.txt?raw";
import BLX_PROPXml from "@/resources/BLX_PROP.txt?raw";
import formatter from '@/helps/formatter';

export default {
    props: [
        "str",
        "isOpen",

        "title",
        "subTitle",
        "subSubTitle",
        "additionalCode"
    ],
    emits: ['update:isOpen'],
    data() {
        return {
            isBusy: false,
            imgSrc: undefined,
            svgSrc: undefined,

            fmt: formatter
        };
    },
    methods: {
        onLoadQRCodes() {
            if (this.isBusy) return;

            this.isBusy = true;

            this.$axios
                .get(this.$api.qrCodes.replace("{str}", this.str), { responseType: 'blob' })
                .then((response) => {
                    this.isBusy = false;
                    this.svgSrc = response.data;
                    this.fmt.blobToBase64(response.data).then(x => {
                        this.imgSrc = x;
                    }).finally(() => { });
                }).catch((error) => {
                    this.isBusy = false;
                });
        },
        close() {
            this.$emit('update:isOpen', false);
        },
        onPrint() {
            this.$MyApp.printBase64(this.imgSrc);
        },
        async onDownloadBLX() {
            let zip = new JSZip();

            let xml = BLX_QR_TagXml
                .replace("{{Title}}", this.title ?? '')
                .replace("{{SubTitle}}", this.subTitle ?? '')
                .replace("{{SubSubTitle}}", this.subSubTitle ?? '')
                .replace("{{AdditionalCode}}", this.additionalCode ?? '');

            zip.file("label.xml", xml);
            zip.file("prop.xml", BLX_PROPXml);

            const imageData = await this.svgStringToImageData(this.svgSrc, 231.3, 231.3);
            const bmpBlob = this.imageDataToBmpBlob(imageData);
            zip.file("Object0.bmp", bmpBlob);

            const blob = await zip.generateAsync({ type: "blob" });

            const link = document.createElement("a");
            link.href = URL.createObjectURL(blob);
            link.download = "QRCode.lbx";
            link.click();

            URL.revokeObjectURL(link.href);
        },
        imageDataToBmpBlob(imageData) {
            const width = imageData.width;
            const height = imageData.height;
            const rgba = imageData.data;

            const bytesPerPixel = 3;
            const rowPadding = (4 - ((width * bytesPerPixel) % 4)) % 4;
            const rowSize = width * bytesPerPixel + rowPadding;
            const pixelDataSize = rowSize * height;

            const fileHeaderSize = 14;
            const infoHeaderSize = 40;
            const offset = fileHeaderSize + infoHeaderSize;
            const fileSize = offset + pixelDataSize;

            const buffer = new ArrayBuffer(fileSize);
            const view = new DataView(buffer);
            let pos = 0;

            view.setUint8(pos++, 0x42);
            view.setUint8(pos++, 0x4D);
            view.setUint32(pos, fileSize, true); pos += 4;
            view.setUint16(pos, 0, true); pos += 2;
            view.setUint16(pos, 0, true); pos += 2;
            view.setUint32(pos, offset, true); pos += 4;

            view.setUint32(pos, infoHeaderSize, true); pos += 4;
            view.setInt32(pos, width, true); pos += 4;
            view.setInt32(pos, height, true); pos += 4;
            view.setUint16(pos, 1, true); pos += 2;
            view.setUint16(pos, bytesPerPixel * 8, true); pos += 2;
            view.setUint32(pos, 0, true); pos += 4;
            view.setUint32(pos, pixelDataSize, true); pos += 4;
            view.setInt32(pos, 0, true); pos += 4;
            view.setInt32(pos, 0, true); pos += 4;
            view.setUint32(pos, 0, true); pos += 4;
            view.setUint32(pos, 0, true); pos += 4;

            pos = offset;
            const out = new Uint8Array(buffer);

            for (let row = height - 1; row >= 0; row--) {
                let rowStart = row * width * 4;
                for (let x = 0; x < width; x++) {
                    const i = rowStart + x * 4;
                    const r = rgba[i];
                    const g = rgba[i + 1];
                    const b = rgba[i + 2];
                    out[pos++] = b;
                    out[pos++] = g;
                    out[pos++] = r;
                }
                for (let p = 0; p < rowPadding; p++) {
                    out[pos++] = 0;
                }
            }

            return new Blob([buffer], { type: "image/bmp" });
        },

        async svgStringToImageData(svgString, width, height) {
            const svgBlob = new Blob([svgString], { type: "image/svg+xml;charset=utf-8" });
            const url = URL.createObjectURL(svgBlob);

            try {
                const img = new Image();

                const imgLoad = new Promise((resolve, reject) => {
                    img.onload = () => resolve();
                    img.onerror = (e) => reject(new Error("Falha ao carregar o SVG como imagem: " + e.message));
                });

                img.src = url;
                await imgLoad;

                const canvas = document.createElement("canvas");
                canvas.width = width;
                canvas.height = height;
                const ctx = canvas.getContext("2d");
                ctx.clearRect(0, 0, width, height);
                ctx.drawImage(img, 0, 0, width, height);

                const imageData = ctx.getImageData(0, 0, width, height);

                URL.revokeObjectURL(url);
                return imageData;
            } catch (err) {
                URL.revokeObjectURL(url);
                throw err;
            }
        },
    },
    updated() {
        if (this.isOpen) this.onLoadQRCodes();
    }
};
</script>