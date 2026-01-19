export interface Experiment {

     ExpID: number;
     UserID: string;
     PUSID: number;
     TaskID: number;
     ExpName: string;
     PISiteName: string;
     PISiteUser: string;
     UserName: string;
     TaskName: string;
     StartExpDate: Date;
     EndExpDate: Date;
    // ErrorMessage: string;
     TaskDescription: string;
     TaskBattery: string;
     DOI: string;
     RepoStatus: boolean;
     ImageIds: Array<number>;
    // ImagePath: Array<string>;
     ImageInfo: string;
     SpeciesID: number;
     species: string;

     multipleSessions: boolean;
     repoGuid: string;
     timeseries: boolean;

}
