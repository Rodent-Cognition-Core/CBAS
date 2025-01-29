export interface Experiment {

  expID: number;
  userID: string;
  puSID: number;
  taskID: number;
  expName: string;
  piSiteName: string;
  piSiteUser: string;
  userName: string;
  taskName: string;
  startExpDate: Date;
  endExpDate: Date;
  // ErrorMessage: string;
  taskDescription: string;
  taskBattery: string;
  doi: string;
  status: boolean;
  imageIds: number[];
  // ImagePath: Array<string>;
  imageInfo: string;
  speciesID: number;
  species: string;

  multipleSessions: boolean;
  repoGuid: string;

}
