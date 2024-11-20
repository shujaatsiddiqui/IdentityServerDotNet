import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';

const routes: Routes = [
  { path: 'signin-oidc', redirectTo:'' }, // Route for handling the redirect 
  { path: 'signout-callback-oidc', redirectTo:''  }, // Route for handling the redirect
  { path: 'signout-oidc', redirectTo:''  }, // Route for handling the redirect
  { path: '', component: AppComponent }, // Default route
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
