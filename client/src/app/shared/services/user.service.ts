import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { IUser } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  public isLoggedIn = new BehaviorSubject<Boolean>(this.hasToken());
  public currentUser = new BehaviorSubject<IUser>(null);
  private endPoint = 'http://localhost:5000/api/account';
  constructor(private http: HttpClient) {}
  register(user) {
    const form = new FormData();
    form.append('name', user.name);
    form.append('password', user.password);
    form.append('email', user.email);
    return this.http.post(`${this.endPoint}/register`, form);
  }

  hasToken() {
    return !!localStorage.getItem('x-token');
  }

  logout() {
    localStorage.removeItem('x-token');
    this.isLoggedIn.next(false);
    this.currentUser.next(null);
  }

  whoAMi() {
    const token = localStorage.getItem('x-token');
    if (!token) {
      return;
    }
    const header = new HttpHeaders({ token: token });
    return this.http.get<IUser>(`${this.endPoint}/whoami`, { headers: header });
  }

  userIdentity() {
    return this.currentUser.asObservable();
  }

  isAuthenticated() {
    return this.isLoggedIn.asObservable();
  }
}
