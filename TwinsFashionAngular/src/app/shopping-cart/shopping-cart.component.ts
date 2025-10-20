import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CartService, CartItem, CartOrderRequest } from '../services/cart.service';
import { Router } from '@angular/router';

const BULGARIAN_PHONE_REGEX = /^(\+359|0)(87|88|89|98)[0-9]{7}$/;
const NAME_REGEX = /^[А-Яа-яA-Za-z\s]+$/;
const CITY_REGEX = /^[А-Яа-яA-Za-z0-9\s]+$/;
const ADDRESS_REGEX = /^[А-Яа-яA-Za-z0-9\s.,-]+$/;

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
  validationErrors: Partial<Record<'customerName' | 'city' | 'deliveryPlace' | 'deliveryAddress' | 'phone', string>> = {};

  @ViewChild('orderForm') orderForm?: NgForm;

  private subscription?: Subscription;

  constructor(private cartService: CartService, private router: Router) {}

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

  onFieldBlur(field: 'customerName' | 'city' | 'deliveryPlace' | 'deliveryAddress' | 'phone'): void {
    this.validateOrder(field);
  }

  submitOrder(): void {
    if (this.orderSubmitting || !this.hasItems) {
      return;
    }

    if (!this.validateOrder()) {
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
        this.router.navigate(['/thank-you']);
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

  private validateOrder(field?: 'customerName' | 'city' | 'deliveryPlace' | 'deliveryAddress' | 'phone'): boolean {
    if (!field) {
      this.validationErrors = {};
    }

    const fields: Array<'customerName' | 'city' | 'deliveryPlace' | 'deliveryAddress' | 'phone'> = field
      ? [field]
      : ['customerName', 'city', 'deliveryPlace', 'deliveryAddress', 'phone'];

    for (const key of fields) {
      switch (key) {
        case 'customerName':
          if (!NAME_REGEX.test(this.order.customerName.trim())) {
            this.validationErrors.customerName = 'Моля, въведете валидно име без цифри и специални символи.';
          } else {
            delete this.validationErrors.customerName;
          }
          break;
        case 'city':
          if (!CITY_REGEX.test(this.order.city.trim())) {
            this.validationErrors.city = 'Градът не може да съдържа специални символи.';
          } else {
            delete this.validationErrors.city;
          }
          break;
        case 'deliveryPlace':
          if (!this.order.deliveryPlace) {
            this.validationErrors.deliveryPlace = 'Изберете място на доставка.';
          } else {
            delete this.validationErrors.deliveryPlace;
          }
          break;
        case 'deliveryAddress':
          if (!ADDRESS_REGEX.test(this.order.deliveryAddress.trim())) {
            this.validationErrors.deliveryAddress = 'Адресът трябва да съдържа само букви, цифри и ,.-';
          } else {
            delete this.validationErrors.deliveryAddress;
          }
          break;
        case 'phone':
          if (!BULGARIAN_PHONE_REGEX.test(this.order.phone.trim())) {
            this.validationErrors.phone = 'Въведете валиден български телефонен номер.';
          } else {
            delete this.validationErrors.phone;
          }
          break;
      }
    }

    return Object.keys(this.validationErrors).length === 0;
  }
}
