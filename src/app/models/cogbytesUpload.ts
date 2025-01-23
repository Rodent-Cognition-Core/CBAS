export interface CogbytesUpload {

     id: number;
     repId: number;
     fileTypeId: number;
     name: string;
     dateUpload: string;
     description: string;
     additionalNotes: string;
     isIntervention: boolean;
     interventionDescription: string;
     imageIds: string;
     imageDescription: string;
     housing: string;
     lightCycle: string;
     taskBattery: string;

     taskID: Array<number>;
     specieID: Array<number>;
     sexID: Array<number>;
     strainID: Array<number>;
     genoID: Array<number>;
     ageID: Array<number>;

     numSubjects: number;

}
