import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { NodeRedComponent } from './node-red.component';

const routes: Routes = [
    {path: '', component: NodeRedComponent}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class NodeRedRoutingModule { }