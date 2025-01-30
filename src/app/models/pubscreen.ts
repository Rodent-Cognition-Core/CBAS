import { PubscreenAuthor } from './pubscreenAuthor';

export interface Pubscreen {

  id: number;
  title: string;
  abstract: string;
  keywords: string;
  doi: string;
  year: string;
  yearID: string[];
  authourID: number[];
  authorString: string;
  paperTypeIdSearch: number[];
  paperTypeID: number;
  paperType: string;
  taskID: number[];
  subTaskID: number[];
  specieID: number[];
  sexID: number[];
  strainID: number[];
  diseaseID: number[];
  subModelID: number[];
  regionID: number[];
  subRegionID: number[];
  cellTypeID: number[];
  methodID: number[];
  subMethodID: number[];
  transmitterID: number[];
  author: PubscreenAuthor[];
  reference: string;
  source: string;
  yearFrom: number;
  yearTo: number;
  taskOther: string;
  specieOther: string;
  strainMouseOther: string;
  strainRatOther: string;
  diseaseOther: string;
  celltypeOther: string;
  methodOther: string;
  neurotransOther: string;
  search: string;



}
