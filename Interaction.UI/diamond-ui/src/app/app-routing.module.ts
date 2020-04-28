import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RpsComponent } from './games/rps/rps.component';


const routes: Routes = [
  {
    path: 'rps',
    component: RpsComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
