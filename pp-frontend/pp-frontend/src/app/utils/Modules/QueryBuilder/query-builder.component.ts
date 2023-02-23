import { Component, Input } from '@angular/core';
import { QueryOptionDto } from 'src/app/models/dtos/QueryOption';
import { QueryOperationModel } from 'src/app/models/QueryOperation';
import { FireflyIIIService } from 'src/app/services/FireflyIII';
import queryOptionsJson from '../../../../assets/queryOptions.json';
import { CurrencyPipe } from '@angular/common';

interface QueryOperatorModel {
  viewValue: string,
  operator: string,
  type: string,
  options?: {
    result: string,
    viewValue: string
  }[]
}

interface QueryOperationWrapper {
  viewValue: string,
  queryOperation: QueryOperationModel
}

@Component({
  selector: 'app-query-builder',
  templateUrl: './query-builder.component.html',
  styleUrls: ['./query-builder.component.scss'],
})
export class QueryBuilderComponent {
  queryOptions: QueryOptionDto[] = Object.assign([], queryOptionsJson);

  queryOperand: QueryOptionDto | null = null;
  queryOperatorOptions: QueryOperatorModel[] = [];
  queryOperator: QueryOperatorModel | null = null;
  queryResult: any;
  @Input() queryOperations: QueryOperationWrapper[] = [];

  formattedAmount: any;

  constructor(private fireflyIIIService: FireflyIIIService,
    private currencyPipe: CurrencyPipe) {
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
      } else if (this.queryOperator.type === "currency") {
        operation.queryOperation.result = (<string>this.queryResult).replace(/[^0-9.]/g, '');
        operation.viewValue += " " + (<string>this.queryResult);
        this.queryOperations.push(operation);
      } else if (this.queryOperator.type === "select") {
        operation.queryOperation.result = this.queryResult.result;
        operation.viewValue += " " + this.queryResult.viewValue;
        this.queryOperations.push(operation);
      } else {
        throw new Error(`Unable to add query operator type ${this.queryOperator.type}`);
      }

    }
  }

  removeQueryOperation(event: QueryOperationWrapper) {
    const i = this.queryOperations.indexOf(event);
    if (i >= 0) {
      this.queryOperations.splice(i, 1);
    }
  }

  transformAmount(element: any) {
    try {
      this.queryResult = (<string>this.queryResult).replace(/[^0-9.]/g, '');
      this.queryResult = this.currencyPipe.transform(this.queryResult, "$");
    } catch {
      this.queryResult = null;
    }
    element.target.value = this.queryResult;
  }

}