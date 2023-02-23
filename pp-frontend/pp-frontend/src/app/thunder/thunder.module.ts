import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AddCaseComponent } from './add-case-dialog/add-case-dialog.component';
import { ThunderRoutingModule } from './thunder-routing.module';
import { ThunderComponent } from './thunder.component';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSelectModule } from '@angular/material/select';

@NgModule({
    imports: [
        CommonModule,
        ThunderRoutingModule,
        MatProgressSpinnerModule,
        MatChipsModule,
        MatFormFieldModule,
        MatAutocompleteModule,
        MatIconModule,
        MatRadioModule,
        MatCheckboxModule,
        MatSelectModule,
        FormsModule,
        ReactiveFormsModule,
        MatCardModule,
        MatSnackBarModule,
        MatButtonModule,
        MatInputModule,
        MatDividerModule,
        MatTooltipModule
    ],
    declarations: [
        ThunderComponent,
        AddCaseComponent
    ]
})
export class ThunderModule { }
