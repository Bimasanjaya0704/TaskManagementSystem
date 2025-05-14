"use client";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { RegisterFormSchema } from "../schemas/schemas";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, SubmitHandler } from "react-hook-form";
import { RegisterDto } from "../types/interfaces";
import { registerUser } from "../utils/api";
import { useNavigate } from "react-router-dom";
import { RegisterFormFields } from "../constants/formFields";
import { toast } from "sonner";
import { Button } from "./ui/button";

const RegisterForm: React.FC = () => {
  const navigate = useNavigate();

  const {
    handleSubmit,
    register,
    formState: { errors, isSubmitting },
  } = useForm<RegisterDto>({
    resolver: zodResolver(RegisterFormSchema),
    defaultValues: {
      username: "",
      firstName: "",
      lastName: "",
      email: "",
      password: "",
    },
  });

  const handleFormSubmit: SubmitHandler<RegisterDto> = async (data) => {
    try {
      await registerUser(data);
      toast.success("Registration Successful", {
        description: "Your account has been created successfully.",
        duration: 3000,
      });
      navigate("/login");
    } catch (err) {
      toast.error("Registration Failed", {
        description: (err as Error).message || "An error occurred during registration.",
        duration: 3000,
      });
    }
  };

  return (
    <div className="md:flex items-center justify-between px-4 pt-12 md:pt-0 md:px-24 gap-8 h-screen bg-gradient-to-r from-accents to-accent-hover rounded-lg dark:bg-primary">
      <div className="text-xl md:text-3xl lg:text-6xl text-center md:text-left mb-6 md:mb-0 font-extrabold text-white">
        Task Management System
      </div>
      <div className="w-full max-w-lg p-6 bg-white/10 backdrop-blur shadow-md rounded-lg">
        <h2 className="text-lg md:text-2xl font-bold text-center md:mb-8 text-white dark:text-accents">
          Register
        </h2>
        <form
          onSubmit={handleSubmit(handleFormSubmit)}
          className="mt-4 space-y-4"
        >
          {RegisterFormFields.map(
            ({ fieldName, fieldType, placeholder }, index) => (
              <div key={index} className="flex flex-col gap-2">
                <Label className="text-white" htmlFor={fieldName}>
                  {placeholder}
                </Label>
                <Input
                  type={fieldType}
                  id={fieldName}
                  {...register(fieldName)}
                  className="w-full border text-white border-gray-300 p-2 rounded-md focus:ring-2 focus:ring-accents"
                  disabled={isSubmitting}
                />
                {errors[fieldName] && (
                  <span className="text-red-500 text-sm">
                    {errors[fieldName]?.message}
                  </span>
                )}
              </div>
            )
          )}
          <Button
            type="submit"
            className="w-full bg-darkAccent hover:bg-darkAccent-hover"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Loading..." : "Register"}
          </Button>
        </form>
      </div>
    </div>
  );
};

export default RegisterForm;
