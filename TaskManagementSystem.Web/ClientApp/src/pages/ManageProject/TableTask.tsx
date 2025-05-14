import React from "react";
import { FaEdit, FaTrash } from "react-icons/fa";


interface Task {
    id: string;
    title: string;
    status: string;
    assignedTo: string;
    reviewBy: string;
    dueDate: string;
    description: string; 
  }

interface TaskTableProps {
    tasks: Task[];
}


const TaskTable: React.FC<TaskTableProps> = ({ tasks }) => {
    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        return new Intl.DateTimeFormat("id-ID", {
            day: "numeric",
            month: "long",
            year: "numeric",
        }).format(date);
    };

    return (
        <div className="overflow-x-auto">
            <table className="w-full bg-white dark:bg-dark rounded-lg shadow-md overflow-hidden border-collapse">
                <thead className="bg-gray-100 dark:bg-secondaryDark">
                    <tr>
                        <th className="text-left py-3 px-4 font-semibold text-xs md:text-sm text-gray-700 dark:text-white">Title</th>
                        <th className="text-left py-3 px-4 font-semibold text-xs md:text-sm text-gray-700 dark:text-white">Description</th>
                        <th className="text-left py-3 px-4 font-semibold text-xs md:text-sm text-gray-700 dark:text-white">Due Date</th>
                        <th className="text-left py-3 px-4 font-semibold text-xs md:text-sm text-gray-700 dark:text-white">Assigned To</th>
                        <th className="text-left py-3 px-4 font-semibold text-xs md:text-sm text-gray-700 dark:text-white">Reviewed By</th>
                        <th className="text-left py-3 px-4 font-semibold text-xs md:text-sm text-gray-700 dark:text-white">Status</th>
                        <th className="text-left py-3 px-4 font-semibold text-xs md:text-sm text-gray-700 dark:text-white">Action</th>
                    </tr>
                </thead>
                <tbody>
                    {tasks.map((task) => (
                        <tr key={task.id} className="border-b hover:bg-gray-50 dark:hover:bg-secondaryDark/30 transition">
                            <td className="py-3 px-4 text-[11px] md:text-sm text-gray-800 dark:text-white">{task.title}</td>
                            <td className="py-3 px-4 text-[11px] md:text-sm text-gray-800 dark:text-white">{task.description}</td>
                            <td className="py-3 px-4 text-[11px] md:text-sm text-gray-800 dark:text-white">{formatDate(task.dueDate)}</td>
                            <td className="py-3 px-4 text-[11px] md:text-sm text-gray-800 dark:text-white">{task.assignedTo}</td>
                            <td className="py-3 px-4 text-[11px] md:text-sm text-gray-800 dark:text-white">{task.reviewBy}</td>
                            <td className="py-3 px-4 text-[11px] md:text-sm text-gray-800 dark:text-white">{task.status}</td>
                            <td className="py-3 px-4 text-[11px] md:text-sm text-gray-800 dark:text-white">
                                <div className="flex items-center space-x-2">
                                <button onClick={() => alert("Edit Task")} className="cursor-pointer text-blue-500 hover:scale-110 transition"><FaEdit /></button>
                                <button onClick={() => alert("Delete Task")} className="cursor-pointer text-red-500 hover:scale-110 transition"><FaTrash /></button>
                                </div>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default TaskTable;
