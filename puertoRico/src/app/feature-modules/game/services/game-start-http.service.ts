import { User } from './../../user/models/user';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StartGameOutput } from '../classes/general';

@Injectable({
  providedIn: 'root'
})
export class GameStartHttpService {
  private url="Game" ;

  constructor(private http: HttpClient) { }
  
    public postNewGame(gameId: number, numOfPlayers: number, playerIndex:number): Observable<StartGameOutput> {
      console.log(`post new game ${environment.apiUrl}/${this.url} numOfPlayers: ${numOfPlayers}`);
      return this.http.post<StartGameOutput>(`${environment.apiUrl}/Game`,{ gameId, numOfPlayers,playerIndex});
  } 
}
