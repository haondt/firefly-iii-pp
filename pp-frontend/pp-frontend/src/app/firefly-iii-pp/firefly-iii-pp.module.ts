import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FireflyIIIPPRoutingModule } from './firefly-iii-pp-routing.module';
import { FireflyIIIPPComponent } from './firefly-iii-pp.component';

import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatInputModule } from '@angular/material/input';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';

@NgModule({
    imports: [
        CommonModule,
        FireflyIIIPPRoutingModule,
        MatSnackBarModule,
        MatDividerModule,
        MatTooltipModule,
        MatProgressSpinnerModule,
        MatInputModule,
        MatButtonModule,
        MatProgressBarModule,
        MatExpansionModule,
        MatNativeDateModule,
        MatDatepickerModule,
        MatRadioModule,
        MatChipsModule,
        MatIconModule,
        FormsModule,
        MatSelectModule,
        MatFormFieldModule,
        ReactiveFormsModule,
    ],
    declarations: [
        FireflyIIIPPComponent,
    ]
})
export class FireflyIIIPPModule { }
