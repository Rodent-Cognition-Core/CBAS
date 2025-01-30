import { PubscreenAuthor } from './pubscreenAuthor';

export interface Cogbytes {

  id: number;
  title: string;
  date: string;
  keywords: string;
  doi: string;
  authourID: number[];
  authorString: string;
  piID: number[];
  piString: string;
  link: string;
  privacyStatus: boolean;
  description: string;
  additionalNotes: string;
  dateRepositoryCreated: string;

}
