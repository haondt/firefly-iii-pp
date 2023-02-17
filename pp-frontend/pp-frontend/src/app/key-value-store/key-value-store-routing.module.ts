import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { KeyValueStoreComponent } from './key-value-store.component';

const routes: Routes = [
    {path: '', component: KeyValueStoreComponent}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class KeyValueStoreRoutingModule { }