import { Component, OnInit, Inject, NgModule, Input } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators, ReactiveFormsModule, FormGroup, UntypedFormBuilder } from '@angular/forms';
// import { NgModel } from '@angular/forms';
// import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { ReplaySubject ,  Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
// import { ManageUserService } from '../services/manageuser.service';
// import { PagerService } from '../services/pager.service';
// import { DeleteConfirmDialogComponent } from '../delete-confirm-dialog/delete-confirm-dialog.component';
import { AuthorDialogeComponent } from '../authorDialoge/authorDialoge.component';
// import { IdentityService } from '../services/identity.service';
import { PubScreenService } from '../services/pubScreen.service';
// import { SharedModule } from '../shared/shared.module';
import { Pubscreen } from '../models/pubscreen';
import { DOINOTVALID, FIELDISREQUIRED, PUBLICATIONWITHSAMEDOI, PUBMEDKEYNOTVALID, YEARNOTVALID } from '../shared/messages';


@Component({
  selector: 'app-shared-pubscreen',
  templateUrl: './shared-pubscreen.component.html',
  styleUrls: ['./shared-pubscreen.component.scss']
})
export class SharedPubscreenComponent implements OnInit {


  // Models Variables for adding Publication
  keywordsModel: any;
  specieModel: any;
  sexModel: any;
  strainModel: any;
  diseaseModel: any;
  regionModel: any;
  subRegionModel: any;
  cellTypeModel: any;
  methodModel: any;
  neurotransmitterModel: any;
  authorMultiSelect: any;
  authorModel2: any;
  paperTypeModel2: any;
  referenceModel: any;



  // Models Variables for searching publication
  yearSearchModel: any;

  // Definiing List Variables
  paperTypeList: any;
  taskList: any;
  specieList: any;
  sexList: any;
  strainList: any;
  diseaseList: any;
  regionSubregionList: any;
  regionList: any;
  subRegionList: any;
  cellTypeList: any;
  methodList: any;
  neurotransmitterList: any;
  authorList: any;
  authorList2: any;
  searchResultList: any;
  yearList: any;
  paperInfoFromDoiList: any;

  // Form Validation Variables for adding publications
  author: UntypedFormControl;
  title: UntypedFormControl;
  abstract: UntypedFormControl;
  doi: UntypedFormControl;
  doiKey: UntypedFormControl;
  paperType: UntypedFormControl;
  cognitiveTask: UntypedFormControl;
  // specie = new FormControl('', [Validators.required]);
  // sex = new FormControl('', [Validators.required]);
  addingOption: UntypedFormControl;
  year: UntypedFormControl;
  pubMedKey: UntypedFormControl;
  sourceOption: UntypedFormControl;
  bioAddingOption: UntypedFormControl;
  doiKeyBio: UntypedFormControl;


  // onbj variable from Models
  _pubscreen: Pubscreen;
  _pubSCreenSearch: Pubscreen;

