import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ClothingItem, ClothingService, ClothingSizeOption } from '../services/clothing.service';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-clothing-details',
  standalone: false,
  templateUrl: './clothing-details.component.html',
  styleUrls: ['./clothing-details.component.css']
})
export class ClothingDetailsComponent implements OnInit, OnDestroy {
  product?: ClothingItem;
  selectedImageIndex = 0;
  selectedSize?: ClothingSizeOption;
  quantity = 1;
  loading = true;

  private subscription?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private clothingService: ClothingService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subscription = this.route.paramMap.subscribe(params => {
      const slug = params.get('slug');
      if (!slug) {
        this.router.navigate(['/clothes']);
        return;
      }

      const product = this.clothingService.getBySlug(slug);
      if (!product) {
        this.router.navigate(['/clothes']);
        return;
      }

      this.product = product;
      this.selectedImageIndex = 0;
      this.selectedSize = product.sizes.find(size => size.inStock);
      this.quantity = 1;
      this.loading = false;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  get gallery() {
    return this.product?.gallery ?? [];
  }

  get selectedImageUrl(): string {
    return this.gallery[this.selectedImageIndex]?.url ?? '';
  }

  selectImage(index: number): void {
    this.selectedImageIndex = index;
  }

  updateQuantity(value: string): void {
    const parsed = Number(value);
    this.quantity = Number.isNaN(parsed) ? 1 : Math.max(1, Math.min(10, parsed));
  }

  addToCart(): void {
    if (!this.product) {
      return;
    }

    this.cartService.addItem(this.product, {
      quantity: this.quantity,
      size: this.selectedSize?.value
    });

    this.router.navigate(['/cart']);
  }
}
