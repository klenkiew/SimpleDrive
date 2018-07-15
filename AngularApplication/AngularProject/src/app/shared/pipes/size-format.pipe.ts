import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'sizeFormat'
})
export class SizeFormatPipe implements PipeTransform {

  private sizeSuffixes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB'];

  transform(value: number, precision: number = 2): string {
    if ( isNaN( parseFloat( String(value) )) || ! isFinite( value) ) return '?';

    let index = 0;

    while ( value >= 1024 ) {
      value /= 1024;
      index ++;
    }

    return value.toFixed(+precision) + ' ' + this.sizeSuffixes[index];
  }
}
