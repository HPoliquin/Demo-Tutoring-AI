import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Student } from '../models';

@Injectable({
  providedIn: 'root'
})
export class StudentsService {
  private readonly apiUrl = 'http://localhost:5115/students';
  private readonly http = inject(HttpClient);

  constructor() {}

  getStudents(): Observable<Student[]> {
    return this.http.get<Student[]>(this.apiUrl);
  }

  addStudent(student: Student): Observable<Student> {
    return this.http.post<Student>(this.apiUrl, student);
  }

  updateStudent(student: Student): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${student.id}`, student);
  }

  deleteStudent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
