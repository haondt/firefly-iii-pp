import { Component, Inject } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";
import { MatChipInputEvent } from "@angular/material/chips";
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { map, Observable, startWith } from "rxjs";
import { A, C, COMMA, ENTER } from '@angular/cdk/keycodes';
import { FireflyIIIService } from "src/app/services/FireflyIII";
import { MatSelectChange } from "@angular/material/select";

export interface DialogData {
    obj: Object
}

@Component({
    selector: 'edit-json-dialog',
    templateUrl: './edit-json-dialog.component.html',
    styleUrls: [
        '../tests.component.scss',
        './edit-json-dialog.component.scss'
    ]
})
export class EditJsonDialog {
    text: string = "";
    constructor(
        public dialogRef: MatDialogRef<EditJsonDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData
    ) {
        this.text = JSON.stringify(data.obj, null, 4);
    }

    onFinishClick() {
        if (this.isValid(this.text)) {
            this.dialogRef.close({
                success: true,
                value: JSON.parse(this.text)
            });
        }
    }

    isValid(data: string) {
        try {
            var o = JSON.parse(data);
            return true;
        } catch {}
        return false;
    }
}