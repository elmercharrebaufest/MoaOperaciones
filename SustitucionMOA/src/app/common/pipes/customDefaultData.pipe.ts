import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'defaultData'
})
export class CustomDefaultDataPipe implements PipeTransform {



public transform(value: any, defaultValue?: string): string {
    const defaultVal = '-';
    if (Number.isFinite(value)) {
      return (value) > 0 ? value.toString() : (defaultValue !== undefined ? defaultValue : defaultVal);
    }
    else {
      return (value) ? value : (defaultValue !== undefined ? defaultValue : defaultVal);
    }
  }
} 