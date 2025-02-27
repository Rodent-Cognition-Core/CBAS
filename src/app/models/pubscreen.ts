import { PubscreenAuthor } from "./pubscreenAuthor";

export interface Pubscreen {

     id: any;
     title: string;
     abstract: string;
     keywords: string;
     doi: string;
     year: any;
     yearID: Array<string>;
     authourID: Array<number>;
     authorString: string;
     paperTypeIdSearch: Array<number>;
     paperTypeID: number;
     paperType: string;
     taskID: Array<number>;
     subTaskID: Array<number>;
     specieID: Array<number>;
     sexID: Array<number>;
     strainID: Array<number>;
     diseaseID: Array<number>;
     subModelID: Array<number>;
     regionID: Array<number>;
     subRegionID: Array<number>;
     cellTypeID: Array<number>;
     methodID: Array<number>;
     subMethodID: Array<number>;
     transmitterID: Array<number>;
     author: Array<PubscreenAuthor>;
     reference: string;
     source: string;
     yearFrom: any;
     yearTo: any;
     taskOther: string;
     specieOther: string;
     strainMouseOther: string;
     strainRatOther: string;
     diseaseOther: string;
     celltypeOther: string;
     methodOther: string;
     neurotransOther: string;
     search: string;

                 

}
