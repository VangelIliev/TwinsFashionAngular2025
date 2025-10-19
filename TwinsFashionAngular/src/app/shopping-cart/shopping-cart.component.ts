import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { CartService, CartItem, CartOrderRequest } from '../services/cart.service';

@Component({
  selector: 'app-shopping-cart',
  standalone: false,
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.css']
})
export class ShoppingCartComponent implements OnInit, OnDestroy {
  items: CartItem[] = [];
  order: CartOrderRequest = {
    customerName: '',
    city: '',
    deliveryPlace: '',
    phone: '',
    deliveryAddress: ''
  };
  orderSubmitting = false;
  orderSuccess = false;
  orderError = false;

  private subscription?: Subscription;

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.subscription = this.cartService.items$.subscribe(items => {
      this.items = items;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  increment(item: CartItem): void {
    this.cartService.updateQuantity(item.productId, item.size, item.quantity + 1);
  }

  decrement(item: CartItem): void {
    this.cartService.updateQuantity(item.productId, item.size, item.quantity - 1);
  }

  remove(item: CartItem): void {
    this.cartService.removeItem(item.productId, item.size);
  }

  submitOrder(): void {
    if (this.orderSubmitting || !this.hasItems) {
      return;
    }

    if (!this.order.customerName || !this.order.city || !this.order.deliveryPlace || !this.order.phone) {
      this.orderError = true;
      this.orderSuccess = false;
      return;
    }

    this.orderSubmitting = true;
    this.orderError = false;
    this.orderSuccess = false;

    this.cartService.submitOrder(this.order).subscribe({
      next: () => {
        this.orderSubmitting = false;
        this.orderSuccess = true;
        this.cartService.clear();
      },
      error: () => {
        this.orderSubmitting = false;
        this.orderError = true;
      }
    });
  }

  get subtotal(): number {
    return this.cartService.getSubtotal();
  }

  get total(): number {
    return this.subtotal;
  }

  get hasItems(): boolean {
    return this.items.length > 0;
  }
}
