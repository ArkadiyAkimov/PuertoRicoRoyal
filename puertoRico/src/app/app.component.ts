import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {register} from 'swiper/element/bundle';
import {Swiper} from 'swiper/types';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements AfterViewInit {
  title = 'puertoRico';
  
  @ViewChild('swiper')
  swiperRef: ElementRef | undefined;
  swiper?: Swiper;

  ngAfterViewInit(): void {
    register();
    
  }

  onActiveIndexChange() {
    console.log(this.swiper?.activeIndex);
  }
}
