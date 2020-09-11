import { PubscreenAuthor } from "./pubscreenAuthor";

export class Pubscreen {

    public id: number;
    public title: string;
    public abstract: string;
    public keywords: string;
    public doi: string;
    public year: string;
    public yearID: Array<string>;
    public authourID: Array<number>;
    public authorString: string;
    public paperTypeID: Array<number>;
    public paperTypeIdSearch: Array<number>;
    public paperType: string;
    public taskID: Array<number>;
    public specieID: Array<number>;
    public sexID: Array<number>;
    public strainID: Array<number>;
    public diseaseID: Array<number>;
    public regionID: Array<number>;
    public subRegionID: Array<number>;
    public cellTypeID: Array<number>;
    public methodID: Array<number>;
    public transmitterID: Array<number>;
    public author: Array<PubscreenAuthor>;
    public reference: string;
    public source: string;
    public yearFrom: number;
    public yearTo: number;
    public taskOther: string;
                 

}
