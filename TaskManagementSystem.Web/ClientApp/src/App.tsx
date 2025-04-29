import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./components/LoginForm";
import Layout from "./Layout";
import Register from "./components/RegisterForm";
import { AuthProvider } from "./context/AuthContext";
import MainPage from "./pages/MainPage";
import ProtectedRoute from "./lib/ProtectedRoute";
import { ROLE } from "./types/roles";
import UnauthorizedPage from "./components/NoAuthorization";
import About from "./pages/About";
import Contact from "./pages/Contact";
import UserPage from "./pages/Profile/UserPage";
import ManageProject from "./pages/ManageProject/ManageProject";
import DetailProject from "./pages/ManageProject/ProjectDetail";
import ProjectDetail from "./pages/ManageProject/ProjectDetail";

const App: React.FC = () => {
  return (
    <AuthProvider>
      <div className="bg-light dark:bg-dark font-sans px-4 md:px-10">
        <Router>
          <Layout>
            <Routes>
              <Route path="/" element={<MainPage />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              {/* <Route element={<ProtectedRoute allowedRoles={[ROLE.User]} />}>
                <Route path="/manage-task" element={<ManageTask />} />
              </Route> */}
              <Route path="/unauthorized" element={<UnauthorizedPage />} />
              <Route path="/about" element={<About />} />
              <Route path="/contact" element={<Contact />} />
              <Route element={<ProtectedRoute allowedRoles={[ROLE.User]} />}>
                <Route path="/profile" element={<UserPage />} />
              </Route>
              <Route element={<ProtectedRoute allowedRoles={[ROLE.User]} />}>
                <Route path="/projects" element={<ManageProject />} />
              </Route>
              <Route element={<ProtectedRoute allowedRoles={[ROLE.User]} />}>
                <Route path="/project/:id" element={<ProjectDetail />} />
              </Route>
            </Routes>
          </Layout>
        </Router>
      </div>
    </AuthProvider>
  );
};

export default App;
