import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AdminLoginRequest {
  username: string;
  password: string;
}

export interface AdminLoginResponse {
  message: string;
  username: string;
}

export interface AuthStatus {
  isAuthenticated: boolean;
  username?: string;
}

export interface AdminProduct {
  id: string;
  name: string;
  description: string;
  price: number;
  categoryId: string | null;
  category: string;
  subcategoryId: string | null;
  color: string;
  colorId: string | null;
  images: AdminImage[];
  coverImageUrl: string;
  sizes: string[];
  sizeIds: string[];
  subcategory: string;
  quantity: number;
}

export interface AdminImage {
  id: string;
  url: string;
  isCover: boolean;
}

export interface Category {
  id: string;
  name: string;
}

export interface SubCategory {
  id: string;
  name: string;
}

export interface Color {
  id: string;
  name: string;
}

export interface Size {
  id: string;
  type: string;
  size: string;
}

export interface AddProductRequest {
  name: string;
  description: string;
  price: number;
  quantity: number;
  categoryId: string;
  subCategoryId: string;
  colorId: string;
  imageUrls: string[];
  sizeIds: string[];
}

export interface UpdateProductRequest {
  name?: string;
  price?: number;
  subCategoryId?: string;
  sizeIds?: string[];
}

export interface UploadedImage {
  url: string;
  publicId: string;
}

export interface CreateCategoryRequest {
  name: string;
}

export interface CreateSubCategoryRequest {
  categoryId: string;
  name: string;
}

export interface CreateColorRequest {
  name: string;
}

export interface CreateSizeRequest {
  type: string;
  size: string;
}

export interface SetCoverImageRequest {
  imageId: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/admin`;

  constructor(private http: HttpClient) {}

  private getHttpOptions() {
    return {
      withCredentials: true // Include cookies for authentication
    };
  }

  login(credentials: AdminLoginRequest): Observable<AdminLoginResponse> {
    return this.http.post<AdminLoginResponse>(`${this.baseUrl}/login`, credentials, this.getHttpOptions());
  }

  logout(): Observable<any> {
    return this.http.post(`${this.baseUrl}/logout`, {}, this.getHttpOptions());
  }

  checkAuth(): Observable<AuthStatus> {
    return this.http.get<AuthStatus>(`${this.baseUrl}/check-auth`, this.getHttpOptions());
  }

  getProducts(): Observable<AdminProduct[]> {
    return this.http.get<AdminProduct[]>(`${this.baseUrl}/dashboard/products`, this.getHttpOptions());
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/categories`, this.getHttpOptions());
  }

  getSubCategories(): Observable<SubCategory[]> {
    return this.http.get<SubCategory[]>(`${this.baseUrl}/subcategories`, this.getHttpOptions());
  }

  getColors(): Observable<Color[]> {
    return this.http.get<Color[]>(`${this.baseUrl}/colors`, this.getHttpOptions());
  }

  getSizes(): Observable<Size[]> {
    return this.http.get<Size[]>(`${this.baseUrl}/sizes`, this.getHttpOptions());
  }

  addProduct(product: AddProductRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/products`, product, this.getHttpOptions());
  }

  updateProduct(id: string, product: UpdateProductRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}/products/${id}`, product, this.getHttpOptions());
  }

  deleteProduct(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/products/${id}/delete`, {}, this.getHttpOptions());
  }

  addCategory(category: CreateCategoryRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/categories`, category, this.getHttpOptions());
  }

  addSubCategory(subCategory: CreateSubCategoryRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/subcategories`, subCategory, this.getHttpOptions());
  }

  addColor(color: CreateColorRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/colors`, color, this.getHttpOptions());
  }

  addSize(size: CreateSizeRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/sizes`, size, this.getHttpOptions());
  }

  setCoverImage(productId: string, request: SetCoverImageRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/products/${productId}/set-cover-image`, request, this.getHttpOptions());
  }

  uploadProductImages(files: File[]): Observable<UploadedImage[]> {
    const formData = new FormData();
    files.forEach(file => formData.append('files', file));
    return this.http.post<UploadedImage[]>(`${this.baseUrl}/upload-images`, formData, this.getHttpOptions());
  }
}
