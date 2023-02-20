import { Component, Inject } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { MatChipInputEvent } from "@angular/material/chips";
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { filter, map, Observable, startWith } from "rxjs";
import { A, COMMA, ENTER } from '@angular/cdk/keycodes';
import { FireflyIIIService } from "src/app/services/FireflyIII";
import { MatSelectChange } from "@angular/material/select";
import { ThunderService } from "src/app/services/Thunder";
import { dict } from "../../utils/ArrayUtils";
import { KeyValueStoreService } from "src/app/services/KeyValueStore";
import { checkResult } from "src/app/utils/ObservableUtils";
import { NodeRedService } from "src/app/services/NodeRed";

export interface DialogData {
    title: string;
    folderNameOptions: string[];
}

interface CaseFieldChip {
    selected: boolean;
    viewValue: string;
}

interface expectedField {
    key: string;
    value: string;
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

    selectedExpectedField: string | undefined;
    expectedFieldValue: string | undefined;
    expectedFields: expectedField[] = [];

    caseFields: {[key: string]: object} = {};

    createCaseError: string | undefined;
    creatingCase: boolean = false;
    caseCreated: boolean = false;
    createdCase: string | undefined;

    createdKvp: boolean  = false;
    createdKvpError: string | undefined;
    createKvpStoreOptions: string[] = [];
    createKvpStore: string | undefined;
    createKvpKey: string | undefined;
    createKvpAutocompleteValue: string | undefined;
    createKvpAutocompleteOptions: string[] = [];
    createKvpField: string | undefined;

    constructor(
        public dialogRef: MatDialogRef<AddCaseDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private fireflyIII: FireflyIIIService,
        private thunder: ThunderService,
        private kvs: KeyValueStoreService,
        private nr: NodeRedService
    ) {
        dialogRef.disableClose = true;
        this.allFolderNameOptions = data.folderNameOptions;
        this.filteredFolderNameOptions = this.allFolderNameOptions;
        this._clearCreateKvpData(true, false);

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
            this.expectedFields = [];
            this.selectedExpectedField = undefined;
            this.expectedFieldValue = undefined;
            this.caseCreated = false;
            this.createdCase = undefined;

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

    onCaseFieldClick(chip: CaseFieldChip) {
        chip.selected = !chip.selected;
        if (chip.selected) {
            this.caseFields[chip.viewValue] = this.transactionData![chip.viewValue];
        } else {
            if (chip.viewValue in this.caseFields) {
                delete this.caseFields[chip.viewValue];
            }
        }
    }

    loadExpectedValueFromTransaction() {
        if (this.selectedExpectedField && this.transactionData) {
            if (this.selectedExpectedField in this.transactionData) {
                this.expectedFieldValue = this.transactionData[this.selectedExpectedField];
            }
        }
    }

    expectedFieldValueChanged() {
        this.expectedFieldValue = undefined;
    }

    removeExpectedFieldChip(key: string) {
        this.expectedFields = this.expectedFields.filter(f => f.key !== key);
    }

    addExpectedValue() {
        if (this.selectedExpectedField && this.expectedFieldValue) {
            for(let field of this.expectedFields) {
                if (field.key === this.selectedExpectedField) {
                    field.value = this.expectedFieldValue;
                    return;
                }
            }
            this.expectedFields.push({
                key: this.selectedExpectedField,
                value: this.expectedFieldValue
            });
        }
    }

    createCase() {
        this.caseCreated = false;
        if (!this.transactionData) {
            this.createCaseError = "No transaction loaded";
        } else if (this.fields.filter(f => f.selected).length <= 0) {
            this.createCaseError = "No fields selected";
        } else if (!this.folderName) {
            this.createCaseError = "Folder name not set";
        } else {
            this.createCaseError = undefined;
            const body = {
                bodyFields: dict(this.fields.filter(f => f.selected).map(f => { return { key: f.viewValue, value: this.transactionData![f.viewValue]};})),
                folderName: this.folderName,
                createFolderMode: this.folderCreationOption,
                configureExpectedValues: this.configureExpectedValues,
                expectedValues: this.configureExpectedValues
                    ? dict(this.expectedFields)
                    : {}
            };

            this.working = true;
            this.creatingCase = true;

            this.thunder.createCase(body).subscribe(r => {
                try {
                    if (r.success) {
                        this.caseCreated = true;
                        this.createdCase = r.body.client.name;
                    } else {
                        this.createCaseError = r.error ?? "Error while creating case";
                    }
                } finally {
                    this.working = false;
                    this.creatingCase = false;
                }
            });
        }
    }

    folderNameAutocompleteChanged(name: string) {
        this.folderName = name;
        const filterValue = name.toLowerCase();
        this.filteredFolderNameOptions = this.allFolderNameOptions.filter(f => f.toLowerCase().includes(filterValue));
    }

    // kvp

    _clearCreateKvpData(unselectStore: boolean, forcePullAutocompleteOptions: boolean) {
        this.createdKvp = false;
        this.createdKvpError = undefined;
        if (unselectStore) {
            this.createKvpStore = undefined;
            this.createKvpStoreOptions = [];
            this.kvs.getStores().subscribe(checkResult<string[]>({
            success: s => this.createKvpStoreOptions = s,
            }));
        }
        this.createKvpKey = undefined;
        if (this.createKvpAutocompleteValue || forcePullAutocompleteOptions) {
            this.createKvpAutocompleteValue = undefined;
            this.kvs.autocomplete(this.createKvpStore!, "").subscribe(checkResult<string[]>({
                success: s => this.createKvpAutocompleteOptions = s,
                fail: e => this.createKvpAutocompleteOptions = []
            }));
        }
        this.createKvpField = undefined;
    }

    createKvpStoreChanged() {
        this._clearCreateKvpData(false, true);
    }

    createKvpAutocompleteValueChanged(value: string) {
        this.createKvpAutocompleteValue = value;
        this.kvs.autocomplete(this.createKvpStore!, value).subscribe(checkResult<string[]>({
            success: s => this.createKvpAutocompleteOptions = s,
            fail: e => this.createKvpAutocompleteOptions = []
        }));
    }

    createKvpExtractKey() {
        if(!this.createKvpField || !this.transactionData) {
            return;
        }

        this.nr.extractKey(this.createKvpField, JSON.stringify(this.transactionData)).subscribe(checkResult<string>({
            success: s => this.createKvpKey = s,
            fail: e => {
                this.createKvpKey = this.createKvpField ? this.transactionData?.[this.createKvpField] : "",
                this.createdKvpError = e
            }
        }));
    }
}