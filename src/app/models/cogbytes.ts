export interface Cogbytes {

    id: number;
    title: string;
    date: string;
    keywords: string;
    doi: string;
    authourID: Array<number>;
    authorString: string;
    piID: Array<number>;
    piString: string;
    link: string;
    privacyStatus: boolean;
    description: string;
    additionalNotes: string;
    dateRepositoryCreated: string;
    taskID: Array<number>;
    specieID: Array<number>;
    sexID: Array<number>;
    strainID: Array<number>;
    genoID: Array<number>;
    startAge: number | null;
    endAge: number | null;
    numSubjects: number;
    housing: string;
    lightCycle: string;
    taskBattery: string;

}
