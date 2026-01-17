import { Component, Input } from '@angular/core';
import { cloudinarySrcset, cloudinaryTransformUrl } from '../shared/image-url';

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
  @Input() priority = false;

  /**
   * Responsive defaults tuned for the /clothes grid.
   * We use Cloudinary transformations to avoid downloading original large images on mobile.
   */
  @Input() aspectRatio: string = '3:4';
  @Input() sizesAttr: string = '(max-width: 540px) 50vw, (max-width: 1024px) 33vw, 25vw';
  @Input() srcsetWidths: number[] = [240, 320, 400, 480, 640, 800];

  get hasDescription(): boolean {
    return this.description.trim().length > 0;
  }

  get resolvedSrc(): string {
    return cloudinaryTransformUrl(this.imageSrc, {
      width: 480,
      aspectRatio: this.aspectRatio,
      crop: 'fill',
      gravity: 'auto'
    });
  }

  get resolvedSrcset(): string | null {
    return cloudinarySrcset(this.imageSrc, this.srcsetWidths, {
      aspectRatio: this.aspectRatio,
      crop: 'fill',
      gravity: 'auto'
    });
  }
}
