import { useAuth } from "../context/AuthContext";
import { getUserById } from "../utils/api";

export const getUserData = async (id:number) => {
  try {
    const { token } = useAuth(); 
    if (token) {
      const response = await getUserById(id,token); 

      if (response.success) {
        return response.data; 
      } else {
        throw new Error(response.message || "Failed to fetch users.");  
      }
    } else {
      throw new Error("No authentication token found.");  
    }
  } catch (err: any) {
    console.error("Error fetching users:", err.message || err); 
    return null; 
  }
};
