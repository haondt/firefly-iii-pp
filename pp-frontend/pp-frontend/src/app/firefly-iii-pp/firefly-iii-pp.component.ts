import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { DryRunResponseDto } from '../models/dtos/DryRunResponse';
import { QueryOptionDto } from '../models/dtos/QueryOption';
import { RunnerStateDto } from '../models/dtos/RunnerState';
import { QueryOperationModel } from '../models/QueryOperation';
import { ServiceResponseModel } from '../models/ServiceResponse';
import { FireflyIIIService } from '../services/FireflyIII';
import { RunnerService } from '../services/Runner';
import queryOptionsJson from '../../assets/queryOptions.json';

@Component({
  selector: 'app-firefly-iii-pp',
  templateUrl: './firefly-iii-pp.component.html',
  styleUrls: ['./firefly-iii-pp.component.scss'],
})
export class FireflyIIIPPComponent {
  busy: boolean = false;

  status: RunnerStateDto | undefined;
  timer: NodeJS.Timer | undefined;

  jobType: string = "single";
  queryOptions: QueryOptionDto[] = Object.assign([], queryOptionsJson);

  singleId: string | null = null;

  startDate: Date | null = null;
  endDate: Date | null = null;

  queryOperations: { viewValue: string, queryOperation: QueryOperationModel }[] = [];

  dryRunResponse: DryRunResponseDto | null = null;

  constructor(private runnerService: RunnerService,
      private fireflyIIIService: FireflyIIIService,
        private snackBar: MatSnackBar) {
    this.initData();
  }

  initData() {
    this.busy = true;
    this.runnerService.getStatus().subscribe(res => {
      try {
        if (res.success) {
          this.status = res.body!;
          if (this.status.state === "running" || this.status.state === "getting-transactions") {
            this.startRefreshTimer();
          }
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  showSnackError(error?: string) {
    this.snackBar.open(error ?? "Error while executing the request", 'Dismiss', {
      duration: 5000
    });
  }

  refreshStatus() {
    this._refreshStatus();
    this.startRefreshTimer();
  }

  stopJob() {
    if (this.busy){
      return;
    }
    this.busy = true;
    this.runnerService.stopJob().subscribe(res => {
      try {
        if (res.success) {
          this.status = res.body;
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  _refreshStatus() {
    if (this.busy){
      return;
    }
    this.busy = true;
    this.runnerService.getStatus().subscribe(res => {
      try {
        if (res.success) {
          this.status = res.body!;
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  startRefreshTimer() {
    if (this.timer !== undefined) {
      clearInterval(this.timer);
    }

    this.timer = setInterval(() => {
      if (this.status && (this.status.state === "running" || this.status.state === "getting-transactions")) {
        this._refreshStatus();
      } else {
        clearInterval(this.timer);
      }
    }, 250);
  }

  getProgress() {
    if (this.status) {
      if (this.status.state === "running"
        && this.status.totalTransactions > 0
        && this.status.completedTransactions <= this.status.totalTransactions) {
          return (this.status.completedTransactions / this.status.totalTransactions) * 100;
      } else if (this.status.state === "getting-transactions"
        && this.status.totalTransactions > 0
        && this.status.queuedTransactions <= this.status.totalTransactions) {
          return (this.status.queuedTransactions / this.status.totalTransactions) * 100;
        }
    }
    return 0;
  }

  changeJobType(event: string) {
    this.jobType = event;

    // reset
    this.singleId = null;
    this.startDate = null;
    this.endDate = null;
    this.queryOperations = [];
  }

  startJob() {
    if (this.busy) {
      return;
    }

    let obs: Observable<ServiceResponseModel<RunnerStateDto>> | null = null;
    if (this.jobType === "single" && this.singleId) {
      obs = this.runnerService.startSingleJob({id: this.singleId});
    } else if (this.jobType === "many" && this.startDate && this.endDate) {
      obs = this.runnerService.startJob({
        start: this.startDate.toISOString(),
        end: this.endDate.toISOString()
      });
    } else if (this.jobType === "query") {
      obs = this.runnerService.startQueryJob({
        operations: this.queryOperations.map(op => op.queryOperation)
      });
    }

    if (obs) {
      this.busy = true;
      obs.subscribe(res => {
        try {
          if (res.success) {
            this.status = res.body!;
            if (this.status.state === "running" || this.status.state === "getting-transactions") {
              this.startRefreshTimer();
            }
          } else {
            this.status = undefined;
            this.showSnackError(res.error);
          }
        } finally {
          this.busy = false;
        }
      });
    }
  }

  dryRunJob() {
    if (this.jobType !== "query"){
      this.showSnackError("Set job type to 'query' to enable dry run");
      return;
    }
    if (this.busy) {
      return;
    }
    this.busy = true;
    this.runnerService.dryRunJob({
      operations: this.queryOperations.map(op => op.queryOperation)
    }).subscribe(res => {
      try {
        if (res.success) {
          this.dryRunResponse = res.body!;
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  formatDryRunSample(sample: Object) {
    return JSON.stringify(sample, null, 4);
  }
}