import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { RoleName } from '../classes/general';

@Injectable({
  providedIn: 'root'
})
export class ScrollService {

  constructor(private router:Router ) { }

  autoScroll(role:RoleName){
     return; //temp
    switch(role){
          case RoleName.NoRole:
          this.router.navigate([{ behavior: 'smooth' }], { fragment: "roles"})
          break;
          case RoleName.Settler:
            this.router.navigate([{ behavior: 'smooth' }], { fragment: "plantations" })
          break;
          case RoleName.Builder:
          this.router.navigate([{ behavior: 'smooth' }], { fragment: "buildings" })
          break;
          case RoleName.Mayor:
          this.router.navigate([{ behavior: 'smooth' }], { fragment: "supply" })
          break;
          case RoleName.Trader:
            this.router.navigate([{ behavior: 'smooth' }], { fragment: "cargo-ships" })
          break;
          case RoleName.Craftsman:
          this.router.navigate([{ behavior: 'smooth' }], { fragment: "supply" })
          break;
          case RoleName.Captain:
          this.router.navigate([{ behavior: 'smooth' }], { fragment: "cargo-ships" })
          break;
          case RoleName.Prospector:
          break;    
    }
  }

}
