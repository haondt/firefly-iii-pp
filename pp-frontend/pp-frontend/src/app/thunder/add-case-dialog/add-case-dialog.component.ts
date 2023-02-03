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
    title: string;
    folderNameOptions: string[];
}

@Component({
    selector: 'add-case-dialog',
    templateUrl: './add-case-dialog.component.html',
    styleUrls: [
        './add-case-dialog.component.scss'
    ]
})
export class AddCaseDialog {
    fields: { viewValue: string, selected: boolean }[] = [];
    transactionId: string | null = null;
    transactionData: { [key: string]: any } | null = null;
    working: boolean = false;
    gettingTransaction: boolean = false;
    getTransactionError: string = "";

    folderName: string | undefined;
    allFolderNameOptions: string[] = [];
    filteredFolderNameOptions: string[] = [];

    folderCreationOption: string = "use-existing-or-create";

    selectedField: string | undefined;
    expectedFieldValue: string | undefined;

    constructor(
        public dialogRef: MatDialogRef<AddCaseDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private fireflyIII: FireflyIIIService
    ) {
        dialogRef.disableClose = true;
        this.allFolderNameOptions = data.folderNameOptions;
        console.log(this.allFolderNameOptions);
    }

    getTransactionData() {
        if (this.transactionId !== null) {
            // reset data
            this.getTransactionError = "";
            this.transactionData = null;
            this.fields = [];
            this.folderCreationOption = "use-existing-or-create";

            // freeze ui
            this.working = true;
            this.gettingTransaction = true;

            this.fireflyIII.getTransactionData(this.transactionId).subscribe(r => {
                try {
                    if (r.success) {
                        this.transactionData = r.body!;
                        this.loadFields();
                        this.working = false;
                    } else {
                        this.getTransactionError = r.error ?? "Unable to retrieve transaction";
                    }
                } finally {
                    // unfreeze ui
                    this.gettingTransaction = false;
                    this.working = false;
                }
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

    onCloseClick() {
        this.dialogRef.close()
    }

    onFinishClick() {
        if (this.fields !== null) {
            this.dialogRef.close(this.fields.filter(f => f.selected).map(f => {
                return {
                    key: f.viewValue,
                    value: this.transactionData![f.viewValue]
                };
            }));
        }
    }

    addExpectedValue() {

    }
}