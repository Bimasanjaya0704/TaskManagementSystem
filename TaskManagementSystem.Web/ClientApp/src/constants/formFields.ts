import {
  LoginFormFieldsProps,
  RegisterFormFieldsProps,
} from "../types/FormTypes";

export const LoginFormFields: LoginFormFieldsProps[] = [
  {
    fieldName: "email",
    fieldType: "email",
    placeholder: "Email",
  },
  {
    fieldName: "password",
    fieldType: "password",
    placeholder: "Password",
  },
];

export const RegisterFormFields: RegisterFormFieldsProps[] = [
   {
    fieldName: "username",
    fieldType: "text",
    placeholder: "username",
  },
  {
    fieldName: "firstName",
    fieldType: "text",
    placeholder: "firstName",
  },
  {
    fieldName: "lastName",
    fieldType: "text",
    placeholder: "lastName",
  },
  {
    fieldName: "email",
    fieldType: "email",
    placeholder: "Email",
  },
  {
    fieldName: "password",
    fieldType: "password",
    placeholder: "Password",
  },
];
