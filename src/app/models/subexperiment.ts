export interface SubExperiment {

  subExpID: number;
  expID: number;
  ageID: number;
  ageInMonth: string;
  subExpName: string;
  errorMessage: string;
  isPostProcessingPass: boolean;
  isIntervention: boolean;
  isDrug: boolean;
  drugName: string;
  drugUnit: string;
  drugQuantity: string;
  interventionDescription: string;
  imageIds: Array<number>;
  imageInfo: string;
  imageDescription: string;
  housing: string;
  lightCycle: string;

}
