export interface User {

  /**
     * Profile data as in ApplicationUser.cs.
     */
  givenName: string;
  familyName: string;
  Email: string;
  selectedPiSiteIds: Array<number>;
  termsConfirmed: boolean;
  userName: string;

  roles: string[];


}
