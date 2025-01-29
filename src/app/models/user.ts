export interface User {

  /**
     * Profile data as in ApplicationUser.cs.
     */
  givenName: string;
  familyName: string;
  email: string;
  selectedPiSiteIds: number[];
  termsConfirmed: boolean;
  userName: string;

  roles: string[];


}
