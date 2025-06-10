import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TutoringClass } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ClassesService {
  private readonly apiUrl = 'http://localhost:5115/classes';
  private readonly http = inject(HttpClient);

  constructor() {}

  getClasses(): Observable<TutoringClass[]> {
    return this.http.get<TutoringClass[]>(this.apiUrl);
  }

  getClass(id: number): Observable<TutoringClass> {
    return this.http.get<TutoringClass>(`${this.apiUrl}/${id}`);
  }

  addClass(tutoringClass: TutoringClass): Observable<TutoringClass> {
    return this.http.post<TutoringClass>(this.apiUrl, tutoringClass);
  }

  updateClass(tutoringClass: TutoringClass): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${tutoringClass.id}`, tutoringClass);
  }

  deleteClass(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addMockData(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/mock`, {});
  }
}
