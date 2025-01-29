export interface CogbytesUpload {

  id: number;
  repId: number;
  fileTypeId: number;
  name: string;
  dateUpload: string;
  description: string;
  additionalNotes: string;
  isIntervention: boolean;
  interventionDescription: string;
  imageIds: string;
  imageDescription: string;
  housing: string;
  lightCycle: string;
  taskBattery: string;

  taskID: number[];
  specieID: number[];
  sexID: number[];
  strainID: number[];
  genoID: number[];
  ageID: number[];

  numSubjects: number;

}
