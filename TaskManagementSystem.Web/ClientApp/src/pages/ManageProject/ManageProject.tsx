import React, { useEffect, useState } from "react";
import { getAllProjects, createProject } from "../../utils/api";
import { useAuth } from "../../context/AuthContext";
import { ProjectRequestDto, ProjectResponseDto } from "../../types/interfaces";
import { Button } from "../../components/ui/button";
import { Card, CardContent } from "../../components/ui/card";
import ModalCreateProject from "./ModalCreateProject";
import { FaList, FaPlus } from "react-icons/fa";
import { get } from "http";
import { LoadingIcon } from "../../components/LoadingIcon";
import { Alert, AlertDescription, AlertTitle } from "../../components/ui/alert";
import { AlertCircle } from "lucide-react";
import { useNavigate } from "react-router-dom";

const ManageProject = () => {
    const [projects, setProjects] = useState<ProjectResponseDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [showCreateProjectModal, setShowCreateProjectModal] = useState(false);
    const { token, id } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        fetchProjects();
    }, [id]);

    const fetchProjects = async () => {
        try {
            if (!token) throw new Error("Token is required.");
            const response = await getAllProjects(token);
            console.log("Projects Data:", response);

            if (response.success && Array.isArray(response.data)) {
                setProjects(response.data);
            } else {
                throw new Error("Invalid data format: Expected an array.");
            }
            setLoading(false);
        } catch (err: any) {
            setError(err.message);
        }
    };

    const handleCreateProject = async (newProject: ProjectRequestDto) => {
        try {
            if (!token) throw new Error("Token is required.");
            await createProject(token, newProject);
            fetchProjects();
        } catch (err: any) {
            setError(err.message);
        }
    };
    const getPriorityColor = (priority: string) => {
        switch (priority) {
            case "High":
                return "text-white bg-red-500 rounded-xl text-[9px] md:text-xs";
            case "Medium":
                return "text-trasnparant bg-yellow-300 rounded-xl text-[9px] md:text-xs";
            case "Low":
                return "text-trasnparant bg-green-300 rounded-xl text-[9px] md:text-xs";
            default:
                return "text-trasnparant bg-gray-200 rounded-xl text-[9px] md:text-xs";
        }
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case "NotStarted":
                return "text-black/80 bg-gray-300 rounded-xl text-[9px] md:text-xs";
            case "InProgress":
                return "text-white bg-blue-500 rounded-xl text-[9px] md:text-xs";
            case "Done":
                return "text-white bg-green-500 rounded-xl text-[9px] md:text-xs";
            case "OnHold":
                return "text-white bg-orange-500 rounded-xl text-[9px] md:text-xs";
            default:
                return "text-black/80 bg-gray-200 rounded-xl text-[9px] md:text-xs";
        }
    };


    return (
        <div className=" py-4 px-3 md:p-6 rounded-xl items-center justify-center min-h-screen bg-gradient-to-r from-indigo-600 to-accents">
            <div className="md:flex justify-between items-center">
                <div className="flex items-center gap-2 text-xl md:text-2xl text-white font-bold">
                    <FaList />
                    <div>List Projects</div>
                </div>
                <Button onClick={() => setShowCreateProjectModal(true)} className="mt-4 md:mt-0 text-xs md:text-base mb-4 hover:scale-105 bg-white text-indigo-600">
                    <FaPlus />
                    Create Project</Button>
            </div>
            {error && (
                <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>{error}</AlertDescription>
                </Alert>
            )}

            {loading && <LoadingIcon />}
            <ModalCreateProject
                isOpen={showCreateProjectModal}
                onClose={() => setShowCreateProjectModal(false)}
                onCreate={handleCreateProject}
            />

            {projects.length > 0 ? (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3 md:gap-6 mt-2 md:mt-6">
                    {projects.map((project) => (
                        <Card
                            key={project.id}
                            onClick={() => navigate(`/project/${project.id}`)}
                            className="bg-white/20 hover:scale-105 cursor-pointer transition-transform duration-300 hover:bg-white/30 duration-500 backdrop-blur-md"
                        >
                            <CardContent className="px-3 md:px-6">
                                <div className="flex justify-between items-center">
                                    <div className="text-sm md:text-xl font-bold mb-2 text-white">{project.name || "N/A"}</div>
                                    <div className={`inline-block text-xs md:text-base px-2 py-1 rounded ${getPriorityColor(project.projectPriority || "")}`}>
                                        {project.projectPriority || "N/A"}
                                    </div></div>
                                <div className="text-white text-xs md:text-base mb-1 overflow-hidden text-ellipsis whitespace-nowrap">
                                    {project.description?.length > 100 ? `${project.description.slice(0, 100)}...` : project.description || "N/A"}
                                </div>



                                <div className="flex items-center justify-between mt-4">
                                    <div className={`inline-block text-xs md:text-base px-2 py-1 rounded ${getStatusColor(project.projectStatus || "")}`}>
                                        <strong>Status:</strong> {project.projectStatus || "N/A"}
                                    </div>
                                    <div className="text-white text-[9px] md:text-xs">
                                        {project.dueDate ? new Date(project.dueDate).toLocaleDateString() : "Invalid Date"}
                                    </div>
                                </div>
                            </CardContent>
                        </Card>
                    ))}
                </div>
            ) : (
                <p className="text-gray-300">No projects available.</p>
            )
            }

        </div >
    );
};

export default ManageProject;
