export class CogbytesUpload {

    public id: number;
    public fileType: string;
    public description: string;
    public additionalNotes: string;

    public taskID: Array<number>;
    public specieID: Array<number>;
    public sexID: Array<number>;
    public strainID: Array<number>;
    public genoID: Array<number>;
    public ageID: Array<number>;
    public housingID: Array<number>;
    public lightID: Array<number>;
    public isIntervention: boolean;
    public interventionDescription: string;
    public images: Array<number>;
    public imageDescription: string;

    public taskOther: string;
    public specieOther: string;
    public strainOther: string;              

}
