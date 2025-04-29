import { Navigate, Outlet } from "react-router-dom";
import { ROLE } from "../types/roles";
import { useAuth } from "../context/AuthContext";

interface ProtectedRouteProps {
  allowedRoles: ROLE[];
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ allowedRoles }) => {
  const { isAuthenticated, role } = useAuth();

  console.log("User Role:", role);
  console.log("Allowed Roles:", allowedRoles);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!allowedRoles.includes(role as ROLE)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <Outlet />;
};

export default ProtectedRoute;
