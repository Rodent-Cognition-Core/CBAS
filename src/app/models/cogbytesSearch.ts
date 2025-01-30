export interface CogbytesSearch {

  repID: number[];
  keywords: string;
  doi: string;
  authorID: number[];
  piID: number[];

  taskID: number[];
  specieID: number[];
  sexID: number[];
  strainID: number[];
  genoID: number[];
  ageID: number[];

  yearFrom: number;
  yearTo: number;

  intervention: string;

  fileTypeID: number[];
}
