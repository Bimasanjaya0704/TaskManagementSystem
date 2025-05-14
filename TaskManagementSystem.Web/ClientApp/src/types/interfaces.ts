import { ROLE } from "./roles";

export interface AuthState {
  isAuthenticated: boolean;
  role: ROLE | null;
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
}

export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface UserResponseDto {
  userId: string;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  role: ROLE;
  assignedTasks: TaskResponseDto[];
  reviewedTasks: TaskResponseDto[];
}

export interface TaskRequestDto {
  title: string;
  description: string;
  dueDate: Date;
  // priority: Priority;
  status: Status;
  assignedTo: string;
  reviewedBy: string;
  projectId: string;
}

export interface TaskResponseDto {
  id: string;
  title: string;
  description: string;
  dueDate: Date;
  // priority: Priority;
  status: Status;
  assignedTo: string;
  reviewedBy: string;
  projectId: string;
}

export interface ProjectRequestDto {
  name: string;
  description: string;
  dueDate: Date;
  projectStatus: ProjectStatus;
  projectPriority: ProjectPriority;
  createdByUserId: string;
}

export interface ProjectResponseDto {
  projectId: string;
  name: string;
  description: string;
  dueDate: Date;
  projectStatus: ProjectStatus;
  projectPriority: ProjectPriority;
  creatorUserId: string;
  tasks: TaskResponseDto[];
}

// export enum Priority {
//   LOW = "Low",
//   MEDIUM = "Medium",
//   HIGH = "High",
// }

export enum Status {
  New = "New",
  InProgress = "InProgress",
  PendingReview = "PendingReview",
  Done = "Done",
  Blocked = "Blocked",
  OnHold = "OnHold",
}

export enum ProjectStatus {
  NotStarted = "NotStarted",
  InProgress = "InProgress",
  Completed = "Completed",
  OnHold = "OnHold",
}

export enum ProjectPriority {
  Low = "Low",
  Medium = "Medium",
  High = "High",
}