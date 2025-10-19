import { Component } from '@angular/core';
import { ClothingItem, ClothingService } from '../services/clothing.service';

@Component({
  selector: 'app-clothes',
  standalone: false,
  templateUrl: './clothes.component.html',
  styleUrls: ['./clothes.component.css']
})
export class ClothesComponent {
  readonly priceLabel = 'Цена';
  readonly collectionTitle = 'Есенна / Зимна Колекция 2025';

  constructor(private clothingService: ClothingService) {}

  get items(): ClothingItem[] {
    return this.clothingService.getAll();
  }
}
