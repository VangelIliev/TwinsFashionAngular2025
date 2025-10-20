import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ClothesComponent } from './clothes/clothes.component';
import { AboutComponent } from './about/about.component';
import { ContactComponent } from './contact/contact.component';
import { ClothingDetailsComponent } from './clothing-details/clothing-details.component';
import { ShoppingCartComponent } from './shopping-cart/shopping-cart.component';
import { ThankYouComponent } from './thank-you/thank-you.component';
import { AdminLoginComponent } from './admin/admin-login/admin-login.component';
import { AdminDashboardComponent } from './admin/admin-dashboard/admin-dashboard.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'clothes', component: ClothesComponent },
  { path: 'clothes/:id', component: ClothingDetailsComponent },
  { path: 'cart', component: ShoppingCartComponent },
  { path: 'about', component: AboutComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'thank-you', component: ThankYouComponent },
  { path: 'admin/login', component: AdminLoginComponent },
  { path: 'admin/dashboard', component: AdminDashboardComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'top' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
