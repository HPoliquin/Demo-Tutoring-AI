import { Routes } from '@angular/router';
import { StudentsComponent } from './students/students.component';
import { ClassesComponent } from './classes/classes.component';
import { SchedulesComponent } from './schedules/schedules.component';

export const routes: Routes = [
  { path: 'students', component: StudentsComponent },
  { path: 'classes', component: ClassesComponent },
  { path: 'schedules', component: SchedulesComponent },
  { path: '', redirectTo: 'students', pathMatch: 'full' },
];
