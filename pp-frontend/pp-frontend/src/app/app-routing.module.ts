import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestsComponent } from './tests/tests.component';

const routes: Routes = [
  { path: '', redirectTo: 'firefly-iii', pathMatch: 'full'},
  { path: 'tests', component: TestsComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
