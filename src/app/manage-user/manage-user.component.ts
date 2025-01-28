import { Component, OnInit, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ManageUserService } from '../services/manageuser.service';
import { PagerService } from '../services/pager.service';
// import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { IdentityService } from '../services/identity.service';
import { User } from '../models/user';


@Component({
  selector: 'app-manage-user',
  templateUrl: './manage-user.component.html',
  styleUrls: ['./manage-user.component.scss']
  })
export class ManageUserComponent implements OnInit {


  pagedItems: any[];
  userList: any;
  expfilter: any = '';
  pager: any = {};
  _user: User;


  constructor(
    private pagerService: PagerService,
    public dialog: MatDialog,
    private manageruserService: ManageUserService,
    private identityService: IdentityService) {

    this.pagedItems = [];
    this._user = { email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' };
  }

  ngOnInit() {

    this.getUserList();
  }

  getUserList() {

    this.manageruserService.getAllUser().subscribe((data: any) => {
      this.userList = data;
      this.setPage(1);
      // console.log(this.userList);
    });

  }

  // Function defintion to add pagination to tabl Uploadlist (minor errors)
  setPage(page: number) {
    let filteredItems = this.userList;

    filteredItems = this.filterByString(this.userList, this.expfilter);

    // get pager object from service
    this.pager = this.pagerService.getPager(filteredItems.length, page, 10);

    if (page < 1 || page > this.pager.totalPages) {
      this.pagedItems = [];
      return;
    }

    // get current page of items
    this.pagedItems = filteredItems.slice(this.pager.startIndex, this.pager.endIndex + 1);
  }

  search(): any {
    this.setPage(1);
  }

  filterByString(data: any, s: string): any {
    s = s.trim();
    // console.log(s.toString());
    const dataArray = Array.isArray(data.result) ? data.result : [];
    return dataArray.filter((e: any) => e.email.includes(s) || e.familyName.includes(s) ||
          e.givenName.includes(s) || e.emailConfirmed.toString().includes(s));
  }

  // Delete User
  deleteUser(username: string) {
    this.identityService.delete(username);
  }

  // Approve User
  approveUser(email: any, givenName: any, familyName: any) {
    this._user.email = email;
    this._user.givenName = givenName;
    this._user.familyName = familyName;
    this.manageruserService.approve(this._user).subscribe((data: any) => {
      this.getUserList();
    });
  }

  // Lock User
  lockUser(email: string) {

    this.manageruserService.lock(email).subscribe((data: any) => {
      this.getUserList();
    });

  }




}
