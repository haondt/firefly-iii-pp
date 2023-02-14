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
      requireMatchingDates: false
    },
    joiningStrategy: {
      descriptionJoinStrategy: null,
      dateJoinStrategy: null,
      categoryJoinStrategy: null,
      notesJoinStrategy: null
    }
  };

  requestOptions: AutoReconcileRequestOptionsModel = {
    joiningStrategyOptions: {
      descriptionJoinStrategyOptions: [],
      dateJoinStrategyOptions: [],
      categoryJoinStrategyOptions: [],
      notesJoinStrategyOptions: []
    }
  };

  dryRunResponseDto: AutoReconcileDryRunResponseDto|null = null;

  constructor(private fireflyIIIService: FireflyIIIService,
        private snackBar: MatSnackBar) {
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
  }

  initData() {
  }

  showSnackError(error?: string) {
    this.snackBar.open(error ?? "Error while executing the request", 'Dismiss', {
      duration: 5000
    });
  }

  dryRun() {
  }

  formatAmount(amount: number) {
    return "TODO";
  }

  formatDate(date: string) {
    return "TODO";
  }

}