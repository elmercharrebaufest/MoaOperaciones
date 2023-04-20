import { Pipe, PipeTransform } from '@angular/core';
/*
 * Formats a /Date(xxxxxxxxxxxxx)/ into a JSON Date object
 * Takes an argument as input of actual date value in /Date(xxxxxxxxxxxxx)/ format.
 * Usage:
 *   date-value | customDateFormat
 * Example:
 *   {{ '/Date(1402034400000)/' | customDateFormat}}
*/
@Pipe({name: 'customDateFormat'}  )
export class CustomDateFormat implements PipeTransform {
  transform(value: any) {
      if(!value) return "";
       var customDate = new Date(value.match(/\d+/)[0] * 1);  
       return  customDate;
  }
}