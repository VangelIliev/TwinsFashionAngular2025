import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AdminService, AdminProduct, Category, SubCategory, Color, Size } from '../../services/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  products: AdminProduct[] = [];
  categories: Category[] = [];
  subCategories: SubCategory[] = [];
  colors: Color[] = [];
  sizes: Size[] = [];
  
  loading = true;
  showAddProductModal = false;
  showAddCategoryModal = false;
  showAddSubCategoryModal = false;
  showAddColorModal = false;
  showAddSizeModal = false;
  showSetCoverModal = false;
  
  selectedProduct: AdminProduct | null = null;
  selectedProductForCover: AdminProduct | null = null;
  selectedCoverImageId: string | null = null;
  
  // Form data
  newProduct = {
    name: '',
    description: '',
    price: 0,
    quantity: 1,
    categoryId: '',
    subCategoryId: '',
    colorId: '',
    imageUrls: [] as string[],
    sizeIds: [] as string[]
  };
  
  newCategory = { name: '' };
  newSubCategory = { categoryId: '', name: '' };
  newColor = { name: '' };
  newSize = { type: '', size: '' };
  newImageUrl = '';

  constructor(
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.checkAuthAndLoadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private checkAuthAndLoadData(): void {
    this.adminService.checkAuth()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (authStatus) => {
          if (!authStatus.isAuthenticated) {
            this.router.navigate(['/admin/login']);
            return;
          }
          this.loadAllData();
        },
        error: () => {
          this.router.navigate(['/admin/login']);
        }
      });
  }

  private loadAllData(): void {
    this.loading = true;
    
    // Load all data in parallel
    this.adminService.getProducts().subscribe({
      next: (products) => {
        console.log('Products loaded:', products);
        products.forEach(product => {
          console.log(`Product: ${product.name}, Category: ${product.category}, Category type: ${typeof product.category}`);
        });
        this.products = products;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.loading = false;
      }
    });

    this.adminService.getCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: (error) => console.error('Error loading categories:', error)
    });

    this.adminService.getSubCategories().subscribe({
      next: (subCategories) => this.subCategories = subCategories,
      error: (error) => console.error('Error loading subcategories:', error)
    });

    this.adminService.getColors().subscribe({
      next: (colors) => this.colors = colors,
      error: (error) => console.error('Error loading colors:', error)
    });

    this.adminService.getSizes().subscribe({
      next: (sizes) => this.sizes = sizes,
      error: (error) => console.error('Error loading sizes:', error)
    });
  }

  logout(): void {
    this.adminService.logout().subscribe({
      next: () => {
        this.router.navigate(['/admin/login']);
      }
    });
  }

  // Product management
  openAddProductModal(): void {
    this.showAddProductModal = true;
    this.resetProductForm();
  }

  closeAddProductModal(): void {
    this.showAddProductModal = false;
    this.resetProductForm();
  }

  addImageUrl(): void {
    if (this.newImageUrl.trim()) {
      this.newProduct.imageUrls.push(this.newImageUrl.trim());
      this.newImageUrl = '';
    }
  }

  removeImageUrl(index: number): void {
    this.newProduct.imageUrls.splice(index, 1);
  }

  addProduct(): void {
    if (!this.validateProductForm()) return;

    this.adminService.addProduct(this.newProduct).subscribe({
      next: () => {
        this.closeAddProductModal();
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error adding product:', error);
      }
    });
  }

  deleteProduct(product: AdminProduct): void {
    if (confirm(`Сигурни ли сте, че искате да изтриете "${product.name}"?`)) {
      this.adminService.deleteProduct(product.id).subscribe({
        next: () => {
          this.loadAllData();
        },
        error: (error) => {
          console.error('Error deleting product:', error);
        }
      });
    }
  }

  // Category management
  openAddCategoryModal(): void {
    this.showAddCategoryModal = true;
    this.newCategory = { name: '' };
  }

  closeAddCategoryModal(): void {
    this.showAddCategoryModal = false;
  }

  addCategory(): void {
    if (!this.newCategory.name.trim()) return;

    this.adminService.addCategory(this.newCategory).subscribe({
      next: () => {
        this.closeAddCategoryModal();
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error adding category:', error);
      }
    });
  }

  // SubCategory management
  openAddSubCategoryModal(): void {
    this.showAddSubCategoryModal = true;
    this.newSubCategory = { categoryId: '', name: '' };
  }

  closeAddSubCategoryModal(): void {
    this.showAddSubCategoryModal = false;
  }

  addSubCategory(): void {
    if (!this.newSubCategory.name.trim() || !this.newSubCategory.categoryId) return;

    this.adminService.addSubCategory(this.newSubCategory).subscribe({
      next: () => {
        this.closeAddSubCategoryModal();
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error adding subcategory:', error);
      }
    });
  }

  // Color management
  openAddColorModal(): void {
    this.showAddColorModal = true;
    this.newColor = { name: '' };
  }

  closeAddColorModal(): void {
    this.showAddColorModal = false;
  }

  addColor(): void {
    if (!this.newColor.name.trim()) return;

    this.adminService.addColor(this.newColor).subscribe({
      next: () => {
        this.closeAddColorModal();
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error adding color:', error);
      }
    });
  }

  // Size management
  openAddSizeModal(): void {
    this.showAddSizeModal = true;
    this.newSize = { type: '', size: '' };
  }

  closeAddSizeModal(): void {
    this.showAddSizeModal = false;
  }

  addSize(): void {
    if (!this.newSize.type.trim() || !this.newSize.size.trim()) return;

    this.adminService.addSize(this.newSize).subscribe({
      next: () => {
        this.closeAddSizeModal();
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error adding size:', error);
      }
    });
  }

  // Cover image management
  openSetCoverModal(product: AdminProduct): void {
    this.selectedProductForCover = product;
    this.showSetCoverModal = true;
    this.selectedCoverImageId = null;
  }

  closeSetCoverModal(): void {
    this.showSetCoverModal = false;
    this.selectedProductForCover = null;
    this.selectedCoverImageId = null;
  }

  setCoverImage(): void {
    if (!this.selectedProductForCover || !this.selectedCoverImageId) {
      alert('Моля изберете снимка');
      return;
    }

    this.adminService.setCoverImage(this.selectedProductForCover.id, {
      imageId: this.selectedCoverImageId
    }).subscribe({
      next: () => {
        console.log('Cover image set successfully');
        this.closeSetCoverModal();
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error setting cover image:', error);
        alert('Грешка при задаване на cover снимката');
      }
    });
  }

  // Helper methods
  private resetProductForm(): void {
    this.newProduct = {
      name: '',
      description: '',
      price: 0,
      quantity: 1,
      categoryId: '',
      subCategoryId: '',
      colorId: '',
      imageUrls: [],
      sizeIds: []
    };
  }

  private validateProductForm(): boolean {
    return !!(
      this.newProduct.name.trim() &&
      this.newProduct.description.trim() &&
      this.newProduct.price > 0 &&
      this.newProduct.quantity > 0 &&
      this.newProduct.categoryId &&
      this.newProduct.subCategoryId &&
      this.newProduct.colorId &&
      this.newProduct.imageUrls.length > 0
    );
  }
}
