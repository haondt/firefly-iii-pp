import { Component, Inject } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { MatChipInputEvent } from "@angular/material/chips";
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { map, Observable, startWith } from "rxjs";
import { A, COMMA, ENTER } from '@angular/cdk/keycodes';
import { FireflyIIIService } from "src/app/services/FireflyIII";
import { MatSelectChange } from "@angular/material/select";

export interface DialogData {

}

@Component({
    selector: 'transaction-checks-dialog',
    templateUrl: './transaction-checks-dialog.component.html',
    styleUrls: [
        '../tests.component.scss',
        './transaction-checks-dialog.component.scss'
    ]
})
export class TransactionChecksDialog {
    fields: { viewValue: string, selected: boolean }[] = [];
    transactionId: string | null = null;
    transactionData: { [key: string]: any } | null = null;
    working: boolean = false;


    constructor(
        public dialogRef: MatDialogRef<TransactionChecksDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private fireflyIII: FireflyIIIService
    ) {
    }

    getTransactionData() {
        this.fields = [];
        this.working = true;
        if (this.transactionId !== null) {
            this.fireflyIII.getTransactionData(this.transactionId).subscribe(r => {
                this.transactionData = r;
                this.loadFields();
                this.working = false;
            });
        }
    }

    loadFields() {
        this.fields = [];
        if (this.transactionData !== null) {
            for (let key of Object.keys(this.transactionData)) {
                let o = this.transactionData[key];
                if (o === null || typeof o === 'string') {
                    this.fields.push({viewValue: key, selected: false});
                }
            }
        }
    }

    onCancelClick() {
        this.dialogRef.close([])
    }

    onFinishClick() {
        if (this.fields !== null) {
            this.dialogRef.close(this.fields.filter(f => f.selected).map(f => {
                if (this.transactionData) { // ?? idk why the compiler hates me
                    return {
                        key: f.viewValue,
                        value: this.transactionData[f.viewValue]
                    };
                }
                throw new Error("Transaction data null");
            }));
        } else {
            this.dialogRef.close([]);
        }
    }
}