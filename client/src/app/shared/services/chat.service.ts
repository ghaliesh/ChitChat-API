import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  public endpoint = 'http://localhost:5000/api/messages';
  constructor(private http: HttpClient) {}

  addMessage(message) {
    return this.http.post(`${this.endpoint}/add`, message);
  }
}
