import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { ImageCardComponent } from './image-card/image-card.component';
import { HomeComponent } from './home/home.component';
import { ClothesComponent } from './clothes/clothes.component';
import { AboutComponent } from './about/about.component';
import { ContactComponent } from './contact/contact.component';
import { ClothingDetailsComponent } from './clothing-details/clothing-details.component';
import { ShoppingCartComponent } from './shopping-cart/shopping-cart.component';
import { ThankYouComponent } from './thank-you/thank-you.component';
import { DualCurrencyPipe } from './shared/dual-currency.pipe';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    ImageCardComponent,
    HomeComponent,
    ClothesComponent,
    AboutComponent,
    ContactComponent,
    ClothingDetailsComponent,
    ShoppingCartComponent,
    DualCurrencyPipe
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    ThankYouComponent
  ],
  providers: [DecimalPipe],
  bootstrap: [AppComponent]
})
export class AppModule { }
