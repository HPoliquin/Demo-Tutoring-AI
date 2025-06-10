import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { StudentsComponent } from './students/students.component';

@NgModule({
  declarations: [
    // ...existing declarations...
  ],
  imports: [
    StudentsComponent,
    // ...existing imports...
  ],
  providers: [

  ],
})
export class AppModule { }
