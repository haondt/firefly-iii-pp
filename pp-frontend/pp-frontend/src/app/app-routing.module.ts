import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestsComponent } from './tests/tests.component';
import { ThunderComponent } from './thunder/thunder.component';

const routes: Routes = [
  { path: '', redirectTo: 'firefly-iii', pathMatch: 'full'},
  { path: 'tests', component: TestsComponent},
  { path: 'thunder', component: ThunderComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
