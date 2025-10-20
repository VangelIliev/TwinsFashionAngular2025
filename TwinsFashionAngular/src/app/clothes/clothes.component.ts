import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ClothingItem, ClothingService } from '../services/clothing.service';

@Component({
  selector: 'app-clothes',
  standalone: false,
  templateUrl: './clothes.component.html',
  styleUrls: ['./clothes.component.css']
})
export class ClothesComponent implements OnInit, OnDestroy {
  readonly priceLabel = 'Цена';
  readonly collectionTitle = 'Есенна / Зимна Колекция 2025';
  items: ClothingItem[] = [];
  private subscription?: Subscription;

  constructor(private clothingService: ClothingService) {}

  ngOnInit(): void {
    this.subscription = this.clothingService.loadAll().subscribe(items => {
      this.items = items;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
