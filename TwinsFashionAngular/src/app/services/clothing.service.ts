import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, tap, catchError, map } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ClothingImage {
  id?: string;
  url: string;
  alt?: string;
  isCover?: boolean;
}

export interface ClothingSizeOption {
  label: BackendSize;
  value: BackendSize;
  inStock: boolean;
}

export interface BackendSize {
  id: string;
  size: string;
  type: string;
}

export interface ClothingCategory {
  id: string;
  name: string;
}

export interface ClothingItem {
  id: string;
  title: string;
  description: string;
  longDescription?: string;
  category: ClothingCategory;
  price: number;
  badge?: string;
  coverImageUrl: string;
  images: ClothingImage[];
  gallery: ClothingImage[];
  sizes: ClothingSizeOption[];
  quantity: number;
  color?: string;
  subcategory?: string;
}

function mapItem(item: any): ClothingItem {
  const images: ClothingImage[] = item.images ?? [];
  const gallery: ClothingImage[] = item.gallery ?? images;
  const cover = images.find(img => img.isCover) ?? images[0];

  const mappedSizes: ClothingSizeOption[] = (item.sizes as BackendSize[] | undefined)?.map((sizeObj: BackendSize) => ({
    label: sizeObj,
    value: sizeObj,
    inStock: true
  })) ?? [];

  const categoryValue = item.category ?? { id: '', name: '' };

  return {
    id: item.id,
    title: item.name ?? item.title ?? '',
    category: {
      id: categoryValue?.id ?? '',
      name: categoryValue?.name ?? ''
    },
    description: item.description ?? '',
    longDescription: item.longDescription ?? '',
    price: item.price ?? 0,
    badge: item.badge ?? '',
    coverImageUrl: cover?.url ?? item.coverImageUrl ?? '',
    images,
    gallery,
    sizes: mappedSizes,
    quantity: item.quantity ?? 1,
    color: item.color?.name ?? item.color ?? '',
    subcategory: item.subCategory?.name ?? item.subcategory ?? ''
  } as ClothingItem;
}

@Injectable({ providedIn: 'root' })
export class ClothingService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/products`;
  private readonly itemsSubject = new BehaviorSubject<ClothingItem[]>([]);
  readonly items$ = this.itemsSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadAll(): Observable<ClothingItem[]> {
    if (this.itemsSubject.value.length > 0) {
      return of(this.itemsSubject.value);
    }

    return this.http.get<any[]>(`${this.baseUrl}/all`).pipe(
      map(items => items.map(mapItem)),
      tap(items => this.itemsSubject.next(items)),
      catchError(() => {
        this.itemsSubject.next([]);
        return of([]);
      })
    );
  }

  getAll(): ClothingItem[] {
    return this.itemsSubject.value;
  }

  getById(id: string): Observable<ClothingItem | undefined> {
    const cached = this.itemsSubject.value.find(item => item.id === id);
    if (cached) {
      return of(cached);
    }

    return this.http.get<any>(`${this.baseUrl}/${id}`).pipe(
      map(mapItem),
      tap(item => {
        if (item) {
          const existingIndex = this.itemsSubject.value.findIndex(existing => existing.id === item.id);
          if (existingIndex >= 0) {
            const copy = [...this.itemsSubject.value];
            copy[existingIndex] = item;
            this.itemsSubject.next(copy);
          } else {
            this.itemsSubject.next([...this.itemsSubject.value, item]);
          }
        }
      }),
      catchError(() => of(undefined))
    );
  }
}
