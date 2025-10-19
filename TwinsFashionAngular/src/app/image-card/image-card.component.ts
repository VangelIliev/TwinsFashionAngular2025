import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-image-card',
  standalone: false,
  templateUrl: './image-card.component.html',
  styleUrls: ['./image-card.component.css']
})
export class ImageCardComponent {
  @Input() imageSrc = '';
  @Input() imageAlt = '';
  @Input() title = '';
  @Input() description = '';
  @Input() price?: number;
  @Input() priceLabel?: string;
  @Input() badge?: string;

  get hasDescription(): boolean {
    return this.description.trim().length > 0;
  }
}
