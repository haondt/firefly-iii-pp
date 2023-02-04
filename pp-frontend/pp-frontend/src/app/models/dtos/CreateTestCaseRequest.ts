export interface CreateTestCaseRequestDto {
    bodyFields: {
        [key: string]: string
    },
    folderName: string,
    createFolderMode: string,
    configureExpectedValues: boolean,
    expectedValues: {
        [key: string]: string
    }
}