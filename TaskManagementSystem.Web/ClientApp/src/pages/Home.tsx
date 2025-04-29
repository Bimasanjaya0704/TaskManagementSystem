import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { Button } from "../components/ui/button";

const HomePage: React.FC = () => {
  const { userName } = useAuth();
  const navigate = useNavigate();

  const taskStats = {
    total: 12,
    completed: 5,
    inProgress: 4,
    pending: 3,
  };

  return (
    <div className="relative flex items-center justify-center rounded-xl min-h-screen bg-gradient-to-r from-accents to-accent-hover text-white px-6">

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8 w-full h-full bg-white/10 backdrop-blur-md p-8 rounded-2xl shadow-lg">
        <div className="flex flex-col justify-center">
          <h1 className="text-4xl font-bold mb-2">Welcome, {userName}!</h1>
          <p className="text-lg text-gray-200 mb-6">
            Manage your tasks easily with the{" "}
            <span className="font-bold text-white">Task Management System</span>
            . Organize priorities and boost productivity with an intuitive and
            collaborative system.
          </p>
          <Button
            variant="outline"
            onClick={() => navigate("/projects")}
            className="px-6 py-3 hover:scale-105 bg-accents border border-white text-white hover:text-white font-semibold rounded-lg shadow-md hover:bg-accent-hover transition"
          >
            Manage Projects
          </Button>
        </div>

        <div className="flex flex-col items-center">
          <h2 className="text-2xl font-semibold mb-4">Task Statistics</h2>
          <div className="grid grid-cols-2 gap-6 md:grid-cols-2 bg-white/20 p-6 rounded-lg shadow-md w-full">
            <div className="text-center">
              <p className="text-2xl font-bold">{taskStats.total}</p>
              <p className="text-gray-300">Total Tasks</p>
            </div>
            <div className="text-center">
              <p className="text-2xl font-bold text-green-400">
                {taskStats.completed}
              </p>
              <p className="text-gray-300">Completed</p>
            </div>
            <div className="text-center">
              <p className="text-2xl font-bold text-yellow-400">
                {taskStats.inProgress}
              </p>
              <p className="text-gray-300">In Progress</p>
            </div>
            <div className="text-center">
              <p className="text-2xl font-bold text-red-400">
                {taskStats.pending}
              </p>
              <p className="text-gray-300">Pending</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
