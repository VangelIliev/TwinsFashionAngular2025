import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {
  menuOpen = false;
  cartCount = 0;

  private subscription?: Subscription;

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.subscription = this.cartService.items$.subscribe(() => {
      this.cartCount = this.cartService.totalQuantity;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
