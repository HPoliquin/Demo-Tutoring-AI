import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { StudentsService } from './students.service';
import { Student } from '../models';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-students',
  templateUrl: './students.component.html',
  styleUrls: ['./students.component.css'],
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule
  ],
  standalone: true,
})
export class StudentsComponent implements OnInit {
  students: Student[] = [];
  newStudent: Partial<Student> = {
    name: '',
    email: '',
    phoneNumber: '',
    age: undefined,
    grade: '',
    address: '',
    isActive: true
  };
  showModal = false;
  showAddForm = false;
  modalStudentId: number | null = null;

  constructor(private readonly studentsService: StudentsService, httpclient: HttpClient) {}

  ngOnInit(): void {
    this.getStudents();
  }

  getStudents(): void {
    this.studentsService.getStudents().subscribe(data => {
      this.students = data;
    });
  }

  addStudent(): void {
    const studentData = {
      ...this.newStudent,
      createdAt: new Date()
    } as Student;

    this.studentsService.addStudent(studentData).subscribe(() => {
      this.getStudents();
      this.resetForm();
    });
  }

  private resetForm(): void {
    this.newStudent = {
      name: '',
      email: '',
      phoneNumber: '',
      age: undefined,
      grade: '',
      address: '',
      isActive: true
    };
    this.showAddForm = false;
  }

  updateStudent(student: Student): void {
    this.studentsService.updateStudent(student).subscribe(() => {
      this.getStudents();
    });
  }

  deleteStudent(id: number): void {
    this.studentsService.deleteStudent(id).subscribe(() => {
      this.getStudents();
    });
  }

  confirmDelete(id: number): void {
    if (confirm('Are you sure you want to delete this student?')) {
      this.deleteStudent(id);
    }
  }

  openModal(id: number): void {
    this.modalStudentId = id;
    this.showModal = true;
  }

  closeModal(): void {
    this.modalStudentId = null;
    this.showModal = false;
  }
}
