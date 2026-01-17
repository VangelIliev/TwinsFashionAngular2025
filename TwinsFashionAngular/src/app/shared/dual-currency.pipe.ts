import { DecimalPipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

const BGN_PER_EUR = 1.95583;

@Pipe({
  name: 'dualCurrency',
  standalone: false
})
export class DualCurrencyPipe implements PipeTransform {
  constructor(private decimalPipe: DecimalPipe) {}

  transform(value: number | string | null | undefined, digitsInfo = '1.2-2'): string {
    if (value === null || value === undefined || value === '') {
      return '';
    }

    const numericValue = typeof value === 'string' ? Number(value) : value;
    if (!Number.isFinite(numericValue)) {
      return '';
    }

    const eur = numericValue / BGN_PER_EUR;
    const eurFormatted = this.decimalPipe.transform(eur, digitsInfo) ?? '';
    const bgnFormatted = this.decimalPipe.transform(numericValue, digitsInfo) ?? '';

    return `€${eurFormatted} / ${bgnFormatted} лв`;
  }
}
