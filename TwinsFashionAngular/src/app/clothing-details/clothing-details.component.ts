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
  private productSubscription?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private clothingService: ClothingService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subscription = this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (!id) {
        this.router.navigate(['/clothes']);
        return;
      }

      this.loading = true;

      this.productSubscription?.unsubscribe();
      this.productSubscription = this.clothingService.getById(id).subscribe(product => {
        if (!product) {
          this.router.navigate(['/clothes']);
          return;
        }

        this.product = product;
        // Set selected image to cover image if available, otherwise first image
        const coverImageIndex = product.gallery.findIndex(img => img.isCover);
        this.selectedImageIndex = coverImageIndex >= 0 ? coverImageIndex : 0;
        this.selectedSize = product.sizes.find(size => size.inStock);
        this.quantity = 1;
        this.loading = false;
      });
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.productSubscription?.unsubscribe();
  }

  get gallery() {
    return this.product?.gallery ?? [];
  }

  get selectedImageUrl(): string {
    return this.gallery[this.selectedImageIndex]?.url ?? this.product?.coverImageUrl ?? '';
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
      size: this.selectedSize ? this.selectedSize.value.size : undefined
    });

    this.router.navigate(['/cart']);
  }
}
