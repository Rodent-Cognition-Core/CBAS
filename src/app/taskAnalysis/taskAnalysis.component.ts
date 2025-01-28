import { Component, OnInit } from '@angular/core';
import { DataSource } from '@angular/cdk/collections';
import { Observable } from 'rxjs';

import { TaskAnalysis } from '../models/taskAnalysis';
import { TaskAnalysisService } from '../services/taskanalysis.service';

export class TaskAnalysisDataSource extends DataSource<any> {
  constructor(private taskAnalysisService: TaskAnalysisService) {
    super();
  }

  /**
       * Connect function called by the table to retrieve one stream containing the data to render.
       */
  connect(): Observable<any[]> {
    return this.taskAnalysisService.taskAnalysises;
  }

  disconnect() {
    // gtfr
  }
}

@Component({
  selector: 'app-taskanalysis',
  templateUrl: './taskAnalysis.component.html',
  styleUrls: ['./taskAnalysis.component.scss']
  })
export class TaskAnalysisComponent implements OnInit {

  displayedColumns = ['id', 'taskname', 'originalname'];

  public dataSource = new TaskAnalysisDataSource(this.taskAnalysisService);

  private _taskAnalysis: TaskAnalysis;

  constructor(private taskAnalysisService: TaskAnalysisService) {
    this._taskAnalysis = { id: 0, name: '', originalName: '', taskDescription: '' };

  }

  ngOnInit() {
    this.taskAnalysisService.getAll();
  }

  public insertTaskAnalysis() {
    this._taskAnalysis.id = 1;
    this._taskAnalysis.name = 'ta name';
    this._taskAnalysis.originalName = 'ta on';
    this.taskAnalysisService.create(this._taskAnalysis);
  }

}

/**
 * Data source to provide data rendered in the table.
 */

