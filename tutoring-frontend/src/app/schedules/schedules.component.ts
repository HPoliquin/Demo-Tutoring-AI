import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Schedule } from '../models';
import { SchedulesService } from '../services/schedules.service';

@Component({
  selector: 'app-schedules',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './schedules.component.html',
  styleUrl: './schedules.component.css'
})
export class SchedulesComponent implements OnInit {
  schedules: Schedule[] = [];
  scheduleForm: FormGroup;
  isFormVisible = false;
  isEditMode = false;
  editingScheduleId: number | null = null;
  isLoading = false;

  // Days of the week for dropdown
  daysOfWeek = [
    { value: 'Monday', label: 'Monday' },
    { value: 'Tuesday', label: 'Tuesday' },
    { value: 'Wednesday', label: 'Wednesday' },
    { value: 'Thursday', label: 'Thursday' },
    { value: 'Friday', label: 'Friday' },
    { value: 'Saturday', label: 'Saturday' },
    { value: 'Sunday', label: 'Sunday' }
  ];

  constructor(
    private readonly schedulesService: SchedulesService,
    private readonly formBuilder: FormBuilder
  ) {
    this.scheduleForm = this.formBuilder.group({
      tutoringClassId: [1, [Validators.required, Validators.min(1)]],
      startTime: ['', [Validators.required]],
      endTime: ['', [Validators.required]],
      dayOfWeek: ['', [Validators.required]],
      location: ['', [Validators.maxLength(200)]],
      maxCapacity: [1, [Validators.required, Validators.min(1), Validators.max(50)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadSchedules();
  }

  loadSchedules(): void {
    this.isLoading = true;
    this.schedulesService.getSchedules().subscribe({
      next: (schedules) => {
        this.schedules = schedules;
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Error loading schedules:', error);
        this.isLoading = false;
      }
    });
  }

  showForm(): void {
    this.isFormVisible = true;
    this.isEditMode = false;
    this.editingScheduleId = null;
    this.scheduleForm.reset({
      tutoringClassId: 1,
      startTime: '',
      endTime: '',
      dayOfWeek: '',
      location: '',
      maxCapacity: 1,
      isActive: true
    });
  }

  hideForm(): void {
    this.isFormVisible = false;
    this.isEditMode = false;
    this.editingScheduleId = null;
    this.scheduleForm.reset();
  }

  editSchedule(schedule: Schedule): void {
    this.isFormVisible = true;
    this.isEditMode = true;
    this.editingScheduleId = schedule.id;

    // Format times for HTML time input (HH:mm format)
    const startTime = this.formatTimeForInput(schedule.startTime);
    const endTime = this.formatTimeForInput(schedule.endTime);

    this.scheduleForm.patchValue({
      tutoringClassId: schedule.tutoringClassId,
      startTime: startTime,
      endTime: endTime,
      dayOfWeek: schedule.dayOfWeek,
      location: schedule.location ?? '',
      maxCapacity: schedule.maxCapacity,
      isActive: schedule.isActive
    });
  }

  deleteSchedule(id: number): void {
    if (confirm('Are you sure you want to delete this schedule?')) {
      this.schedulesService.deleteSchedule(id).subscribe({
        next: () => {
          this.loadSchedules();
        },
        error: (error: any) => {
          console.error('Error deleting schedule:', error);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.scheduleForm.valid) {
      const formValue = this.scheduleForm.value;

      // Convert time format for backend
      const scheduleData: Partial<Schedule> = {
        tutoringClassId: formValue.tutoringClassId,
        startTime: this.parseTimeForBackend(formValue.startTime),
        endTime: this.parseTimeForBackend(formValue.endTime),
        dayOfWeek: formValue.dayOfWeek,
        location: formValue.location,
        maxCapacity: formValue.maxCapacity,
        isActive: formValue.isActive,
        createdAt: new Date()
      };

      if (this.isEditMode && this.editingScheduleId) {
        const updateData = { ...scheduleData, id: this.editingScheduleId } as Schedule;
        this.schedulesService.updateSchedule(updateData).subscribe({
          next: () => {
            this.loadSchedules();
            this.hideForm();
          },
          error: (error: any) => {
            console.error('Error updating schedule:', error);
          }
        });
      } else {
        this.schedulesService.addSchedule(scheduleData as Schedule).subscribe({
          next: () => {
            this.loadSchedules();
            this.hideForm();
          },
          error: (error: any) => {
            console.error('Error creating schedule:', error);
          }
        });
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.scheduleForm.controls).forEach(key => {
      const control = this.scheduleForm.get(key);
      control?.markAsTouched();
    });
  }

  private formatTimeForInput(time: Date): string {
    if (!time) return '';
    const date = new Date(time);
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }

  private parseTimeForBackend(timeString: string): Date {
    const today = new Date();
    const [hours, minutes] = timeString.split(':').map(Number);
    const date = new Date(today.getFullYear(), today.getMonth(), today.getDate(), hours, minutes);
    return date;
  }

  formatTime(time: Date): string {
    if (!time) return 'N/A';
    const date = new Date(time);
    return date.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  trackByScheduleId(index: number, schedule: Schedule): number {
    return schedule.id;
  }

  generateMockData(): void {
    this.isLoading = true;
    this.schedulesService.addMockData().subscribe({
      next: () => {
        this.loadSchedules();
      },
      error: (error: any) => {
        console.error('Error generating mock data:', error);
        this.isLoading = false;
      }
    });
  }
}
