import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ManageUserService } from '../services/manageuser.service';
import { PagerService } from '../services/pager.service';
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
        this._user = { Email: '', familyName: '', givenName: '', roles: [], selectedPiSiteIds: [], termsConfirmed: false, userName: '' }
    } 

    ngOnInit() {

        this.getUserList();
    }

    getUserList() {

        this.manageruserService.GetAllUser().subscribe((data : any) => {
            this.userList = data;
            this.setPage(1);
            //console.log(this.userList);
        })

    }

    // Function defintion to add pagination to tabl Uploadlist (minor errors)
    setPage(page: number) {
        let filteredItems = this.userList;

        filteredItems = this.filterByString(this.userList, this.expfilter);
        console.log(filteredItems);

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
        return data.filter((e: any) => e.email.includes(s) || e.familyName.includes(s) ||
              e.givenName.includes(s) || e.emailConfirmed.toString().includes(s));
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
        this.manageruserService.approve(this._user).subscribe((_data : any) => {
            this.getUserList();
        })
    }

    // Lock User
    lockUser(email: string) {

        this.manageruserService.lock(email).subscribe((_data : any) => {
            this.getUserList();
        })

    }




}
