import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { ClothingItem } from './clothing.service';
import { environment } from '../../environments/environment';

export interface CartItem {
  productId: string;
  title: string;
  price: number;
  quantity: number;
  size?: string;
  imageUrl: string;
}

export interface CartOrderRequest {
  customerName: string;
  city: string;
  deliveryPlace: string;
  phone: string;
  deliveryAddress: string;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly storageKey = 'twins-fashion-cart';
  private readonly itemsSubject = new BehaviorSubject<CartItem[]>(this.readFromStorage());
  readonly items$ = this.itemsSubject.asObservable();
  readonly totalQuantity$ = this.items$.pipe(map(items => items.reduce((sum, item) => sum + item.quantity, 0)));

  constructor(private http: HttpClient) {}

  private readFromStorage(): CartItem[] {
    try {
      const raw = localStorage.getItem(this.storageKey);
      return raw ? (JSON.parse(raw) as CartItem[]) : [];
    } catch (error) {
      console.warn('Неуспешно зареждане на количката от localStorage', error);
      return [];
    }
  }

  private persist(items: CartItem[]): void {
    try {
      localStorage.setItem(this.storageKey, JSON.stringify(items));
    } catch (error) {
      console.warn('Неуспешно записване на количката в localStorage', error);
    }
  }

  private setItems(items: CartItem[]): void {
    this.itemsSubject.next(items);
    this.persist(items);
  }

  get items(): CartItem[] {
    return this.itemsSubject.value;
  }

  get totalQuantity(): number {
    return this.items.reduce((sum, item) => sum + item.quantity, 0);
  }

  addItem(product: ClothingItem, options: { quantity: number; size?: string }): void {
    const items = [...this.items];
    const keySize = options.size ?? undefined;
    const existing = items.find(item => item.productId === product.id && item.size === keySize);

    if (existing) {
      existing.quantity = Math.min(existing.quantity + options.quantity, 10);
    } else {
      items.push({
        productId: product.id,
        title: product.title,
        price: product.price,
        quantity: Math.max(1, Math.min(options.quantity, 10)),
        size: keySize,
        imageUrl: product.coverImageUrl
      });
    }

    this.setItems(items);
  }

  updateQuantity(productId: string, size: string | undefined, quantity: number): void {
    const items = this.items.map(item => {
      if (item.productId === productId && item.size === size) {
        return { ...item, quantity: Math.max(1, Math.min(10, quantity)) };
      }
      return item;
    });

    this.setItems(items);
  }

  removeItem(productId: string, size: string | undefined): void {
    const items = this.items.filter(item => !(item.productId === productId && item.size === size));
    this.setItems(items);
  }

  clear(): void {
    this.setItems([]);
  }

  getSubtotal(): number {
    return this.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
  }

  submitOrder(order: CartOrderRequest): Observable<unknown> {
    const payload = {
      customerName: order.customerName,
      city: order.city,
      deliveryPlace: order.deliveryPlace,
      phone: order.phone,
      deliveryAddress: order.deliveryAddress,
      items: this.items.map(item => ({
        productId: item.productId,
        title: item.title,
        price: item.price,
        quantity: item.quantity,
        size: item.size
      })),
      total: this.getSubtotal()
    };

    return this.http.post(`${environment.apiBaseUrl}/api/ShoppingBasket/order`, payload);
  }
}
