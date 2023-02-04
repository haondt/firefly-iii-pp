import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ThunderComponent } from './thunder/thunder.component';

const routes: Routes = [
  { path: '', redirectTo: 'firefly-iii', pathMatch: 'full'},
  { path: 'thunder', component: ThunderComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
