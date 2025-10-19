import { Injectable } from '@angular/core';

export interface ClothingImage {
  url: string;
  alt: string;
}

export interface ClothingSizeOption {
  label: string;
  value: string;
  inStock: boolean;
}

export interface ClothingItem {
  id: string;
  slug: string;
  title: string;
  description: string;
  longDescription?: string;
  category: string;
  price: number;
  badge?: string;
  coverImage: ClothingImage;
  gallery: ClothingImage[];
  sizes: ClothingSizeOption[];
}

@Injectable({ providedIn: 'root' })
export class ClothingService {
  private readonly items: ClothingItem[] = [
    {
      id: 'jacket-black',
      slug: 'kozeno-yake-cherno',
      title: 'Кожено яке с пух в цвят "черно"',
      description: 'Елегантен силует, богата яка от естествен пух, златни детайли.',
      longDescription: 'Ръчно подбрана кожа, богата яка от естествен пух и златни акценти, които подчертават модерния силует. Подплатата е от мек сатен за максимален комфорт.',
      category: 'Дамско',
      price: 160,
      badge: 'Ново',
      coverImage: {
        url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821715/Jacket_1_gxadvj.jpg',
        alt: 'Черно кожено яке с пух'
      },
      gallery: [
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821715/Jacket_1_gxadvj.jpg', alt: 'Кожено яке черно - преден изглед' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821719/Jacket_7_brq80i.jpg', alt: 'Кожено яке черно - гръб' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821715/Jacke_2_iebaxs.jpg', alt: 'Кожено яке черно - детайл яка' }
      ],
      sizes: [
        { label: 'S', value: 'S', inStock: true },
        { label: 'M', value: 'M', inStock: true },
        { label: 'L', value: 'L', inStock: true }
      ]
    },
    {
      id: 'jacket-caramel',
      slug: 'kozeno-yake-karamel',
      title: 'Кожено яке с пух в цвят "карамел"',
      description: 'Топъл карамелен оттенък и премиум изработка за ежедневен блясък.',
      longDescription: 'Меко шлифована кожа в карамелен нюанс, пухкава яка и маншети от естествен косъм. Подходящо за зимни вечери с елегантни детайли.',
      category: 'Дамско',
      price: 160,
      coverImage: {
        url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821717/Jacket_5_wd2zdr.jpg',
        alt: 'Кожено яке с пух в карамелен цвят'
      },
      gallery: [
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821717/Jacket_5_wd2zdr.jpg', alt: 'Карамелено кожено яке - преден изглед' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821730/Jacket_15_yiyuhi.jpg', alt: 'Карамелено яке - детайл' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821734/Palto_1_sv4bsr.jpg', alt: 'Карамелено яке - гръб' }
      ],
      sizes: [
        { label: 'S', value: 'S', inStock: true },
        { label: 'M', value: 'M', inStock: true },
        { label: 'L', value: 'L', inStock: true },
        { label: 'XL', value: 'XL', inStock: false }
      ]
    },
    {
      id: 'jacket-chocolate',
      slug: 'kozeno-yake-shokolad',
      title: 'Кожено яке с пух в цвят "шоколад"',
      description: 'Структурирана кройка и дълбок шоколадов тон за изискано излъчване.',
      longDescription: 'Тъмно шоколадово яке с пухкав кант. Вътрешна подплата от сатен за допълнителна мекота.',
      category: 'Дамско',
      price: 160,
      coverImage: {
        url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821716/Jacket_4_ddbcjc.jpg',
        alt: 'Кожено яке с пух в шоколадов цвят'
      },
      gallery: [
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821716/Jacket_4_ddbcjc.jpg', alt: 'Шоколадово яке - преден изглед' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821726/Jacket_12_ilamwn.jpg', alt: 'Шоколадово яке - детайл' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821715/Jacke_2_iebaxs.jpg', alt: 'Шоколадово яке - яка' }
      ],
      sizes: [
        { label: 'S', value: 'S', inStock: true },
        { label: 'M', value: 'M', inStock: true },
        { label: 'L', value: 'L', inStock: true }
      ]
    },
    {
      id: 'parka-bronze',
      slug: 'parka-bronz',
      title: 'Пухено яке в бронзов цвят',
      description: 'Металически бронзов ефект и топъл пух за впечатляващо присъствие.',
      longDescription: 'Пухено яке с луксозна, металическа текстура и гарантирана топлина, оборудвано с практични джобове и устойчиви ципове.',
      category: 'Дамско',
      price: 155,
      coverImage: {
        url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821721/Jacket_8_j1tli3.jpg',
        alt: 'Бронзова пухена парка'
      },
      gallery: [
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821721/Jacket_8_j1tli3.jpg', alt: 'Бронзова парка - преден изглед' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821726/Jacket_13_jfusrc.jpg', alt: 'Бронзова парка - детайл' },
        { url: 'https://res.cloudinary.com/dl6dp2cr0/image/upload/v1760821734/Palto_2_tcuklu.jpg', alt: 'Бронзова парка - гръб' }
      ],
      sizes: [
        { label: 'S', value: 'S', inStock: true },
        { label: 'M', value: 'M', inStock: true },
        { label: 'L', value: 'L', inStock: true },
        { label: 'XL', value: 'XL', inStock: true }
      ]
    }
  ];

  getAll(): ClothingItem[] {
    return this.items;
  }

  getBySlug(slug: string): ClothingItem | undefined {
    return this.items.find(item => item.slug === slug);
  }
}
