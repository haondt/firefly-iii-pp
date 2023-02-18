import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { KeyValueStoreRoutingModule } from './key-value-store-routing.module';
import { KeyValueStoreComponent } from './key-value-store.component';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextFieldModule } from '@angular/cdk/text-field';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';

import { QueryBuilderModule } from '../utils/Modules/QueryBuilder/query-builder.module';

@NgModule({
    imports: [
        CommonModule,
        KeyValueStoreRoutingModule,
        MatSnackBarModule,
        MatDividerModule,
        MatButtonModule,
        FormsModule,
        MatCardModule,
        ReactiveFormsModule,
        QueryBuilderModule,
        MatFormFieldModule,
        MatInputModule,
        MatAutocompleteModule,
        MatSelectModule,
        MatIconModule,
        TextFieldModule
    ],
    declarations: [
        KeyValueStoreComponent
    ]
})
export class KeyValueStoreModule { }
