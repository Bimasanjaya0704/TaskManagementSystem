import {
  createContext,
  useState,
  useContext,
  ReactNode,
  useEffect
} from "react";
import { jwtDecode } from "jwt-decode";
import { ROLE } from "../types/roles";

interface AuthState {
  isAuthenticated: boolean;
  role: ROLE | null;
  token: string | null;
  userName: string | null;
  id: string | null;
}

interface AuthContextType extends AuthState {
  login: (token: string, role: ROLE, userName: string, id: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [auth, setAuth] = useState<AuthState>({
    isAuthenticated: !!localStorage.getItem("token"),
    role: localStorage.getItem("role") as ROLE | null,
    token: localStorage.getItem("token"),
    userName: localStorage.getItem("userName"),
    id: localStorage.getItem("id")
  });

  useEffect(() => {
    const checkTokenExpiration = () => {
      const token = localStorage.getItem("token");
      if (!token) return;

      try {
        interface DecodedToken {
          exp: number;
        }
        const decoded: DecodedToken = jwtDecode<DecodedToken>(token);
        const exp = decoded.exp * 1000;
        const now = Date.now();

        if (now >= exp) {
          logout();
        } else {
          setTimeout(logout, exp - now);
        }
      } catch (error) {
        console.error("Invalid token:", error);
        logout();
      }
    };

    checkTokenExpiration();
  }, []);

  const login = (token: string, role: ROLE, userName: string, id: string) => {
    localStorage.setItem("token", token);
    localStorage.setItem("role", role);
    localStorage.setItem("userName", userName);
    localStorage.setItem("id", id);

    setAuth({
      isAuthenticated: true,
      role,
      token,
      userName,
      id,
    });

    interface DecodedToken {
      exp: number;
    }
    const decoded: DecodedToken = jwtDecode<DecodedToken>(token);
    const exp = decoded.exp * 1000;
    const now = Date.now();

    if (exp > now) {
      setTimeout(logout, exp - now);
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("userName");
    localStorage.removeItem("id");

    setAuth({
      isAuthenticated: false,
      role: null,
      token: null,
      userName: null,
      id: null,
    });

    window.location.href = "/login";
  };

  return (
    <AuthContext.Provider value={{ ...auth, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
};