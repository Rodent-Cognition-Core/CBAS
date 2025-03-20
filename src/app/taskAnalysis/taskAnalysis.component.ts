import { Component, OnInit } from '@angular/core';
import { DataSource } from '@angular/cdk/collections';
import { Observable } from 'rxjs';
import { TaskAnalysis } from '../models/taskAnalysis';
import { TaskAnalysisService } from '../services/taskanalysis.service';

@Component({
    selector: 'app-task-analysis',
    templateUrl: './taskAnalysis.component.html',
    styleUrls: ['./taskAnalysis.component.scss']
})
export class TaskAnalysisComponent implements OnInit {

    displayedColumns = ['id', 'taskname', 'originalname'];

    private _taskAnalysis: TaskAnalysis;

    constructor(private taskAnalysisService: TaskAnalysisService,
        public dataSource: TaskAnalysisDataSource) {
        this._taskAnalysis = { ID: 0, Name: '', OriginalName: '', TaskDescription: '' }

    }

    ngOnInit() {
        this.taskAnalysisService.getAll();
        this.dataSource = new TaskAnalysisDataSource(this.taskAnalysisService);
    }

    public insertTaskAnalysis() {
        this._taskAnalysis.ID = 1;
        this._taskAnalysis.Name = "ta name";
        this._taskAnalysis.OriginalName = "ta on";
        this.taskAnalysisService.create(this._taskAnalysis);
    }

}

/**
 * Data source to provide data rendered in the table.
 */
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
        //gtfr
    }
}
