"use client";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { LoginFormSchema } from "../schemas/schemas";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, SubmitHandler } from "react-hook-form";
// import { z } from "zod";
import { LoginFormFields } from "../constants/formFields";
import { login, setAuthToken } from "../utils/api";
import { LoginRequestDto } from "../types/interfaces";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { toast } from "sonner";
import { FaGooglePlusG } from "react-icons/fa";
import { Button } from "./ui/button";

const LoginForm: React.FC = () => {
  const navigate = useNavigate();
  const { login: loginAuth, token } = useAuth();

  const {
    handleSubmit,
    register,
    formState: { errors, isSubmitting },
  } = useForm<LoginRequestDto>({
    resolver: zodResolver(LoginFormSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const handleFormSubmit: SubmitHandler<LoginRequestDto> = async (data) => {
    try {
      const response = await login(data);
      loginAuth(response.token, response.user.role, response.user.firstName, response.user.id);
      setAuthToken(token);
      toast.success("Login Successful", {
        description: "Your account has been Login.",
        duration: 2000,
      });
      navigate("/");
    } catch (err) {
      toast.error("Login Failed", {
        description: (err as Error).message || "An error occurred during Login.",
        duration: 2000,
      });
    }
  };

  return (
    <div className="md:flex items-center justify-between px-4 pt-12 md:pt-0 md:px-24 gap-8 h-screen bg-gradient-to-r from-accents to-accent-hover rounded-lg dark:bg-primary">
      <div className="text-xl md:text-3xl lg:text-6xl text-center md:text-left mb-6 md:mb-0 font-extrabold text-white">
        Task Management System
      </div>
      <div className="w-full max-w-lg p-6 bg-white/10 backdrop-blur shadow-md rounded-lg">
        <h2 className="text-lg md:text-2xl font-bold text-center md:mb-6 text-white dark:text-accents">
          Login
        </h2>
        <form
          onSubmit={handleSubmit(handleFormSubmit)}
          className="mt-4 space-y-4"
        >
          {LoginFormFields.map(
            ({ fieldName, fieldType, placeholder }, index) => (
              <div key={index} className="flex flex-col gap-2">
                <Label className="text-white" htmlFor={fieldName}>
                  {placeholder}
                </Label>
                <Input
                  type={fieldType}
                  id={fieldName}
                  {...register(fieldName)}
                  className="w-full border focus:bg-white text-white focus:text-black/70 focus:border-accents p-2 rounded-md focus:ring-2 focus:ring-accents text-white"
                  disabled={isSubmitting}
                  placeholder={fieldName === "email" ? "user@mail.com" : ""}
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
            className="w-full bg-darkAccent hover:bg-darkAccent-hover text-sm md:text-base"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Loading..." : "Login"}
          </Button>


        </form>
        <Button
          className="w-full text-black/80 text-sm md:text-base flex justify-center gap-2 mt-4"
          variant="outline"
        >
          Login with Google <FaGooglePlusG className="bg-accents rounded-full text-lg text-white p-1" />
        </Button>

        <div className="mt-4 text-center text-sm text-white">
          Don&apos;t have an account?{" "}
          <a href="/register" className="underline underline-offset-4">
            Sign up
          </a>
        </div>
      </div>
    </div>
  );
};

export default LoginForm;
