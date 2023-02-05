import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { FireflyIIIPPComponent } from './firefly-iii-pp.component';

const routes: Routes = [
    {path: '', component: FireflyIIIPPComponent}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class FireflyIIIPPRoutingModule { }