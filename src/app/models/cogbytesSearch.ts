export interface CogbytesSearch {

     repID: Array<number>;
     keywords: string;
     doi: string;
     authorID: Array<number>;
     piID: Array<number>;

     taskID: Array<number>;
     specieID: Array<number>;
     sexID: Array<number>;
     strainID: Array<number>;
     genoID: Array<number>;
     ageID: Array<number>;

     yearFrom: any;
     yearTo: any

     intervention: string;

     fileTypeID: Array<number>;
}
