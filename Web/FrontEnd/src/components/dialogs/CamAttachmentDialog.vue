<template>
    <v-dialog width="auto" persistent :modelValue="isOpen" @update:modelValue="(value) => this.$emit('update:isOpen', value)" >
            <v-card class="mt-6" v-if="foto" >
                <template v-slot:prepend>
                    <v-card-title>{{ $t('label.cam') }}</v-card-title>
                </template>
                <template v-slot:append>
                    <v-btn variant="elevated" size="small" @click="onCloseCamDialog" icon="" 
                        color="error" rounded>
                        <v-icon icon="mdi-close" />
                        <v-tooltip activator="parent" location="top">{{ $t('label.onClose') }}</v-tooltip>
                    </v-btn>
                </template>
                
                <v-img :src="foto" cover width="auto" height="auto"/>

                <v-card-actions class="mt-4">
                    <v-menu >
                        <template v-slot:activator="{ props }">
                            <v-btn
                                color="info"
                                variant="elevated"
                                v-bind="props"
                            >
                                {{ $t('label.save') }}
                            </v-btn>
                        </template>
                        <v-list density="compact" activatable>
                            <v-list-item @click="onDialogSaveToAttachment" nav>
                                <v-list-item-title>{{ $t('label.saveToAttachment') }}</v-list-item-title>
                            </v-list-item>
                        </v-list>
                    </v-menu>
                    
                </v-card-actions>
            </v-card>
            <v-card class="pa-4" v-else >
                <template v-slot:prepend>
                    <v-card-title>{{ $t('label.cam') }}</v-card-title>
                </template>
                <template v-slot:append>
                    <v-btn variant="elevated" size="small" @click="onCloseCamDialog" icon="" 
                        color="error" rounded>
                        <v-icon icon="mdi-close" />
                        <v-tooltip activator="parent" location="top">{{ $t('label.onClose') }}</v-tooltip>
                    </v-btn>
                </template>
    
                <v-select
                    v-model="selectedDeviceId"
                    :items="videoDevices"
                    item-title="label"
                    item-value="deviceId"
                    :label="$t('label.selectTheCamera')"
                    class="mb-4"
                    @update:modelValue="initCam"
                />
                
                <video ref="video" autoplay playsinline width="100%" height="80%" />
    
                <v-card-actions class="mt-4">
                    <v-btn color="info" variant="elevated" @click="onPhoto"> {{ $t('label.onPhoto') }}</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
</template>

<script>

export default {
    name: "CamAttachmentDialog",

    props: [
        "isOpen",
        "onSaveToAttachment"
    ],
    emits: ['update:isOpen'],
    components: {},

    data() {
        return {
            foto: null,
            stream: null,
            videoDevices: [],
            selectedDeviceId: null,
        };
    },
    computed: {

    },
    methods: {
        async listDevices(){
            const devices = await navigator.mediaDevices.enumerateDevices();
            this.videoDevices = devices.filter(d => d.kind === 'videoinput');

            if (this.videoDevices.length > 0) {
                this.selectedDeviceId = this.videoDevices[0].deviceId;
            }
        },
        async onCam() {
            await this.listDevices();
            await this.initCam();
        },
        async initCam(){
            try {
                if (this.stream) {
                    this.stream.getTracks().forEach(track => track.stop());
                }

                this.stream = await navigator.mediaDevices.getUserMedia({
                    video: { deviceId: this.selectedDeviceId ? { exact: this.selectedDeviceId } : undefined }
                })
                this.$refs.video.srcObject = this.stream;

                if (!this.videoDevices || this.videoDevices.length <= 0 || this.videoDevices[0].deviceId == "") this.listDevices();
            } catch (err) {
                alert('Erro ao acessar a câmera: ' + err.message)
            }
        },
        onPhoto() {
            let video = this.$refs.video;
            if (!video) return

            const canvas = document.createElement('canvas')
            canvas.width = video.videoWidth
            canvas.height = video.videoHeight

            const context = canvas.getContext('2d')
            context.drawImage(video, 0, 0, canvas.width, canvas.height)

            this.foto = canvas.toDataURL('image/png');
        },
        onCloseCamDialog(){
            this.destroyCam();
            this.$emit('update:isOpen', false);
        },
        destroyCam(){
            if (this.stream) {
                this.stream?.getTracks()?.forEach(track => track.stop());
                this.stream = 
                this.foto = undefined;
            }
        },
        onDialogSaveToAttachment(){
            if (this.onSaveToAttachment)
                this.onSaveToAttachment(this.foto);
            this.$emit('update:isOpen', false);
        }
    },
    mounted() {
        if(this.isOpen){
            this.onCam();
        }
    }
};
</script>

<style>
.v-data-table__tr--clickable:hover {
    background-color: rgba(var(--v-theme-success)) !important; 
}
</style>

