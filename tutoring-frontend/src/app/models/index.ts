export interface TutoringClass {
  id: number;
  name: string;
  description?: string;
  subject: string;
  difficultyLevel: number;
  pricePerHour: number;
  maxStudents: number;
  createdAt: Date;
  schedules?: Schedule[];
  enrollments?: StudentClassEnrollment[];
}

export interface Schedule {
  id: number;
  tutoringClassId: number;
  startTime: Date;
  endTime: Date;
  dayOfWeek: string;
  location?: string;
  isActive: boolean;
  maxCapacity: number;
  createdAt: Date;
  tutoringClass?: TutoringClass;
  studentEnrollments?: StudentScheduleEnrollment[];
}

export interface StudentClassEnrollment {
  id: number;
  studentId: number;
  tutoringClassId: number;
  enrolledAt: Date;
  isActive: boolean;
  student?: Student;
  tutoringClass?: TutoringClass;
}

export interface StudentScheduleEnrollment {
  id: number;
  studentId: number;
  scheduleId: number;
  enrolledAt: Date;
  isActive: boolean;
  notes?: string;
  student?: Student;
  schedule?: Schedule;
}

// Re-export the existing Student interface with enhanced properties
export interface Student {
  id: number;
  name: string;
  email: string;
  phoneNumber?: string;
  age?: number;
  grade?: string;
  address?: string;
  createdAt: Date;
  isActive: boolean;
  classEnrollments?: StudentClassEnrollment[];
  scheduleEnrollments?: StudentScheduleEnrollment[];
}
