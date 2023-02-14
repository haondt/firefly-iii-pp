import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { AutoReconcileJoiningStrategyOptionsModel, AutoReconcileRequestOptionsModel } from '../models/AutoReconcileRequestOptions';
import { AutoReconcileDryRunResponseDto } from '../models/dtos/AutoReconcileDryRunResponse';
import { AutoReconcileRequestDto } from '../models/dtos/AutoReconcileRequest';
import { DryRunResponseDto } from '../models/dtos/DryRunResponse';
import { QueryOptionDto } from '../models/dtos/QueryOption';
import { RunnerStateDto } from '../models/dtos/RunnerState';
import { QueryOperationModel } from '../models/QueryOperation';
import { ServiceResponseModel } from '../models/ServiceResponse';
import { FireflyIIIService } from '../services/FireflyIII';
import { RunnerService } from '../services/Runner';

import requestOptions from '../../assets/autoReconcileRequestOptions.json';

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

  dryRunResponseDto: AutoReconcileDryRunResponseDto|null = null;

  constructor(private fireflyIIIService: FireflyIIIService,
        private snackBar: MatSnackBar) {
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
  }

  showSnackError(error?: string) {
    this.snackBar.open(error ?? "Error while executing the request", 'Dismiss', {
      duration: 5000
    });
  }

  dryRun() {
    console.log(this.requestDto);
  }

  formatAmount(amount: number) {
    return "TODO";
  }

  formatDate(date: string) {
    return "TODO";
  }

  fixDateMatchTolerance() {
    let v = this.requestDto.pairingStrategy.dateMatchToleranceInDays;
    v = Math.max(0, v);
    v = Math.min(14, v);
    v = Math.floor(v);
    this.requestDto.pairingStrategy.dateMatchToleranceInDays = v;
  }

}