  public authorMultiFilterCtrl: UntypedFormControl;
  public filteredAutorList: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    // private pagerService: PagerService,
    public dialog: MatDialog,
    private pubScreenService: PubScreenService, public dialogAuthor: MatDialog,
    private fb: UntypedFormBuilder) {

    this.author = fb.control('', [Validators.required]);
    this.title = fb.control('', [Validators.required]);
    this.abstract = fb.control('', [Validators.required]);
    this.doi = fb.control('', [Validators.required]);
    this.doiKey = fb.control('', [Validators.required]);
    this.paperType = fb.control('', [Validators.required]);
    this.cognitiveTask = fb.control('', [Validators.required]);
    this.addingOption = fb.control('', [Validators.required]);
    this.year = fb.control('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    this.pubMedKey = fb.control('', [Validators.required]);
    this.sourceOption = fb.control('', [Validators.required]);
    this.bioAddingOption = fb.control('', [Validators.required]);
    this.doiKeyBio = fb.control('', [Validators.required]);
    this.authorMultiFilterCtrl = fb.control('');
    this._pubSCreenSearch = {
      abstract: '', author: [], authorString: '', authourID: [], cellTypeID: [], celltypeOther: '',
      diseaseID: [], diseaseOther: '', doi: '', id: 0, keywords: '', methodID: [], methodOther: '',
      neurotransOther: '', paperType: '', paperTypeID: 0, paperTypeIdSearch: [], reference: '', regionID: [],
      search: '', sexID: [], source: '', specieID: [], specieOther: '', strainID: [], strainMouseOther: '',
      strainRatOther: '', subMethodID: [], subModelID: [], subRegionID: [], subTaskID: [], taskID: [], taskOther: '',
      title: '', transmitterID: [], year: '', yearFrom: 0, yearID: [], yearTo: 0
    };
    this._pubscreen = {
      abstract: '', author: [], authorString: '', authourID: [], cellTypeID: [], celltypeOther: '',
      diseaseID: [], diseaseOther: '', doi: '', id: 0, keywords: '', methodID: [], methodOther: '',
      neurotransOther: '', paperType: '', paperTypeID: 0, paperTypeIdSearch: [], reference: '', regionID: [],
      search: '', sexID: [], source: '', specieID: [], specieOther: '', strainID: [], strainMouseOther: '',
      strainRatOther: '', subMethodID: [], subModelID: [], subRegionID: [], subTaskID: [], taskID: [], taskOther: '',
      title: '', transmitterID: [], year: '', yearFrom: 0, yearID: [], yearTo: 0
    };
    this.resetFormVals();
  }

  ngOnInit() {

    // this.GetAuthorList();
    this.pubScreenService.getPaperType().subscribe((data: any) => {
      this.paperTypeList = data;
    });
    this.pubScreenService.getTask().subscribe((data: any) => {
      this.taskList = data;
    });
    this.pubScreenService.getSpecie().subscribe((data: any) => {
      this.specieList = data;
    });
    this.pubScreenService.getSex().subscribe((data: any) => {
      this.sexList = data;
    });
    this.pubScreenService.getStrain().subscribe((data: any) => {
      this.strainList = data;
    });
    this.pubScreenService.getDisease().subscribe((data: any) => {
      this.diseaseList = data;
    });
    this.pubScreenService.getRegion().subscribe((data: any) => {
      this.regionList = data;
    });
    this.pubScreenService.getCellType().subscribe((data: any) => {
      this.cellTypeList = data;
    });
    this.pubScreenService.getMethod().subscribe((data: any) => {
      this.methodList = data;
    });
    this.pubScreenService.getNeurotransmitter().subscribe((data: any) => {
      this.neurotransmitterList = data;
    });

    // this.pubScreenService.getAllYears().subscribe(data => { this.yearList = data; console.log(this.yearList); });
    this.getAllYears();


  }

  // Function Definition to open a dialog for adding new cognitive task to the system
  openDialogAuthor(): void {

    const dialogref = this.dialogAuthor.open(AuthorDialogeComponent, {
      height: '500px',
      width: '700px',
      data: {}

    });

    dialogref.afterClosed().subscribe(result => {

      this.getAuthorList();
    });
  }

  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  getAuthorList() {

    this.resetFormVals();

    this.pubScreenService.getAuthor().subscribe((data: any) => {
      this.authorList = data;

      // load the initial expList
      this.filteredAutorList.next(this.authorList.slice());

      this.authorMultiFilterCtrl.valueChanges
        .pipe(takeUntil(this._onDestroy))
        .subscribe(() => {
          this.filterAuthor();
        });

    });

    return this.authorList;
  }

  // Getting list of all years  in database ???
  getAllYears() {
    return this.pubScreenService.getAllYears().subscribe((data: any) => {
      this.yearList = data; /* console.log(this.yearList);*/
    });
  }

  selectedRegionChange(selectedRegion: any) {

    this.pubScreenService.getRegionSubRegion().subscribe((data: any) => {
      this.regionSubregionList = data;
      // console.log(this.regionSubregionList);
      const filtered = this.regionSubregionList.filter(function (item: any) {
        return selectedRegion.indexOf(item.rid) !== -1;
      });

      // console.log(filtered);
      this.subRegionList = JSON.parse(JSON.stringify(filtered));
    });

    // console.log(this.subRegionList);
  }

  // Handling Error for the required fields
  getErrorMessageAuthor() {

    return this.author.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessageTitle() {

    return this.title.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessageYear() {
    return this.year.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessageYearVal() {
    return this.year.hasError('pattern') ? YEARNOTVALID : '';
  }

  getErrorMessageDOI() {
    return this.doi.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessagePaperType() {

    return this.paperType.hasError('required') ? FIELDISREQUIRED : '';

  }

  getErrorMessageTask() {
    return this.cognitiveTask.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessagePaperOption() {
    return this.addingOption.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessageDOIKey() {
    return this.doiKey.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessagePubMedKey() {
    return this.pubMedKey.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessagePaperSource() {
    return this.sourceOption.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessagePaperOptionBio() {
    return this.bioAddingOption.hasError('required') ? FIELDISREQUIRED : '';
  }

  getErrorMessageDOIKeyBio() {
    return this.doiKeyBio.hasError('required') ? FIELDISREQUIRED : '';
  }

  setDisabledVal() {

    if (this.author.value == null && this.author.hasError('required')) {

      return true;
    }

    if (this.paperType.value == null && this.paperType.hasError('required')) {
      return true;
    }

    if (this.sourceOption.value === 1 && this.addingOption.hasError('required')) {
      return true;

    }

    if (this.sourceOption.value === 2 && this.bioAddingOption.hasError('required')) {
      return true;

    }

    if (this.addingOption.value === 1 && this.doiKey.hasError('required')) {
      return true;

    }

    if (this.addingOption.value === 2 && this.pubMedKey.hasError('required')) {
      return true;

    }

    if (this.bioAddingOption.value === 1 && this.doiKeyBio.hasError('required')) {
      return true;

    } else if (this.title.hasError('required') ||
            this.doi.hasError('required') ||
            this.cognitiveTask.hasError('required') ||
            this.year.hasError('required') ||
            this.year.hasError('pattern') ||
            this.sourceOption.hasError('required')


    ) {

      return true;
    } else {

      return false;
    }

  }

  setDisabledAddDOI() {
    if (this.doiKey.hasError('required')) {
      return true;
    }
    return false;
  }

  setDisabledAddPubMedID() {
    if (this.pubMedKey.hasError('required')) {
      return true;
    }
    return false;
  }

  setDisabledAddDOIBio() {
    if (this.doiKeyBio.hasError('required')) {
      return true;
    }
    return false;
  }

  // Adding DOI's paper to get some paper's info from PubMed
  addDOI(doi: any) {

    this.pubScreenService.getPaparInfoFromDOI(doi).subscribe((data: any) => {

      /* console.log(data);
                  console.log(data.result);*/

      if (data.result == null) {
        alert(DOINOTVALID);

      } else {

        this.authorModel2 = data.result.authorString;
        this.title.setValue(data.result.title);
        this.abstract.setValue(data.result.abstract);
        this.year.setValue(data.result.year);
        this.keywordsModel = data.result.keywords;
        this.doi.setValue(data.result.doi);
        this.paperTypeModel2 = data.result.paperType;
        this.referenceModel = data.result.reference;
        this.authorList2 = data.result.author;
        this.paperType.setValue(data.result.paperType);

      }

    });

  }

  // Adding pubmed key to get paper information from pubMed
  addPubMedID(pubMedKey: any) {

    this.pubScreenService.getPaparInfoFromPubmedKey(pubMedKey).subscribe((data: any) => {

      // console.log(data);
      // console.log(data.result);

      if (data.result == null) {
        alert(PUBMEDKEYNOTVALID);

      } else {

        this.authorModel2 = data.result.authorString;
        this.title.setValue(data.result.title);
        this.abstract.setValue(data.result.abstract);
        this.year.setValue(data.result.year);
        this.keywordsModel = data.result.keywords;
        this.doi.setValue(data.result.doi);
        this.paperTypeModel2 = data.result.paperType;
        this.referenceModel = data.result.reference;
        this.authorList2 = data.result.author;
        this.paperType.setValue(data.result.paperType);
      }

    });

  }

  addDOIBio(doi: any) {

    this.pubScreenService.getPaparInfoFromDOIBio(doi).subscribe((data: any) => {

      // console.log(data);
      // console.log(data.result);

      if (data.result == null) {
        alert(DOINOTVALID);

      } else {

        this.authorModel2 = data.result.authorString;
        this.title.setValue(data.result.title);
        this.abstract.setValue(data.result.abstract);
        this.year.setValue(data.result.year);
        this.keywordsModel = data.result.keywords;
        this.doi.setValue(data.result.doi);
        this.paperTypeModel2 = data.result.paperType;
        this.referenceModel = data.result.reference;
        this.authorList2 = data.result.author;
        this.paperType.setValue(data.result.paperType);

      }

    });

  }

  // Adding a new publication to DB by cliking on Submit button
  addPublication() {

    if (this.author.value != null && this.author.value.length !== 0) {
      this._pubscreen.authourID = this.author.value;
      // console.log(this.authorModel)
    } else {

      this._pubscreen.author = this.authorList2;
      this._pubscreen.authorString = this.authorModel2;
      // console.log(this._pubscreen.author)
      // console.log(this.authorList2);

    }

    if (this.paperType.value != null) {
      this._pubscreen.paperType = this.paperType.value;
    } else {
      this._pubscreen.paperType = this.paperTypeModel2;
    }
    if (this.referenceModel != null) {
      this._pubscreen.reference = this.referenceModel;
    }

    this._pubscreen.title = this.title.value;
    this._pubscreen.abstract = this.abstract.value;
    this._pubscreen.keywords = this.keywordsModel;
    this._pubscreen.doi = this.doi.value;
    this._pubscreen.year = this.year.value;
    this._pubscreen.taskID = this.cognitiveTask.value;
    this._pubscreen.specieID = this.specieModel;
    this._pubscreen.sexID = this.sexModel;
    this._pubscreen.strainID = this.strainModel;
    this._pubscreen.diseaseID = this.diseaseModel;
    this._pubscreen.regionID = this.regionModel;
    this._pubscreen.subRegionID = this.subRegionModel;
    this._pubscreen.cellTypeID = this.cellTypeModel;
    this._pubscreen.methodID = this.methodModel;
    this._pubscreen.transmitterID = this.neurotransmitterModel;

    switch (this.sourceOption.value) {

      case 1: {
        this._pubscreen.source = 'pubMed';
        break;
      }
      case 2: {
        this._pubscreen.source = 'BioRxiv';
        break;
      }

    }


    this.pubScreenService.addPublication(this._pubscreen).subscribe(data => {

      if (data == null) {
        alert(PUBLICATIONWITHSAMEDOI);
      }
      this.resetFormVals();

    });



  }

  resetFormVals() {

    this.title.setValue('');
    this.abstract.setValue('');
    this.keywordsModel = '';
    this.doi.setValue('');
    this.year.setValue('');
    this.yearSearchModel = [];
    this.author.setValue([]);
    this.paperType.setValue('');
    this.cognitiveTask.setValue([]);
    this.specieModel = [];
    this.sexModel = [];
    this.strainModel = [];
    this.diseaseModel = [];
    this.regionModel = [];
    this.subRegionModel = [];
    this.cellTypeModel = [];
    this.methodModel = [];
    this.neurotransmitterModel = [];
    this.doiKey.setValue('');
    this.authorModel2 = '';
    this.paperTypeModel2 = '';
    this.referenceModel = '';
    this.pubMedKey.setValue('');
    this.doiKeyBio.setValue('');

  }

  // Checking the errorMessage for searching publication
  setDisabledValSearch() {

    if (this.cognitiveTask.hasError('required') ||
            this.year.hasError('pattern')

    ) {
      return true;
    }
    return false;


  }


  // handling multi filtered Author list
  private filterAuthor() {
    if (!this.authorList) {
      return;
    }

    // get the search keyword
    let searchAuthor = this.authorMultiFilterCtrl.value;

    if (!searchAuthor) {
      this.filteredAutorList.next(this.authorList.slice());
      return;
    } else {
      searchAuthor = searchAuthor.toLowerCase();
    }

    // filter the Author
    this.filteredAutorList.next(
      this.authorList.filter((x: any) => x.lastName.toLowerCase().indexOf(searchAuthor) > -1)
    );
  }

}
