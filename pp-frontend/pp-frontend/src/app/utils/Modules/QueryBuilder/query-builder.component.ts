import { Component } from '@angular/core';
import { QueryOptionDto } from 'src/app/models/dtos/QueryOption';
import { QueryOperationModel } from 'src/app/models/QueryOperation';
import { FireflyIIIService } from 'src/app/services/FireflyIII';
import { Input, EventEmitter } from '@angular/core';

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
  selector: 'app-query-builder',
  templateUrl: './query-builder.component.html',
  styleUrls: ['./query-builder.component.scss'],
})
export class QueryBuilderComponent {
  queryOptions: QueryOptionDto[] = [];

  queryOperand: QueryOptionDto | null = null;
  queryOperatorOptions: QueryOperatorModel[] = [];
  queryOperator: QueryOperatorModel | null = null;
  queryResult: any;
  @Input() queryOperations: QueryOperationWrapper[] = [];

  constructor(private fireflyIIIService: FireflyIIIService) {
    this.fireflyIIIService.getQueryOptions().subscribe(res => {
        if (res.success) {
          this.queryOptions = res.body!;
        } else {
          this.queryOptions = [];
          throw new Error(res.error);
        }
    });
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

}