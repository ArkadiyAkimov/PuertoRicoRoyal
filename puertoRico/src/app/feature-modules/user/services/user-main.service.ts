import { environment } from 'src/environments/environment.development';
import { User } from './../models/user';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserMainService {
  private url="User" ;
  userLoggedIn:BehaviorSubject<User|null> = new BehaviorSubject<User|null>(null);

  constructor(
    private http:HttpClient
    ){}

  public getUser(id: number): Observable<User> {
  return this.http.get<User>(`${environment.apiUrl}/${this.url}/${id}`);
}

  public registerUser(user: User): Observable<User> {
    return this.http.post<User>(`${environment.apiUrl}/${this.url}`, user);
  }

  public updateUser(user: User): Observable<User> {
    return this.http.put<User>(`${environment.apiUrl}/${this.url}/${user.id}`, user);
  }
  
  public deleteUser(id: number): Observable<User> {
    return this.http.delete<User>(`${environment.apiUrl}/${this.url}/${id}`);
  }

  public getUserByUsernameAndPassword(username: string, password: string): Observable<User> {
    console.log(this.http);
    return this.http.post<User>(`${environment.apiUrl}/${this.url}/authenticate`,
    { username, password });
  } 

}