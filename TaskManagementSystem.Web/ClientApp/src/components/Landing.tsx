import React from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "./ui/button";

const LandingPage: React.FC = () => {
  const navigate = useNavigate();

  const handleLogin = () => {
    navigate("/login");
  };

  const handleRegister = () => {
    navigate("/register");
  };

  return (
    <div className="flex flex-col rounded-xl items-center justify-center min-h-screen bg-gradient-to-r from-accents to-accent-hover text-white px-6">
      <div className="text-center max-w-4xl bg-white/10 backdrop-blur-md p-8 rounded-2xl shadow-lg">
        <h1 className="text-4xl font-bold mb-4">Task Management System</h1>
        <p className="text-lg text-gray-200 mb-6">
          Organize your tasks{" "}
          <span className="font-bold text-white">effortlessly</span> and{" "}
          <span className="font-bold text-white">boost productivity</span>.
          Easily create, organize, and track your tasks with an intuitive
          interface, collaborate with your team in real-time, and stay on top of
          deadlines. Best of all, it’s{" "}
          <span className="font-bold text-accent">100% free</span> with no
          hidden fees! Get started by logging in or creating an account.
        </p>

        <div className="flex gap-4 justify-center">
          <Button
            onClick={handleLogin}
            className="px-6 py-3 hover:scale-105 bg-accents border border-white text-white font-semibold rounded-lg shadow-md hover:bg-accent-hover transition"
          >
            Login
          </Button>
          <Button
            onClick={handleRegister}
            className="px-6 py-3 hover:scale-105 bg-accents border border-white text-white font-semibold rounded-lg shadow-md hover:bg-accent-hover transition"
          >
            Register
          </Button>
        </div>
      </div>
    </div>
  );
};

export default LandingPage;
