import { z } from "zod";
import { LoginFormSchema, RegisterFormSchema } from "../schemas/schemas";

export type LoginFormFieldsProps = {
  fieldName: keyof z.infer<typeof LoginFormSchema>;
  fieldType: "text" | "email" | "password";
  placeholder: string;
};

export type RegisterFormFieldsProps = {
  fieldName: keyof z.infer<typeof RegisterFormSchema>;
  fieldType: "text" | "email" | "password";
  placeholder: string;
};
