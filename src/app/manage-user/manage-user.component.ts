import { Component, OnInit, NgModule } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ManageUserService } from '../services/manageuser.service';
import { PagerService } from '../services/pager.service';
import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
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
    _user = new User();


    constructor(
        private pagerService: PagerService,
        public dialog: MatDialog,
        private manageruserService: ManageUserService,
        private identityService: IdentityService) {
    } 

    ngOnInit() {

        this.getUserList();
    }

    getUserList() {

        this.manageruserService.GetAllUser().subscribe(data => {
            this.userList = data;
            this.setPage(1);
            //console.log(this.userList);
        })

    }

    // Function defintion to add pagination to tabl Uploadlist (minor errors)
    setPage(page: number) {
        var filteredItems = this.userList;

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

    filterByString(data, s): any {
        s = s.trim();
        //console.log(s.toString());
        return data.filter(e => e.email.includes(s) || e.familyName.includes(s) || e.givenName.includes(s) || e.emailConfirmed.toString().includes(s)); // || e.another.includes(s)
        //.sort((a, b) => a.userFileName.includes(s) && !b.userFileName.includes(s) ? -1 : b.userFileName.includes(s) && !a.userFileName.includes(s) ? 1 : 0);
    }

    // Delete User
    deleteUser(username: string) {
        this.identityService.delete(username);
    }

    // Approve User
    approveUser(email: any, givenName: any, familyName: any) {
        this._user.Email = email;
        this._user.givenName = givenName;
        this._user.familyName = familyName;
        this.manageruserService.approve(this._user).subscribe(data => {
            this.getUserList();
        })
    }

    // Lock User
    lockUser(email: string) {

        this.manageruserService.lock(email).subscribe(data => {
            this.getUserList();
        })

    }




}
