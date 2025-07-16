export interface SubExperiment {

     SubExpID: number;
     ExpID: number;
     startAge: number | null;
     endAge: number | null;
     SubExpName: string;
     ErrorMessage: string;
     IsPostProcessingPass: boolean;
     isIntervention: boolean;
     isDrug: boolean;
     drugName: string;
     drugUnit: string;
     drugQuantity: string;
     interventionDescription: string;
     ImageIds: Array<number>;
     ImageInfo: string;
     ImageDescription: string;
     Housing: string;
     LightCycle: string;

    }
