import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';

const routes: Routes = [
  { path: 'signin-oidc', component: AppComponent }, // Route for handling the redirect 
  { path: 'signout-oidc', component: AppComponent }, // Route for handling the redirect
  { path: '', component: AppComponent }, // Default route
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
