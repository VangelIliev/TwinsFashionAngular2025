import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, tap, catchError, map } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ClothingImage {
  url: string;
  alt: string;
  isCover?: boolean;
}

export interface ClothingSizeOption {
  label: string;
  value: string;
  inStock: boolean;
}

export interface ClothingItem {
  id: string;
  title: string;
  description: string;
  longDescription?: string;
  category: string;
  price: number;
  badge?: string;
  coverImageUrl: string;
  images: ClothingImage[];
  gallery: ClothingImage[];
  sizes: ClothingSizeOption[];
}

function mapItem(item: ClothingItem): ClothingItem {
  const cover = item.images?.find(i => i.isCover) ?? item.images?.[0];
  return {
    ...item,
    coverImageUrl: cover?.url ?? item.coverImageUrl,
    gallery: item.gallery ?? item.images
  };
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

    return this.http.get<ClothingItem[]>(`${this.baseUrl}/all`).pipe(
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

    return this.http.get<ClothingItem>(`${this.baseUrl}/${id}`).pipe(
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
