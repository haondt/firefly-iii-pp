import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { QueryOptionDto } from '../models/dtos/QueryOption';
import { RunnerStateDto } from '../models/dtos/RunnerState';
import { QueryOperationModel } from '../models/QueryOperation';
import { RunnerService } from '../services/Runner';

interface QueryOperatorModel {
  viewValue: string,
  operator: string,
  type: string
}

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
  queryOptions: QueryOptionDto[] = [];

  startDate: Date | null = null;
  endDate: Date | null = null;

  queryOperand: QueryOptionDto | null = null;
  queryOperatorOptions: QueryOperatorModel[] = [];
  queryOperator: QueryOperatorModel | null = null;
  queryResult: any;
  queryOperations: { viewValue: string, queryOperation: QueryOperationModel }[] = [];

  constructor(private runnerService: RunnerService,
        private snackBar: MatSnackBar) {
    this.initData();
  }

  initData() {
    this.busy = true;
    let gettingQueryOptions = true;
    let gettingStatus = true;
    this.runnerService.getQueryOptions().subscribe(res => {
      try {
        if (res.success) {
          this.queryOptions = res.body!;
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        gettingQueryOptions = false;
        this.busy = gettingStatus;
      }
    });

    this.runnerService.getStatus().subscribe(res => {
      try {
        if (res.success) {
          this.status = res.body;
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        gettingStatus = false;
        this.busy = gettingQueryOptions;
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

  startRefreshTimer() {
    clearInterval(this.timer);
    this.timer = setInterval(() => {
      if (this.status && this.status.state === "running") {
        this._refreshStatus();
      } else {
        clearInterval(this.timer);
      }
    }, 1000);
  }

  getProgress() {
    if (this.status
      && this.status.state === "running"
      && this.status.totalTransactions > 0
      && this.status.completedTransactions <= this.status.totalTransactions) {
        return (this.status.completedTransactions / this.status.totalTransactions) * 100;
    }
    return 0;
  }

  changeJobType(event: string) {
    this.jobType = event;

    // reset
    this.startDate = null;
    this.endDate = null;
    this.queryOperations = [];
    this.queryOperand = null;
    this.queryOperatorOptions = [];
    this.queryOperator = null;
    this.queryResult = undefined;
  }

  changeQueryOperand() {
    this.queryOperatorOptions = this.queryOperand?.operators ?? [];

    // reset
    this.queryOperator = null;
    this.queryResult = undefined;
  }

  changeQueryOperator() {
    this.queryResult = null;
  }

  addQueryOperation() {
    if (this.queryOperand && this.queryOperator && this.queryResult) {
      let operation = {
        viewValue: `${this.queryOperand.viewValue} ${this.queryOperator.viewValue}`,
        queryOperation: {
          operand: this.queryOperand.operand,
          operator: this.queryOperator.operator,
          result: ""
        }
      };

      if (this.queryOperator.type === "string") {
        operation.queryOperation.result = <string>this.queryResult;
        operation.viewValue += " " + operation.queryOperation.result;
        this.queryOperations.push(operation);
      } else if (this.queryOperator.type === "date") {
        operation.queryOperation.result = (<Date>this.queryResult).toISOString();
        operation.viewValue += " " + (<Date>this.queryResult).toLocaleDateString('en-CA');
        this.queryOperations.push(operation);
      } else {
        this.showSnackError(`Unable to add query operator type ${this.queryOperator.type}`);
      }
    }
  }

  removeQueryOperation(event: { viewValue: string, queryOperation: QueryOperationModel }) {
    const i = this.queryOperations.indexOf(event);
    if (i >= 0) {
      this.queryOperations.splice(i, 1);
    }
  }
}