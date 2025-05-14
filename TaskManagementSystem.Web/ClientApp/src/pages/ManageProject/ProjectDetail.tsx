import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
    deleteUserFromMember,
    getProjectById,
    getProjectMember,
    getUserById,
    getUserByUsername,
    inviteUserToProject,
} from "../../utils/api";
import {
    ProjectResponseDto,
    Status,
    InviteUserToProjectDto,
    ProjectRole,
} from "../../types/interfaces";
import { LoadingIcon } from "../../components/LoadingIcon";
import {
    Alert,
    AlertDescription,
    AlertTitle,
} from "../../components/ui/alert";
import { AlertCircle } from "lucide-react";
import { useAuth } from "../../context/AuthContext";
import {
    Tabs,
    TabsList,
    TabsTrigger,
} from "../../components/ui/tabs";
import { TabsContent } from "@radix-ui/react-tabs";
import TaskTable from "./TableTask";
import { Button } from "../../components/ui/button";
import { FaPlus } from "react-icons/fa";
import { Input } from "../../components/ui/input";

const ProjectDetail = () => {
    const { projectId } = useParams<{ projectId: string }>();
    const { token } = useAuth();
    const [project, setProject] = useState<ProjectResponseDto | null>(null);
    const [members, setMembers] = useState<string[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [userNames, setUserNames] = useState<Record<string, string>>({});
    const [inviteUsername, setInviteUsername] = useState("");
    const [inviteLoading, setInviteLoading] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [usernameValid, setUsernameValid] = useState<boolean | null>(null);
    const [usernameDeleteValid, setUsernameDeleteValid] = useState<boolean | null>(null);
    const [deleteUsername, setDeleteUsername] = useState("");




    useEffect(() => {
        if (projectId && token) {
            fetchProjectDetail(projectId);
        }
    }, [projectId, token]);

    useEffect(() => {
        const fetchUserNames = async () => {
            if (!project || !token) return;

            const userIds = new Set<string>();
            project.tasks.forEach((task) => {
                if (task.assignedToUserId) userIds.add(task.assignedToUserId);
                if (task.reviewedToUserId) userIds.add(task.reviewedToUserId);
            });

            const names: Record<string, string> = { ...userNames };

            for (const id of userIds) {
                if (!names[id]) {
                    try {
                        const res = await getUserById(id);
                        names[id] = res.success
                            ? `${res.data.firstName} ${res.data.lastName}`
                            : "Unknown User";
                    } catch {
                        names[id] = "Error Fetching";
                    }
                }
            }

            setUserNames(names);
        };

        fetchUserNames();
    }, [project]);

    useEffect(() => {
        const fetchProjectMembers = async () => {
            if (!projectId) return;

            try {
                const response = await getProjectMember(projectId);
                if (response.success) {
                    const userIds = response.data.map(
                        (member: any) => member.user.userId
                    );
                    setMembers(userIds);
                } else {
                    console.error("Failed to get project members:", response.message);
                }
            } catch (error) {
                console.error("Failed to fetch members", error);
            }
        };
        fetchProjectMembers();
    }, [projectId]);

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

    const checkUsernameExists = async (username: string) => {
        if (!username) {
            setUsernameValid(null);
            return;
        }
        try {
            const res = await getUserByUsername(username);
            setUsernameValid(res.success);
        } catch {
            setUsernameValid(false);
        } finally {
        }
    };

    const checkUsernameDeleteExists = async (username: string) => {
        if (!username) {
            setUsernameDeleteValid(null);
            return;
        }
        try {
            const res = await getUserByUsername(username);
            setUsernameDeleteValid(res.success);
        } catch {
            setUsernameDeleteValid(false);
        } finally {
        }
    };


    const handleInviteUser = async () => {
        if (!inviteUsername || !projectId) return;
        try {
            setInviteLoading(true);
            const payload: InviteUserToProjectDto = {
                username: inviteUsername,
                projectId: projectId,
                projectRole: ProjectRole.Member,
            };
            const response = await inviteUserToProject(projectId, payload);
            if (response.success) {
                setInviteUsername("");
                const refreshed = await getProjectMember(projectId);
                if (refreshed.success) {
                    const userIds = refreshed.data.map((member: any) => member.user.userId);
                    setMembers(userIds);
                }
            } else {
            }
        } catch (error: any) {
        } finally {
            setInviteLoading(false);
        }
    };

    const handleDeleteMember = async () => {
        if (!projectId || !deleteUsername) return;

        try {
            setInviteLoading(true);
            const res = await getUserByUsername(deleteUsername);

            if (res.success) {
                const userId = res.data.userId;
                await deleteUserFromMember(projectId, userId);

                const refreshed = await getProjectMember(projectId);
                if (refreshed.success) {
                    const userIds = refreshed.data.map((member: any) => member.user.userId);
                    setMembers(userIds);
                }
                setDeleteUsername(""); // clear input
            }
        } catch (err) {
            console.error("Failed to delete member:", err);
        } finally {
            setInviteLoading(false);
        }
    };



    const transformTasksForTable = () =>
        project?.tasks?.map((task) => ({
            id: task.id,
            title: task.title || "No Title",
            status: task.status || "Unknown",
            assignedToUserId: userNames[task.assignedToUserId] || "Unknown",
            reviewedToUserId: task.reviewedToUserId,
            dueDate: task.dueDate
                ? new Date(task.dueDate).toISOString()
                : "No Due Date",
            description: task.description || "No Description",
        })) || [];

    const getTaskCountByStatus = (status: string) =>
        project?.tasks?.filter((task) => task.status === status).length || 0;

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

    if (loading) return <LoadingIcon />;

    if (error) {
        return (
            <Alert variant="destructive" className="mb-4">
                <AlertCircle className="h-4 w-4 mr-2" />
                <AlertTitle>Error</AlertTitle>
                <AlertDescription>{error}</AlertDescription>
            </Alert>
        );
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-accent-hover p-4 rounded-lg">
            <div className="container mx-auto max-w-6xl p-3 md:p-6 bg-white rounded-lg shadow-lg">
                <Tabs defaultValue="overview">
                    <div className="md:flex items-center justify-between gap-6">
                        <div className="md:flex items-center gap-6">
                            <TabsList className="md:flex gap-4 bg-indigo-700 rounded-lg p-4">
                                <TabsTrigger value="overview" className="px-4 py-3 rounded-lg text-white">
                                    Overview
                                </TabsTrigger>
                                <TabsTrigger value="manage" className="px-4 py-3 rounded-lg text-white">
                                    Manage
                                </TabsTrigger>
                            </TabsList>
                            {project ? (
                                <div className="text-2xl font-bold text-indigo-700 mt-2 md:mt-0">
                                    {project.name}
                                </div>
                            ) : (
                                <div>No project details available.</div>
                            )}
                        </div>

                        <div className="flex items-center gap-2">
                            {project && (
                                <>
                                    <div className={getPriorityColor(project.projectPriority || "")}>
                                        <span className="font-semibold">Priority: </span>
                                        <span>{project.projectPriority}</span>
                                    </div>
                                    <div className={getStatusColor(project.projectStatus || "")}>
                                        <span className="font-semibold">Status: </span>
                                        <span>{project.projectStatus}</span>
                                    </div>
                                    <Button onClick={() => setShowModal(true)} variant={"outline"} className="text-indigo-700">
                                        Manage Member
                                    </Button>
                                </>
                            )}
                        </div>
                    </div>
                    {showModal && (
                        <div className="fixed inset-0 bg-white/10 backdrop-blur-md flex items-center justify-center z-50">
                            <div className="bg-white p-6 rounded-lg w-[90%] max-w-md">
                                <div className="flex justify-between items-center">
                                    <h2 className="font-semibold mb-2 text-indigo-700 text-left text-xs ml-0.5"> + Invite Member</h2>

                                    <div className="font-semibold mb-2 text-indigo-700 text-left text-xs">
                                        {usernameValid === true && <p className="text-green-600">Username found!</p>}
                                        {usernameValid === false && <p className="text-red-600">User not found</p>}
                                    </div>
                                </div>


                                <Input
                                    type="text"
                                    placeholder="Enter username"
                                    value={inviteUsername}
                                    onChange={(e) => {
                                        const val = e.target.value;
                                        setInviteUsername(val);
                                        checkUsernameExists(val);
                                    }}
                                    className="w-full px-4 py-2 border rounded mb-2 text-indigo-700"
                                />

                                <div className="flex justify-end gap-2 mt-4 mb-6">
                                    <Button onClick={() => setShowModal(false)} variant="outline" className="text-indigo-700 border border-indigo-500 hover:bg-indigo-400 hover:text-white">Cancel</Button>
                                    <Button
                                        onClick={handleInviteUser}
                                        disabled={!usernameValid || inviteLoading}
                                        className="bg-indigo-600 text-white"
                                    >
                                        {inviteLoading ? "Inviting..." : "Invite"}
                                    </Button>
                                </div>

                                <div className="flex justify-between items-center">
                                    <h2 className="font-semibold mb-2 text-red-700 text-left text-xs ml-0.5"> - Delete Member</h2>

                                    <div className="font-semibold mb-2 text-indigo-700 text-left text-xs">
                                        {usernameDeleteValid === true && <p className="text-green-600">Username found!</p>}
                                        {usernameDeleteValid === false && <p className="text-red-600">User not found</p>}
                                    </div>
                                </div>


                                <Input
                                    type="text"
                                    placeholder="Enter username"
                                    value={inviteUsername}
                                    onChange={(e) => {
                                        const val = e.target.value;
                                        setInviteUsername(val);
                                        checkUsernameDeleteExists(val);
                                    }}
                                    className="w-full px-4 py-2 border rounded mb-2 text-indigo-700"
                                />

                                <div className="flex justify-end gap-2 mt-4">
                                    <Button onClick={() => setShowModal(false)} variant="outline" className="text-indigo-700 border border-indigo-500 hover:bg-indigo-400 hover:text-white">Cancel</Button>
                                    <Button
                                        onClick={handleDeleteMember}
                                        disabled={!usernameDeleteValid || inviteLoading}
                                        className="bg-red-600 text-white"
                                    >
                                        {inviteLoading ? "Deleting..." : "Delete"}
                                    </Button>
                                </div>
                            </div>
                        </div>
                    )}
                    <div className="md:px-2 my-4">
                        <TabsContent value="overview">
                            <div className="w-full lg:w-full md:flex gap-6">
                                {project && (
                                    <div className="w-full lg:w-1/3 flex flex-col gap-6">
                                        <div className="md:px-6 px-4 py-3 text-white bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg">
                                            <div className="font-semibold text-sm">Created by:</div>
                                            <div className="text-xs">{project.creatorUserId}</div>

                                            <div className="font-semibold text-sm mt-2">Members:</div>
                                            <div className="flex flex-col gap-1">
                                                {members.length > 0 ? (
                                                    members.map((userId, idx) => (
                                                        <div className="text-xs" key={idx}>{userId},</div>
                                                    ))
                                                ) : (
                                                    <div>No members found.</div>
                                                )}
                                            </div>
                                        </div>

                                        <div className="py-6 text-white text-center bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg">
                                            <div className="font-bold">Total Tasks:</div>
                                            <div className="text-[60px] font-semibold">{project.tasks.length}</div>
                                        </div>
                                    </div>
                                )}

                                <div className="w-full lg:w-2/3 mt-2 md:mt-0 md:px-6 px-4 py-3 text-white bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg">
                                    <div className="font-semibold text-base">Description:</div>
                                    <div className="text-sm">{project?.description}</div>
                                </div>
                            </div>

                            <div className="w-full mt-4 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 text-center p-3 md:p-6 text-white bg-indigo-700/90 backdrop-blur-md border-2 rounded-lg">
                                <div className="bg-white/20 p-4 rounded-lg">
                                    <div className="font-bold">{Status.New}</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">{getTaskCountByStatus(Status.New)}</div>
                                </div>
                                <div className="bg-white/20 p-4 rounded-lg">
                                    <div className="font-bold">{Status.InProgress}</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">{getTaskCountByStatus(Status.InProgress)}</div>
                                </div>
                                <div className="bg-white/20 p-4 rounded-lg">
                                    <div className="font-bold">{Status.PendingReview}</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">{getTaskCountByStatus(Status.PendingReview)}</div>
                                </div>
                                <div className="bg-white/20 p-4 rounded-lg">
                                    <div className="font-bold">{Status.OnHold}</div>
                                    <div className="text-[40px] md:text-[60px] font-semibold">{getTaskCountByStatus(Status.OnHold)}</div>
                                </div>
                            </div>
                        </TabsContent>

                        <TabsContent value="manage" className="w-full h-screen">
                            {project ? (
                                <div className="w-full">
                                    <div className="flex items-center justify-between mb-4">
                                        <div className="text-xl font-bold mb-4">Manage Tasks</div>
                                        <Button
                                            onClick={() => alert("Add Task")}
                                            className="hover:scale-105 transition-all duration-300"
                                        >
                                            <FaPlus /> Add Tasks
                                        </Button>
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
