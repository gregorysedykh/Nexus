import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface User {
  id: number;
  username: string;
  email: string;
}

export interface WordDto {
  id: number;
  term: string;
  languageCode: string;
}

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/users/${id}`);
  }

  getUserWords(userId: number): Observable<WordDto[]> {
    return this.http.get<WordDto[]>(`${this.baseUrl}/users/${userId}/words`);
  }
}
