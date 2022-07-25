export const groupAttachementsByEmail = (arr) => {
    const deepCopyArr = JSON.parse(JSON.stringify(arr));
    const separatedAttachment = deepCopyArr.reduce((acc, cur) => {
        cur.LeadingText === "Vedlegg" ? acc.att.push(cur) : acc.nonAtt.push(cur);
        return acc;
    }, { att: [], nonAtt: [] });

    let nonAttArr = [...separatedAttachment.nonAtt];
    [...separatedAttachment.att].forEach(att => {
        let parent = nonAttArr.findIndex(p => p.Id === att.ActivityId);
        if (nonAttArr[parent].Attachments) {
            nonAttArr[parent].Attachments = [...nonAttArr[parent].Attachments, att];
        } else {
            nonAttArr[parent].Attachments = [att];
        }
    });
    return nonAttArr;
}