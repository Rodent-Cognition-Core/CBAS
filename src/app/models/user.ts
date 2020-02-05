export class User {

    /**
     * Profile data as in ApplicationUser.cs.
     */
    public givenName: string;
    public familyName: string;
    public Email: string;
    public selectedPiSiteIds: Array<number>;
    public termsConfirmed: boolean;

    /**
     * From OpenID.
     */
    public userName: string;

    /**
     * Identity resource added in Config.cs.
     */
    public roles: string[];

}
