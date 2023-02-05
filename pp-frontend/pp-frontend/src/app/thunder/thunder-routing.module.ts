import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { ThunderComponent } from "./thunder.component";

const routes: Routes = [
    {path: '', component: ThunderComponent}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ThunderRoutingModule { }