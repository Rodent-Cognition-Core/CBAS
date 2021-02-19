export class CogbytesUpload {

    public id: number;
    public repId: number;
    public fileTypeId: number;
    public name: string;
    public dateUpload: string;
    public description: string;
    public additionalNotes: string;
    public isIntervention: boolean;
    public interventionDescription: string;
    public imageIds: string;
    public imageDescription: string;
    public housing: string;
    public lightCycle: string;
    public taskBattery: string;

    public taskID: Array<number>;
    public specieID: Array<number>;
    public sexID: Array<number>;
    public strainID: Array<number>;
    public genoID: Array<number>;
    public ageID: Array<number>;    

}
