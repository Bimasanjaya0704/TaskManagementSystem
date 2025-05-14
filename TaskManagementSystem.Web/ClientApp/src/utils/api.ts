import axios from "axios";
import { LoginRequestDto, ProjectRequestDto, RegisterDto, TaskRequestDto } from "../types/interfaces";
import { useAuth } from "../context/AuthContext";

const API_BASE_URL = "http://localhost:5023/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Setup axios interceptors untuk otomatis menambahkan token ke setiap request
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor untuk menangani error 401 (Unauthorized)
api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      const { logout: logoutAuth } = useAuth();
      logoutAuth();
    }
    return Promise.reject(error);
  }
);

// Fungsi untuk debugging, bisa dipanggil saat perlu memeriksa header
export function logAuthHeader() {
  const token = localStorage.getItem("token");
  console.log("Current token in localStorage:", token ? `${token.substring(0, 15)}...` : "None");
  console.log("Current headers:", api.defaults.headers.common);
}

export function handleError(error: unknown): never {
  if (axios.isAxiosError(error)) {
    throw new Error(
      error.response?.data?.errorMessage || error.response?.data?.message || "An unexpected error occurred"
    );
  }
  throw new Error("An unexpected error occurred");
}

// Auth Api
export async function login(loginDto: LoginRequestDto) {
  try {
    const { data } = await api.post("/auth/login", loginDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function registerUser(registerDto: RegisterDto) {
  try {
    const { data } = await api.post("/auth/register", registerDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

// User Api
export async function getAllUsers() {
  try {
    const { data } = await api.get("/user");
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getUserById(id: string) {
  try {
    const { data } = await api.get(`/user/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function updateUser(id: string, userDto: any) {
  try {
    const { data } = await api.put(`/user/${id}`, userDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function deleteUser(id: string) {
  try {
    const { data } = await api.delete(`/user/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

// Task Api
export async function getAllTasks() {
  try {
    const { data } = await api.get("/task");
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function createTask(taskDto: TaskRequestDto) {
  try {
    const { data } = await api.post(`/task`, taskDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getTaskById(id: string) {
  try {
    const { data } = await api.get(`/task/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function updateTask(id: string, taskDto: TaskRequestDto) {
  try {
    const { data } = await api.put(`/task/${id}`, taskDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function deleteTask(id: string) {
  try {
    const { data } = await api.delete(`/task/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getReviewedTasks(id: string) {
  try {
    const { data } = await api.get(`/task/review-tasks/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getAssignedTasks(id: string) {
  try {
    const { data } = await api.get(`/task/assigned-tasks/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

// Project Api
export async function getAllProjects() {
  try {
    const { data } = await api.get("/project");
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function createProject(projectDto: ProjectRequestDto) {
  try {
    const { data } = await api.post(`/project`, projectDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getProjectById(id: string) {
  try {
    const { data } = await api.get(`/project/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function updateProject(id: string, projectDto: ProjectRequestDto) {
  try {
    const { data } = await api.put(`/project/${id}`, projectDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function deleteProject(id: string) {
  try {
    const { data } = await api.delete(`/project/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}