import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClassesService } from '../services/classes.service';
import { TutoringClass } from '../models';

@Component({
  selector: 'app-classes',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './classes.component.html',
  styleUrl: './classes.component.css'
})
export class ClassesComponent implements OnInit {
  classes: TutoringClass[] = [];
  showModal = false;
  showAddForm = false;
  modalClassId: number | null = null;

  newClass: TutoringClass = {
    id: 0,
    name: '',
    description: '',
    subject: '',
    difficultyLevel: 1,
    pricePerHour: 0,
    maxStudents: 10,
    createdAt: new Date()
  };

  subjects = ['Mathematics', 'English', 'Physics', 'Chemistry', 'Biology', 'Computer Science', 'Spanish', 'French', 'History', 'Geography'];
  difficultyLevels = [
    { value: 1, label: 'Beginner' },
    { value: 2, label: 'Elementary' },
    { value: 3, label: 'Intermediate' },
    { value: 4, label: 'Advanced' },
    { value: 5, label: 'Expert' }
  ];

  constructor(private readonly classesService: ClassesService) {}

  ngOnInit(): void {
    this.getClasses();
  }

  getClasses(): void {
    this.classesService.getClasses().subscribe(data => {
      this.classes = data;
    });
  }

  addClass(): void {
    this.classesService.addClass(this.newClass).subscribe(() => {
      this.getClasses();
      this.resetForm();
      this.showAddForm = false;
    });
  }

  updateClass(tutoringClass: TutoringClass): void {
    this.classesService.updateClass(tutoringClass).subscribe(() => {
      this.getClasses();
    });
  }

  deleteClass(id: number): void {
    this.classesService.deleteClass(id).subscribe(() => {
      this.getClasses();
    });
  }

  openModal(id: number): void {
    this.modalClassId = id;
    this.showModal = true;
  }

  closeModal(): void {
    this.modalClassId = null;
    this.showModal = false;
  }

  toggleAddForm(): void {
    this.showAddForm = !this.showAddForm;
    if (!this.showAddForm) {
      this.resetForm();
    }
  }

  resetForm(): void {
    this.newClass = {
      id: 0,
      name: '',
      description: '',
      subject: '',
      difficultyLevel: 1,
      pricePerHour: 0,
      maxStudents: 10,
      createdAt: new Date()
    };
  }

  getDifficultyLabel(level: number): string {
    const difficulty = this.difficultyLevels.find(d => d.value === level);
    return difficulty ? difficulty.label : 'Unknown';
  }

  addMockData(): void {
    this.classesService.addMockData().subscribe(() => {
      this.getClasses();
    });
  }
}
