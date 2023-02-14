import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { QueryBuilderComponent } from './query-builder.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatIconModule,
        MatSelectModule,
        MatChipsModule,
        MatInputModule,
        MatButtonModule
    ],
    declarations: [
        QueryBuilderComponent
    ],
    exports: [
        QueryBuilderComponent
    ]
})
export class QueryBuilderModule { }