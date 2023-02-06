import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NodeRedRoutingModule } from './node-red-routing.module';
import { NodeRedComponent } from './node-red.component';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextFieldModule } from '@angular/cdk/text-field';
import { MatSnackBarModule } from '@angular/material/snack-bar';


@NgModule({
    imports: [
        CommonModule,
        NodeRedRoutingModule,
        MatDividerModule,
        MatCardModule,
        MatSnackBarModule,
        MatButtonModule,
        FormsModule,
        ReactiveFormsModule,
        TextFieldModule
    ],
    declarations: [
        NodeRedComponent
    ]
})
export class NodeRedModule { }
