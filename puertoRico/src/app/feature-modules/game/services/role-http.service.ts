import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { Observable } from 'rxjs';
import { GameStateJson } from './game-start-http.service';


@Injectable({
  providedIn: 'root'
})
export class RoleHttpService {

  constructor(private http: HttpClient){
  }
  
  public postRole(roleId:number, dataGameId:number, playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/Role`,{roleId ,dataGameId ,playerIndex});
  } 
  
  public postBuilding(buildingId:number, dataGameId:number, playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/Building`,{buildingId ,dataGameId ,playerIndex});
  } 

  public postPlantation(plantationId:number, dataGameId:number, playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/Plantation`,{plantationId ,dataGameId, playerIndex});
  } 

  public postQuarry( dataGameId:number, playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/Plantation/quarry`,{ dataGameId ,playerIndex});
  } 

  public postUpsideDown( dataGameId:number, playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/Plantation/upSideDown`,{ dataGameId ,playerIndex});
  } 

  public postSlot(slotId:number, dataGameId:number, playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/Slot`,{slotId ,dataGameId ,playerIndex});
  } 

  public postEndTurn( dataGameId:number, StorageGoods:number[], playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/EndTurn`,{ dataGameId, StorageGoods ,playerIndex});
  } 

  public postGood(GoodId:number, ShipIndex:number, DataGameId:number, playerIndex:number): Observable<GameStateJson> {
    return this.http.post<GameStateJson>(`${environment.apiUrl}/Good`,{GoodId ,ShipIndex,DataGameId ,playerIndex});
  } 

}


