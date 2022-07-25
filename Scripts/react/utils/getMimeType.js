export const getMimeType = (subject) => {
    const splitName = subject.split('.');
    const extension = splitName[splitName.length - 1];;

    switch (extension) {
        case 'pdf':
            return 'application/pdf';
            break;
        case 'jpeg':
        case 'jpg':
        case 'jfif':
        case 'gif':
        case 'png':
        case 'pjpeg':
        case 'pjpg':
            return 'image/jpeg';
            break;
        default:
            return 'application/pdf';
    }
};