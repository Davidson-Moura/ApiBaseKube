import i18n from '@/plugins/i18nBase';

const fmt = {
    hexToRgba(hex) {
        if (!hex.replace) return 0;
        hex = hex.replace("#", "");

        const red = parseInt(hex.substr(0, 2), 16);
        const green = parseInt(hex.substr(2, 2), 16);
        const blue = parseInt(hex.substr(4, 2), 16);
        const alpha = hex.lenght > 6 ? parseInt(hex.substr(6, 2), 16) / 255 : 1;

        let argb = {
            red,
            green,
            blue,
            alpha
        }

        return argb;
    },
    rgbaToHex(rgba) {
        if (!rgba) return 0;

        var red = 0;
        var green = 0;
        var blue = 0;
        var alpha = 0;
        if (rgba.r || rgba.r == 0) {
            red = parseInt(rgba.r);//(argb >> 16) & 0xFF;
            green = parseInt(rgba.g);//(argb >> 8) & 0xFF;
            blue = parseInt(rgba.b);//argb & 0xFF;
            alpha = Math.round(parseFloat(rgba.a) * 255);//(argb >> 24) & 0xFF;
        } else {
            let list = rgba.split(',');
            red = parseInt(list[0]),
                green = parseInt(list[1]),
                blue = parseInt(list[2]),
                alpha = Math.round(parseFloat(list[3]) * 255);
        }

        var hex = "#" + red.toString(16).padStart(2, '0') + green.toString(16).padStart(2, '0') + blue.toString(16).padStart(2, '0') + alpha.toString(16).padStart(2, '0');

        return hex;
    },
    rgbaToString(rgba) {
        if(!rgba) return "";
        var r = `${rgba.r},${rgba.g},${rgba.b},${rgba.a}`;
        return r;
    },
    rgbaStringToRGBA(rgba) {
        if (!rgba) return;
        let list = rgba.split(',');
        let obj = {
            r: parseInt(list[0]),
            g: parseInt(list[1]),
            b: parseInt(list[2]),
            a: list.length > 3 ? parseInt(list[3]) : 1
        }
        return obj;
    },
    formatDate(date) {
        if (!date || date == "-") return "-";
        else if (typeof date == 'string') date = new Date(date)
        const opcoesDeFormatacao = {
            year: 'numeric',
            month: 'numeric',
            day: 'numeric'
        };
        if(date.getYear() <= -1900) return "";

        return date.toLocaleDateString(undefined, opcoesDeFormatacao);
    },

    formatFullDate(date) {
        if (!date) return "";
        else if (typeof date == 'string') date = new Date(date)
        const opcoesDeFormatacao = {
            year: 'numeric',
            month: 'numeric',
            day: 'numeric'
        };
        if(date.getYear() <= -1900) return "";

        const opcoesDeHora = {
            hour: '2-digit',
            minute: '2-digit'
        };

        const dataFormatada = date.toLocaleDateString(undefined, opcoesDeFormatacao);
        const horaFormatada = date.toLocaleTimeString(undefined, opcoesDeHora);

        return `${dataFormatada} ${horaFormatada}`;;
    },
    formatDateEn(date) {
        if (!date) return "";
        if (typeof date === "string") date = new Date(date);

        let year = (date.getFullYear()+"").padStart(4, "0");
        if(year == '0000') year = '0001';
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");

        return `${year}-${month}-${day}`;
    },
    formatInputDate(v) {
        if (!v) return null;

        return this.limitString(v, 10);//2024-03-30T17:26:15
    },
    isValidDateString(value) {
        const date = new Date(value);
        return !isNaN(date.getTime());
    },
    formatPrice(v){
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }).format(v);
    },
    limitString(str, limit) {
        if (!str) return "";
        if (str.length > limit)
            return str.substr(0, limit);
        return str;
    },
    totalPages(total, take) {
        if (!total) total = 1
        if (!take) take = 10
        let pages = 1;

        pages = Math.ceil(total / take);
        if (!pages) pages = 1;

        return pages;
    },
    queryRoute(queries) {
        if (!queries || queries.length <= 0) return "";

        return "?" + (
            queries.length == 1 ?
                queries[0]
                : queries.reduce((p, n) => `${p}&${n}`)
        );
    },
    licenseTypeEnumModel(){
        return [
            { ID: 0, Name: i18n.global.t('label.trial')  },
            { ID: 1, Name: i18n.global.t('label.production')  },
        ];
    },
    licenseTypeName(e){
        switch (e) {
            case 0: return i18n.global.t('label.trial');
            case 1: return i18n.global.t('label.production');
            default: return "";
        }
    },
    itemFeatureTypeEnumModel: [
        { ID: 1, Name: i18n.global.t('label.string')  },
        { ID: 2, Name: i18n.global.t('label.number')  },
        { ID: 3, Name: i18n.global.t('label.boolean')  },
        { ID: 4, Name: i18n.global.t('label.validValues')  },
    ],
    itemFeatureTypeName(e){
        switch (e) {
            case 1: return i18n.global.t('label.string');
            case 2: return i18n.global.t('label.number');
            case 3: return i18n.global.t('label.boolean');
            case 4: return i18n.global.t('label.validValues');
            default: return "";
        }
    },
    itemStatusEnumModel: [
        { ID: 1, Name: i18n.global.t('label.available')  },
        { ID: 2, Name: i18n.global.t('label.allocated')  },
        { ID: 3, Name: i18n.global.t('label.maintenance')  },
        { ID: 4, Name: i18n.global.t('label.unavailable')  },
        
    ],
    itemStatusName(e){
        switch (e) {
            case 1: return i18n.global.t('label.available');
            case 2: return i18n.global.t('label.allocated');
            case 3: return i18n.global.t('label.maintenance');
            case 4: return i18n.global.t('label.unavailable');
            default: return i18n.global.t('label.uninformed');
        }
    },

    partnerTypeEnumModel: [
        { ID: 1, Name: i18n.global.t('label.customer')  },
        { ID: 2, Name: i18n.global.t('label.supplier')  },
    ],
    partnerTypeName(e){
        switch (e) {
            case 1: return i18n.global.t('label.customer');
            case 2: return i18n.global.t('label.supplier');
            default: return i18n.global.t('label.uninformed');
        }
    },

    itemLocationProcessStatusEnumModel: [
        { ID: 1, Name: i18n.global.t('label.open')  },
        { ID: 2, Name: i18n.global.t('label.closed')  },
    ],
    itemLocationProcessStatusName(e){
        switch (e) {
            case 1: return i18n.global.t('label.open');
            case 2: return i18n.global.t('label.closed');
            default: return i18n.global.t('label.uninformed');
        }
    },

    ItemLocationProcessItemStatusEnumModel: [
        { ID: 1, Name: i18n.global.t('label.allocad')  },
        { ID: 2, Name: i18n.global.t('label.unAllocad')  },
    ],
    ItemLocationProcessItemStatusName(e){
        switch (e) {
            case 1: return i18n.global.t('label.allocad');
            case 2: return i18n.global.t('label.unAllocad');
            default: return i18n.global.t('label.uninformed');
        }
    },

    ItemEntryStatusEnumModel: [
        { ID: 1, Name: i18n.global.t('label.closed')  },
        { ID: 2, Name: i18n.global.t('label.cancelled')  },
    ],
    ItemEntryStatusName(e){
        switch (e) {
            case 1: return i18n.global.t('label.closed');
            case 2: return i18n.global.t('label.cancelled');
            default: return i18n.global.t('label.uninformed');
        }
    },

    ContractStatusEnumModel: [
        { ID: 1, Name: i18n.global.t('label.open')  },
        { ID: 2, Name: i18n.global.t('label.closed')  },
        { ID: 3, Name: i18n.global.t('label.cancelled')  },
    ],
    ContractStatusName(e){
        switch (e) {
            case 1: return i18n.global.t('label.open');
            case 2: return i18n.global.t('label.closed');
            case 3: return i18n.global.t('label.cancelled');
            default: return i18n.global.t('label.uninformed');
        }
    },
    ContractStatusColor(e){
        switch (e) {
            case 1: return 'success';
            case 2: return 'info';
            case 3: return 'error';
            default: return "";
        }
    },

    ObjectTypeEnumModel: [
        { ID: 1, Name: i18n.global.t('label.users')  },
        { ID: 2, Name: i18n.global.t('label.partners')  },
        { ID: 3, Name: i18n.global.t('label.warehouses')  },
        { ID: 4, Name: i18n.global.t('label.itemEntries')  },
        { ID: 5, Name: i18n.global.t('label.itemLocationProcesses')  },
        { ID: 6, Name: i18n.global.t('label.items')  },
        { ID: 8, Name: i18n.global.t('label.itemTypes')  },
        { ID: 9, Name: i18n.global.t('label.itemFeatures')  },
        { ID: 10, Name: i18n.global.t('label.itemGroups')  },
        { ID: 11, Name: i18n.global.t('label.branches')  },
        { ID: 12, Name: i18n.global.t('label.constructions')  },
        { ID: 13, Name: i18n.global.t('label.departments')  },
        { ID: 14, Name: i18n.global.t('label.collaborators')  },
        { ID: 15, Name: i18n.global.t('label.authorizationGroups')  },
        
    ],
    ObjectTypeEnumName(e){
        switch (e) {
            case 1: return i18n.global.t('label.users');
            case 2: return i18n.global.t('label.partners');
            case 3: return i18n.global.t('label.warehouses');
            case 4: return i18n.global.t('label.itemEntries');
            case 5: return i18n.global.t('label.itemLocationProcesses');
            case 6: return i18n.global.t('label.items');
            case 8: return i18n.global.t('label.itemTypes');
            case 9: return i18n.global.t('label.itemFeatures');
            case 10: return i18n.global.t('label.itemGroups');
            case 11: return i18n.global.t('label.branches');
            case 12: return i18n.global.t('label.constructions');
            case 13: return i18n.global.t('label.departments');
            case 14: return i18n.global.t('label.collaborators');
            case 15: return i18n.global.t('label.authorizationGroups');
            default: return i18n.global.t('label.uninformed');
        }
    },
    SequenceObjectTypeEnumModel(){
        return fmt.ObjectTypeEnumModel.filter(x=> 
            [2,3,6,7,8,9,10,12,13].includes(x.ID));
    },
    base64ToFile(base64Data, filename) {
        const arr = base64Data.split(',')
        const mime = arr[0].match(/:(.*?);/)[1]
        const bstr = atob(arr[1])
        let n = bstr.length
        const u8arr = new Uint8Array(n)

        while (n--) {
            u8arr[n] = bstr.charCodeAt(n)
        }

        return new File([u8arr], filename, { type: mime })
    },
    blobToBase64(blob) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => resolve(reader.result);
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    },
    isPdfOrImage(extension){
        const imagens = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.svg'];
        const pdfs = ['.pdf'];

        if (!extension.startsWith(".")) extension = "." + extension;

        return imagens.includes(extension) || pdfs.includes(extension);
    },
    
    CorrectionTypeEnumModel: [
        { ID: 1, Name: i18n.global.t('label.monthly')  },
        { ID: 2, Name: i18n.global.t('label.annual')  },
    ],
    CorrectionTypeEnumName(e){
        switch (e) {
            case 1: return i18n.global.t('label.monthly');
            case 2: return i18n.global.t('label.annual');
            default: return i18n.global.t('label.uninformed');
        }
    },
    CorrectionTypeEnumColor(e){
        switch (e) {
            case 1: return 'success';
            case 2: return 'info';
            default: return "";
        }
    },

    ChargeTypeEnumModel: [
        { ID: 1, Name: i18n.global.t('label.monthly')  },
        { ID: 2, Name: i18n.global.t('label.biannual')  },
        { ID: 3, Name: i18n.global.t('label.monthlyAndBiannual')  },
    ],
    ChargeTypeEnumName(e){
        switch (e) {
            case 1: return i18n.global.t('label.monthly');
            case 2: return i18n.global.t('label.biannual');
            case 3: return i18n.global.t('label.monthlyAndBiannual');
            default: return i18n.global.t('label.uninformed');
        }
    },
    ChargeTypeEnumColor(e){
        switch (e) {
            case 1: return 'success';
            case 2: return 'info';
            case 3: return 'primary';
            default: return "";
        }
    },

    WarehouseTransferStatusEnumModel: [
        { ID: 1, Name: i18n.global.t('label.closed')  },
        { ID: 2, Name: i18n.global.t('label.cancelled')  },
    ],
    WarehouseTransferStatusName(e){
        switch (e) {
            case 1: return i18n.global.t('label.closed');
            case 2: return i18n.global.t('label.cancelled');
            default: return i18n.global.t('label.uninformed');
        }
    },
    WarehouseTransferStatusColor(e){
        switch (e) {
            case 1: return 'success';
            case 2: return 'error';
            default: return "";
        }
    },

    ActionRequestStatusEnumModel: [
        { ID: -1, Name: i18n.global.t('label.cancelled')  },
        { ID: 1, Name: i18n.global.t('label.awaiting')  },
        { ID: 2, Name: i18n.global.t('label.processing')  },
        { ID: 3, Name: i18n.global.t('label.processed')  },
        { ID: 4, Name: i18n.global.t('label.withError')  },
    ],
    ActionRequestStatusName(e){
        switch (e) {
            case -1: return i18n.global.t('label.cancelled');
            case 1: return i18n.global.t('label.awaiting');
            case 2: return i18n.global.t('label.processing');
            case 3: return i18n.global.t('label.processed');
            case 4: return i18n.global.t('label.withError');
            default: return i18n.global.t('label.uninformed');
        }
    },
    ActionRequestStatusColor(e){
        switch (e) {
            case -1: return 'error';
            case 1: return 'warning';
            case 2: return 'info';
            case 3: return 'success';
            case 4: return 'error';
            default: return "";
        }
    },

    ActionRequestEnumModel: [
        { ID: 1, Name: i18n.global.t('label.import')  },
    ],
    ActionRequestName(e){
        switch (e) {
            case 1: return i18n.global.t('label.import');
            default: return i18n.global.t('label.uninformed');
        }
    },
    ActionRequestColor(e){
        switch (e) {
            case 1: return 'success';
            default: return "";
        }
    },
    ObjectTypeEnum:
    {
        None: 0,
        Users: 1,
        Partners: 2,
        Warehouses: 3,
        ItemEntries: 4,
        ItemLocationProcesses: 5,
        Items: 6,
        ItemTypes: 8,
        ItemFeatures: 9,
        ItemGroups: 10,
        Branches: 11,
        Constructions: 12,
        Departments: 13,
        Collaborators: 14,
        AuthorizationGroups: 15,
        Contracts: 16,
        DocumentTypes: 17,
        Documents: 18,
        WarehouseTransfers: 19,
        FinancialIndexes: 20
    }
};

export default fmt;