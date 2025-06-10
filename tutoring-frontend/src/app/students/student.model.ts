export interface Student {
  id: number;
  name: string;
  email: string;
  phoneNumber?: string;
  age?: number;
  grade?: string;
  address?: string;
  createdAt?: Date;
  isActive?: boolean;
}
