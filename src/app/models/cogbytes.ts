import { PubscreenAuthor } from "./pubscreenAuthor";

export class Cogbytes {

    public id: number;
    public title: string;
    public keywords: string;
    public doi: string;
    public authourID: Array<number>;
    public authorString: string;
    public author: Array<PubscreenAuthor>;
    public piString: string;
    public piID: number;
    public link: string;
    public privacyStatus: boolean;
    public description: string;
    public additionalNotes: string;

}
