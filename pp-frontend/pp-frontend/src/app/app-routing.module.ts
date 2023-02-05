import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: 'firefly-iii', pathMatch: 'full'},
  { path: 'thunder', loadChildren: () =>
    import('./thunder/thunder.module').then(m => m.ThunderModule)},
  { path: 'firefly-iii', loadChildren: () =>
    import('./firefly-iii-pp/firefly-iii-pp.module').then(m => m.FireflyIIIPPModule)}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
