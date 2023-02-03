import { Component, Inject } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { MatChipInputEvent } from "@angular/material/chips";
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { filter, map, Observable, startWith } from "rxjs";
import { A, COMMA, ENTER } from '@angular/cdk/keycodes';
import { FireflyIIIService } from "src/app/services/FireflyIII";
import { MatSelectChange } from "@angular/material/select";

export interface DialogData {
    title: string;
    folderNameOptions: string[];
}

interface CaseFieldChip {
    selected: boolean;
    viewValue: string;
}

@Component({
    selector: 'add-case-dialog',
    templateUrl: './add-case-dialog.component.html',
    styleUrls: [
        './add-case-dialog.component.scss'
    ]
})
export class AddCaseDialog {
    fields: CaseFieldChip[] = [];
    transactionId: string | null = null;
    transactionData: { [key: string]: any } | null = null;
    working: boolean = false;
    gettingTransaction: boolean = false;
    getTransactionError: string = "";

    folderName: string | undefined;
    allFolderNameOptions: string[] = [];
    filteredFolderNameOptions: string[] = [];

    folderCreationOption: string = "use-existing-or-create";
    configureExpectedValues: boolean = false;

    selectedField: string | undefined;
    expectedFieldValue: string | undefined;

    createCaseError: string | undefined;

    constructor(
        public dialogRef: MatDialogRef<AddCaseDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private fireflyIII: FireflyIIIService
    ) {
        dialogRef.disableClose = true;
        this.allFolderNameOptions = data.folderNameOptions;
        this.filteredFolderNameOptions = this.allFolderNameOptions;
    }

    getTransactionData() {
        if (this.transactionId !== null) {
            // reset data
            this.getTransactionError = "";
            this.transactionData = null;
            this.fields = [];
            this.folderCreationOption = "use-existing-or-create";
            this.configureExpectedValues = false;
            this.folderName = undefined;
            this.createCaseError = undefined;
            this.filteredFolderNameOptions = this.allFolderNameOptions;

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

    ______onFinishClick() {
        if (this.fields !== null) {
            this.dialogRef.close(this.fields.filter(f => f.selected).map(f => {
                return {
                    key: f.viewValue,
                    value: this.transactionData![f.viewValue]
                };
            }));
        }
    }

    onCaseFieldClick(chip: CaseFieldChip) {
        chip.selected = !chip.selected;
    }

    addExpectedValue() {

    }

    createCase(button: { disabled: boolean }) {
        if (!this.transactionData) {
            this.createCaseError = "No transaction loaded";
        } else if (this.fields.filter(f => f.selected).length <= 0) {
            this.createCaseError = "No fields selected";
        } else if (!this.folderName) {
            this.createCaseError = "Folder name not set";
        } else {
            this.createCaseError = undefined;

            button.disabled = true;
            button.disabled = false;
        }
    }

    folderNameAutocompleteChanged(name: string) {
        this.folderName = name;
        const filterValue = name.toLowerCase();
        this.filteredFolderNameOptions = this.allFolderNameOptions.filter(f => f.toLowerCase().includes(filterValue));
        console.log(this.folderName);
    }
}