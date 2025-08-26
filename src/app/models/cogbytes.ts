export interface Cogbytes {

    id: number;
    title: string;
    date: string;
    keywords: string;
    doi: string;
    authourID: Array<number>;
    authorString: string;
    psID: Array<number>;
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
    diseaseID: Array<number>;
    subModelID: Array<number>;
    regionID: Array<number>;
    subRegionID: Array<number>;
    cellTypeID: Array<number>;
    methodID: Array<number>;
    subMethodID: Array<number>;
    transmitterID: Array<number>;
    housing: string;
    lightCycle: string;
    taskBattery: string;

}
