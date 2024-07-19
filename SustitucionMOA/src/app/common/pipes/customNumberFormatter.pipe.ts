import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
  name: 'customNumberFormatter'
})
export class CustomNumberFormatterPipe implements PipeTransform {
  transform(value: any, maxFractionDigits: number): any {
    const selectedCulture = 'en-US';
    if (!maxFractionDigits) {
      maxFractionDigits = 2;
    }
    if (!value || Number.isNaN(value)) {
      return 0;
    }
    if (typeof value === 'string' && value.length > 0) {
      value = parseFloat(value.replace(',', ''));
    }
    const localNumber = (value).toLocaleString(selectedCulture, { useGrouping: true, maximumFractionDigits: maxFractionDigits });
    return localNumber;
  }
}