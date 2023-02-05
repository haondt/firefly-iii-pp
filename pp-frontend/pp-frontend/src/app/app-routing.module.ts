import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FireflyIIIPPComponent } from './firefly-iii-pp/firefly-iii-pp.component';
import { ThunderComponent } from './thunder/thunder.component';

const routes: Routes = [
  { path: '', redirectTo: 'firefly-iii', pathMatch: 'full'},
  { path: 'thunder', component: ThunderComponent},
  { path: 'firefly-iii', component: FireflyIIIPPComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
