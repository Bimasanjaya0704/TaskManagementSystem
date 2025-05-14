import { use } from "react";
import { z } from "zod";

export const LoginFormSchema = z.object({
  email: z.string().email("Invalid email address"),
  password: z
    .string()
    .min(8, "Password must be at least 8 characters long")
    .max(16, "Password must be less than 16 characters"),
});

export const RegisterFormSchema = z.object({
  username: z.string(),
  firstName: z.string(),
  lastName: z.string(),
  email: z.string().email("Invalid email address"),
  password: z
    .string()
    .min(8, "Password must be at least 8 characters long")
    .max(16, "Password must be less than 16 characters"),
});
