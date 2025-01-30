export interface DataExtraction {

  taskID: number;
  taskName: string;
  expIDs: number[];
  subtaskID: number;
  subTaskName: string;
  sessionInfoNames: string[];
  markerInfoNames: string[];
  aggNames: string;
  pisiteIDs: number[];
  ageVals: number[];
  sexVals: string[];
  genotypeVals: number[];
  strainVals: number[];
  isTrialByTrials: boolean;
  subExpID: number[];
  sessionName: string[];
  species: string;
  speciesID: number;

}
