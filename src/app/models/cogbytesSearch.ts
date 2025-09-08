export interface CogbytesSearch {

     repID: Array<number>;
     keywords: string;
     doi: string;
     authorID: Array<number>;
     psID: Array<number>;

     taskID: Array<number>;
     specieID: Array<number>;
     sexID: Array<number>;
     strainID: Array<number>;
     genoID: Array<number>;
     startAge: number | null;
     endAge: number | null;

     yearFrom: any;
     yearTo: any

     intervention: string;

     fileTypeID: Array<number>;

     diseaseID: Array<number>;
     subModelID: Array<number>;
     regionID: Array<number>;
     subRegionID: Array<number>;
     cellTypeID: Array<number>;
     methodID: Array<number>;
     subMethodID: Array<number>;
     transmitterID: Array<number>;
}
