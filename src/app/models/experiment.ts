export class Experiment {

    public ExpID: number;
    public UserID: string;
    public PUSID: number;
    public TaskID: number;
    public ExpName: string;
    public PISiteName: string;
    public PISiteUser: string;
    public UserName: string;
    public TaskName: string;
    public StartExpDate: Date;
    public EndExpDate: Date;
    //public ErrorMessage: string;
    public TaskDescription: string;
    public TaskBattery: string;
    public DOI: string;
    public Status: boolean;
    public ImageIds: Array<number>;
    //public ImagePath: Array<string>;
    public ImageInfo: string;
    public SpeciesID: number;
    public species: string;

}
