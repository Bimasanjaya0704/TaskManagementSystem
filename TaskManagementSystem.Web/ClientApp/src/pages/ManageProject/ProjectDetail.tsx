import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getProjectById, getUserById } from "../../utils/api";
import { ProjectResponseDto } from "../../types/interfaces";
import { LoadingIcon } from "../../components/LoadingIcon";
import { Alert, AlertDescription, AlertTitle } from "../../components/ui/alert";
import { AlertCircle } from "lucide-react";
import { useAuth } from "../../context/AuthContext";
import { Tabs, TabsList, TabsTrigger } from "../../components/ui/tabs";
import { TabsContent } from "@radix-ui/react-tabs";
import TaskTable from "./TableTask";
import { Button } from "../../components/ui/button";
import { FaPlus } from "react-icons/fa";

const ProjectDetail = () => {
    const { projectId } = useParams<{ projectId: string }>();
    const { token } = useAuth();
    const [project, setProject] = useState<ProjectResponseDto | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [userNames, setUserNames] = useState<Record<string, string>>({});


    useEffect(() => {
        if (projectId && token) {
            fetchProjectDetail(projectId);
        }
    }, [projectId, token]);

    useEffect(() => {
        const fetchUserNames = async () => {
            if (!project || !token) return;

            const userIds = new Set<string>();

            project.tasks.forEach(task => {
                if (task.assignedToUserId) userIds.add(task.assignedToUserId);
                if (task.reviewedToUserId) userIds.add(task.reviewedToUserId);
            });

            const names: Record<string, string> = { ...userNames };

            for (const id of userIds) {
                if (!names[id]) {
                    try {
                        const res = await getUserById(id);
                        if (res.success) {
                            names[id] = res.data.firstName + " " + res.data.lastName;
                        } else {
                            names[id] = "Unknown User";
                        }
                    } catch {
                        names[id] = "Error Fetching";
                    }
                }
            }

            setUserNames(names);
        };

        fetchUserNames();
    }, [project]);



    const fetchProjectDetail = async (projectId: string) => {
        try {
            setLoading(true);
            setError(null);
            if (token) {
                const response = await getProjectById(projectId);
                if (response.success) {
                    setProject(response.data);
                } else {
                    throw new Error(response.message || "Failed to fetch project details.");
                }
            }
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const transformTasksForTable = () => {
        return project?.tasks?.map((task) => ({
            id: task.id,
            title: task.title || "No Title",
            status: task.status || "Unknown",
            assignedToUserId: userNames[task.assignedToUserId] || "Unknown",
            reviewedToUserId: task.reviewedToUserId,
            dueDate: task.dueDate ? new Date(task.dueDate).toISOString() : "No Due Date",
            description: task.description || "No Description",
        })) || [];
    };



    const getPriorityColor = (priority: string) => {
        switch (priority) {
            case "High":
                return "text-white px-4 py-1 bg-red-500 rounded-xl text-[9px] md:text-sm";
            case "Medium":
                return "text-black/80 px-4 py-1 bg-yellow-300 rounded-xl text-[9px] md:text-sm";
            case "Low":
                return "text-white px-4 py-1 bg-green-300 rounded-xl text-[9px] md:text-sm";
            default:
                return "text-black/80 px-4 py-1 bg-gray-200 rounded-xl text-[9px] md:text-sm";
        }
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case "NotStarted":
                return "text-black/80 px-4 py-1 bg-gray-300 rounded-xl text-[9px] md:text-sm";
            case "InProgress":
                return "text-white px-4 py-1 bg-blue-500 rounded-xl text-[9px] md:text-sm";
            case "Done":
                return "text-white px-4 py-1 bg-green-500 rounded-xl text-[9px] md:text-sm";
            case "OnHold":
                return "text-white px-4 py-1 bg-orange-500 rounded-xl text-[9px] md:text-sm";
            default:
                return "text-black/80 px-4 py-1 bg-gray-200 rounded-xl text-[9px] md:text-sm";
        }
    };

    // Loading State
    if (loading) return <LoadingIcon />;

    // Error State
    if (error) {
        return (
            <Alert variant="destructive" className="mb-4">
                <AlertCircle className="h-4 w-4 mr-2" />
                <AlertTitle>Error</AlertTitle>
                <AlertDescription>{error}</AlertDescription>
            </Alert>
        );
    }

    // Main Content
    return (
        <div className="min-h-screen flex items-center justify-center bg-accent-hover p-4 rounded-lg">
            <div className="container mx-auto max-w-6xl p-3 md:p-6 bg-white rounded-lg shadow-lg">
                <Tabs defaultValue="overview">
                    <div className="md:flex items-center justify-between gap-6">
                        <div className="md:flex items-center gap-6">
                            <TabsList className="md:flex gap-4 bg-indigo-700 rounded-lg p-4">
                                <TabsTrigger value="overview" className="px-4 py-3 rounded-lg text-white">Overview</TabsTrigger>
                                <TabsTrigger value="manage" className="px-4 py-3 rounded-lg text-white">Manage</TabsTrigger>
                            </TabsList>

                            {project ? (
                                <div className="text-2xl font-bold text-indigo-700 order-1 md:order-none mt-2 md:mt-0">{project.name}</div>
                            ) : (
                                <div>No project details available.</div>
                            )}
                        </div>

                        <div className="flex items-center gap-2">
                            {project ? (
                                <>
                                    <div className={getPriorityColor(project.projectPriority || "")}>
                                        <span className="font-semibold">Priority: </span>
                                        <span>{project.projectPriority}</span>
                                    </div>
                                    <div className={getStatusColor(project.projectStatus || "")}>
                                        <span className="font-semibold">Status: </span>
                                        <span>{project.projectStatus}</span>
                                    </div>
                                </>
                            ) : (
                                <div></div>
                            )}
                        </div>
                    </div>

                    <div className="md:px-2 my-4">
                        <TabsContent value="overview">
                            <div className="w-full lg:w-full md:flex gap-6">
                                {project ? (
                                    <div className="w-full lg:w-1/3 flex flex-col gap-2 md:gap-6">
                                        <div className="md:px-6 px-4 py-3 md:py-4 text-white bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg order-1">
                                            <div className="font-semibold">Created by:</div>
                                            <div>{project.creatorUserId}</div>
                                        </div>
                                        <div className="md:py-6 py-3 text-white text-center bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg order-3">
                                            <div className="font-bold">Total Tasks:</div>
                                            <div className="text-[60px] font-semibold">{project.tasks.length}</div>
                                        </div>
                                    </div>
                                ) : (
                                    <div>No project details available.</div>
                                )}

                                <div className="w-full lg:w-2/3 mt-2 md:mt-0 md:px-6 px-4 py-3 text-white bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg order-2">
                                    <div className="font-semibold">Description:</div>
                                    <div>{project?.description}</div>
                                </div>
                            </div>

                            <div className="w-full mt-2 md:mt-4 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 text-center p-3 md:p-6 text-white bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg">
                                <div className="bg-white/20 backdrop-blur-md p-2 md:p-4 rounded-lg">
                                    <div className="font-bold">Task New</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">12</div>
                                </div>
                                <div className="bg-white/20 backdrop-blur-md p-2 md:p-4 rounded-lg">
                                    <div className="font-bold">Task InProgress</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">10</div>
                                </div>
                                <div className="bg-white/20 backdrop-blur-md p-2 md:p-4 rounded-lg">
                                    <div className="font-bold">Task Done</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">9</div>
                                </div>
                                <div className="bg-white/20 backdrop-blur-md p-2 md:p-4 rounded-lg">
                                    <div className="font-bold">Task OnHold</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">2</div>
                                </div>
                            </div>
                        </TabsContent>

                        <TabsContent value="manage" className="w-full h-screen">
                            {project ? (
                                <div className="w-full">
                                    <div className="flex items-center justify-between mb-4">
                                        <div className="text-xl font-bold mb-4">Manage Tasks</div>
                                        <Button onClick={() => alert("Add Task")} className="hover:scale-105 transition-all duration-300"> <FaPlus /> Add Tasks</Button>
                                    </div>
                                    <TaskTable tasks={transformTasksForTable()} />
                                </div>
                            ) : (
                                <div>No tasks available.</div>
                            )}
                        </TabsContent>
                    </div>
                </Tabs>
            </div>
        </div>
    );
};

export default ProjectDetail;