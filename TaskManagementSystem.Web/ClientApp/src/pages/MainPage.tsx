import React, { useEffect, useState } from "react";
import HomePage from "./Home";
import LandingPage from "../components/Landing";

const MainPage: React.FC = () => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    setIsAuthenticated(!!token);
  }, []);

  return isAuthenticated ? <HomePage /> : <LandingPage />;
};

export default MainPage;
