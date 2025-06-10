import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Schedule } from '../models';

@Injectable({
  providedIn: 'root'
})
export class SchedulesService {
  private readonly apiUrl = 'http://localhost:5115/schedules';
  private readonly http = inject(HttpClient);

  constructor() {}

  getSchedules(): Observable<Schedule[]> {
    return this.http.get<Schedule[]>(this.apiUrl);
  }

  getSchedule(id: number): Observable<Schedule> {
    return this.http.get<Schedule>(`${this.apiUrl}/${id}`);
  }

  addSchedule(schedule: Schedule): Observable<Schedule> {
    return this.http.post<Schedule>(this.apiUrl, schedule);
  }

  updateSchedule(schedule: Schedule): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${schedule.id}`, schedule);
  }

  deleteSchedule(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addMockData(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/mock`, {});
  }
}
