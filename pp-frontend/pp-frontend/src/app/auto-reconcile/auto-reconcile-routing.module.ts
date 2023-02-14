import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { AutoReconcileComponent } from './auto-reconcile.component';

const routes: Routes = [
    {path: '', component: AutoReconcileComponent}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AutoReconcileRoutingModule { }