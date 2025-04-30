import axios from "axios";
import { LoginRequestDto, RegisterDto } from "../types/interfaces";

const API_BASE_URL = "http://localhost:5023/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

export function setAuthToken(token: string | null) {
  if (token) {
    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    api.defaults.headers.common["Content-Type"] = "application/json";
  } else {
    delete api.defaults.headers.common["Authorization"];
  }

  console.log("header :", api.defaults.headers.common);
}

export function handleError(error: unknown): never {
  if (axios.isAxiosError(error)) {
    throw new Error(
      error.response?.data?.message || "An unexpected error occurred"
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
export async function getAllUsers(token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.get("/user");
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getUserById(id: number, token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.get(`/user/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function updateUser(id: number, token: string, userDto: any) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.put(`/user/${id}`, userDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function deleteUser(id: number, token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.delete(`/user/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

// Task Api
export async function getAllTasks(token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.get("/task");
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function createTask(token: string, taskDto: any) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.post(`/task`, taskDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getTaskById(id: number, token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.get(`/task/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function updateTask(id: number, token: string, taskDto: any) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.put(`/task/${id}`, taskDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function deleteTask(id: number, token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.delete(`/task/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

// Project Api
export async function getAllProjects(token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.get("/project");
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function createProject(token: string, projectDto: any) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.post(`/project`, projectDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function getProjectById(id: number, token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.get(`/project/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function updateProject(id: number, token: string, projectDto: any) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.put(`/project/${id}`, projectDto);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}

export async function deleteProject(id: number, token: string) {
  try {
    if (token) {
      setAuthToken(token);
    }

    const { data } = await api.delete(`/project/${id}`);
    return data;
  } catch (error: unknown) {
    handleError(error);
  }
}