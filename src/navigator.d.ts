interface Navigator {
    msSaveBlob?: (blob: Blob, defaultName?: string) => boolean;
    msSaveOrOpenBlob?: (blob: Blob, defaultName?: string) => boolean;
}
