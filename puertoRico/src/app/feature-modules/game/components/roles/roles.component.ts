import { GameStateJson } from './../../services/game-start-http.service';
import { RoleHttpService } from './../../services/role-http.service';
import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { DataRole } from '../../services/game-start-http.service';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss']
})
export class RolesComponent implements OnInit{
  private url="Role" ;
  roles:DataRole[] = [];
  gs:GameStateJson = new GameStateJson();

  constructor(
    public gameService:GameService,
    private roleHttp:RoleHttpService
    ){
    }

    ngOnInit(): void {
      this.gameService.gs.subscribe({
        next: (gs:GameStateJson) => {
          if(gs.roles.length == 0) return;
          this.gs = gs;
          this.roles = gs.roles;
          this.roles.sort((a,b) => a.id - b.id);
        }
      })
    }
    
    bountyAnimations:string[] = ['bounty1','bounty2','bounty3','bounty4','bounty5','bounty6','bounty7','bounty8','bounty9','bounty10'];

    onClick(roleId:number){
    this.roleHttp.postRole(roleId, this.gameService.gs.value.id, this.gameService.playerIndex)
    .subscribe({
      next: (result:GameStateJson) => {
        console.log('success: ',result);
        this.gameService.gs.next(result);
      },
      error: (response:any)=> {
        console.log("error:",response.error.text);
      }
    });
  }

  getRoleNameString(role:DataRole):string{
    switch(role.name){
        case 0:
            return 'Settler';
        case 1:
            return 'Builder';
        case 2:
            return 'Mayor';
        case 3:
            return 'Trader';
        case 4:
            return 'Craftsman';
        case 5:
            return 'Captain';
        case 6:
            return 'Prospector';
        case 7:
            return 'PostCaptain';
        default:
            return '';
    }
}

getRoleSelectionClass(role:DataRole):string{
  if(role.name == 5 && this.gs.currentRole == 7) return 'role-alternate';
  else if(this.gs.currentRole == role.name ) return 'role-selected';
  return '';
}

get5coinsQuant(bounty:number):number{
  return Math.floor(bounty/5);
}

}
