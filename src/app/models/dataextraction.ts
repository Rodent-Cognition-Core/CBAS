export interface DataExtraction {

  taskID: number;
  taskName: string;
  expIDs: Array<number>;
  subtaskID: number;
  subTaskName: string;
  sessionInfoNames: Array<string>;
  markerInfoNames: Array<string>;
  aggNames: string;
  pisiteIDs: Array<number>;
  ageVals: Array<number>;
  sexVals: Array<string>;
  genotypeVals: Array<number>;
  strainVals: Array<number>;
  isTrialByTrials: boolean;
  subExpID: Array<number>;
  sessionName: Array<string>;
  species: string;
  speciesID: number;

}
