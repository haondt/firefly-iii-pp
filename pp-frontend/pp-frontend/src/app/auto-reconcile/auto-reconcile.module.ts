import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AutoReconcileRoutingModule } from './auto-reconcile-routing.module';
import { AutoReconcileComponent } from './auto-reconcile.component';

import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';

import { QueryBuilderModule } from '../utils/Modules/QueryBuilder/query-builder.module';

@NgModule({
    imports: [
        CommonModule,
        AutoReconcileRoutingModule,
        MatSnackBarModule,
        MatDividerModule,
        MatInputModule,
        MatButtonModule,
        MatNativeDateModule,
        MatDatepickerModule,
        MatCheckboxModule,
        MatIconModule,
        FormsModule,
        MatSelectModule,
        MatTooltipModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatExpansionModule,
        QueryBuilderModule,
        MatProgressSpinnerModule,
        MatProgressBarModule,
    ],
    declarations: [
        AutoReconcileComponent
    ]
})
export class AutoReconcileModule { }
