import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { AutoReconcileJoiningStrategyOptionsModel, AutoReconcileRequestOptionsModel } from '../models/AutoReconcileRequestOptions';
import { AutoReconcileDryRunResultResponseDto } from '../models/dtos/AutoReconcileDryRunResultResponse';
import { AutoReconcileRequestDto } from '../models/dtos/AutoReconcileRequest';
import { DryRunResponseDto } from '../models/dtos/DryRunResponse';
import { QueryOptionDto } from '../models/dtos/QueryOption';
import { RunnerStateDto } from '../models/dtos/RunnerState';
import { QueryOperationModel } from '../models/QueryOperation';
import { ServiceResponseModel } from '../models/ServiceResponse';
import { FireflyIIIService } from '../services/FireflyIII';
import { RunnerService } from '../services/Runner';

import requestOptions from '../../assets/autoReconcileRequestOptions.json';
import { AutoReconcileService } from '../services/AutoReconcile';
import { checkResult } from '../utils/ObservableUtils';
import { CurrencyPipe } from '@angular/common';
import { AutoReconcileStatusDto } from '../models/dtos/AutoReconcileStatus';

interface QueryOperatorModel {
  viewValue: string,
  operator: string,
  type: string
}

interface QueryOperationWrapper {
  viewValue: string,
  queryOperation: QueryOperationModel
}

@Component({
  selector: 'app-auto-reconcile',
  templateUrl: './auto-reconcile.component.html',
  styleUrls: ['./auto-reconcile.component.scss'],
})
export class AutoReconcileComponent {
  sourceQueryOperations: QueryOperationWrapper[] = [];
  destinationQueryOperations: QueryOperationWrapper[] = [];

  requestDto: AutoReconcileRequestDto = {
    sourceQueryOperations: [],
    destinationQueryOperations: [],
    pairingStrategy: {
      requireMatchingDescriptions: false,
      requireMatchingDates: false,
      dateMatchToleranceInDays: 0
    },
    joiningStrategy: {
      descriptionJoinStrategy: null,
      dateJoinStrategy: null,
      categoryJoinStrategy: null,
      notesJoinStrategy: null
    }
  };

  requestOptions: AutoReconcileRequestOptionsModel = Object.assign({}, requestOptions);

  dryRunResponseDto: AutoReconcileDryRunResultResponseDto|null = null;

  busy: boolean = false;
  running: boolean = false;
  status: AutoReconcileStatusDto|undefined;
  timer: NodeJS.Timer | undefined;
  waitingForDryRunResult: boolean = false;

  constructor(private autoReconcileService: AutoReconcileService,
        private snackBar: MatSnackBar,
        private currencyPipe: CurrencyPipe) {
    this.initData();
  }

  initData() {
    this.destinationQueryOperations = [{
      viewValue: "Source account is (no name)",
      queryOperation: {
        operand: "source_account",
        operator: "is",
        result: "(no name)"
      }
    }];

    this.sourceQueryOperations = [{
      viewValue: "Destination account is (no name)",
      queryOperation: {
        operand: "destination_account",
        operator: "is",
        result: "(no name)"
      }
    }];

    this.requestDto.joiningStrategy.descriptionJoinStrategy = "concatenate";
    this.requestDto.joiningStrategy.dateJoinStrategy = "average";
    this.requestDto.joiningStrategy.categoryJoinStrategy = "clear";
    this.requestDto.joiningStrategy.notesJoinStrategy = "concatenate";

    this.refreshStatus();
  }

  showSnackError(error?: string) {
    this.snackBar.open(error ?? "Error while executing the request", 'Dismiss', {
      duration: 5000
    });
  }

  isRunning() {
    return this.status
      && this.status.state !== "failed"
      && this.status.state !== "completed"
      && this.status.state !== "stopped";
  }

  refreshStatus() {
    this._refreshStatus();
    this.startRefreshTimer();
  }

  _refreshStatus() {
    if (this.busy){
      return;
    }
    this.busy = true;
    this.autoReconcileService.getStatus().subscribe(checkResult<AutoReconcileStatusDto>({
      success: s => {
        this.status = s
        if (!this.isRunning() && this.waitingForDryRunResult) {
          this.waitingForDryRunResult = false;
          this.autoReconcileService.getDryRunResult().subscribe(checkResult<AutoReconcileDryRunResultResponseDto>({
            success: s => this.dryRunResponseDto = s,
            fail: e => {
              this.dryRunResponseDto = null;
              this.showSnackError(e);
            },
            finally: () => {}
          }));
        }
      },
      fail: e => {
        this.status = undefined;
        if (this.waitingForDryRunResult) {
          this.waitingForDryRunResult = false;
          this.dryRunResponseDto = null;
        }
        this.showSnackError(e);
      },
      finally: () => this.busy = false
    }));
  }

  startRefreshTimer() {
    if (this.timer !== undefined) {
      clearInterval(this.timer);
    }

    this.timer = setInterval(() => {
      if (this.isRunning()) {
        this._refreshStatus();
      } else {
        clearInterval(this.timer);
      }
    }, 250);
  }

  prepareRequestDto() {
    this.requestDto.sourceQueryOperations = this.sourceQueryOperations.map(o => o.queryOperation);
    this.requestDto.destinationQueryOperations = this.destinationQueryOperations.map(o => o.queryOperation);
  }


  formatAmount(amount: number) {
    return this.currencyPipe.transform(amount, "$");
  }

  formatDate(date: string) {
    return new Date(date).toLocaleDateString('en-CA');
  }

  fixDateMatchTolerance() {
    let v = this.requestDto.pairingStrategy.dateMatchToleranceInDays;
    v = Math.max(0, v);
    v = Math.min(14, v);
    v = Math.floor(v);
    this.requestDto.pairingStrategy.dateMatchToleranceInDays = v;
  }

  dryRun() {
    this.prepareRequestDto();
    if (this.busy) {
      return;
    }
    this.busy = true;
    this.dryRunResponseDto = null;
    this.autoReconcileService.dryRun(this.requestDto)
      .subscribe(checkResult<AutoReconcileStatusDto>({
        success: r => {
          this.status = r;
          this.waitingForDryRunResult = true;
          this.startRefreshTimer();
        },
        fail: e => this.showSnackError(e),
        finally: () => this.busy = false
      }));
  }

  startJob() {
    this.prepareRequestDto();
    if (this.busy) {
      return;
    }
    this.busy = true;
    this.autoReconcileService.run(this.requestDto)
      .subscribe(checkResult<AutoReconcileStatusDto>({
        success: r => {
          this.status = r;
          this.startRefreshTimer();
        },
        fail: e => this.showSnackError(e),
        finally: () => this.busy = false
      }));
  }

  stopJob() {
    if (this.busy) {
      return;
    }
    this.busy = true;
    this.waitingForDryRunResult = false;
    this.autoReconcileService.stop()
      .subscribe(checkResult<AutoReconcileStatusDto>({
        success: r => {
          this.status = r;
        },
        fail: e => this.showSnackError(e),
        finally: () => this.busy = false
      }));
  }

  getProgress() {
    console.log(this.status);
    if (this.status
      && this.status.state === "running"
      && this.status.totalTransfers > 0
      && this.status.completedTransfers <= this.status.totalTransfers) {
        return (this.status.completedTransfers / this.status.totalTransfers) * 100;
    }
    return 0;
  }
}