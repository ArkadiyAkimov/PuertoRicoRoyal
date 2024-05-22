import { User } from './../../user/models/user';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GameStartInput, StartGameOutput } from '../classes/general';

@Injectable({
  providedIn: 'root'
})
export class GameStartHttpService {
  private url="Game" ;

  constructor(private http: HttpClient) { }
  
    public postNewGame(gameStartInput:GameStartInput): Observable<StartGameOutput> {
      console.log(`post new game ${environment.apiUrl}/${this.url} numOfPlayers: ${gameStartInput.numOfPlayers}`);
      return this.http.post<StartGameOutput>(`${environment.apiUrl}/Game`,gameStartInput);
  } 
}
