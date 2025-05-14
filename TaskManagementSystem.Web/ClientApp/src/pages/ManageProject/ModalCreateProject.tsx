import React, { useState } from "react";
import { ProjectRequestDto, ProjectStatus, ProjectPriority } from "../../types/interfaces";
import { Input } from "../../components/ui/input";
import { Textarea } from "../../components/ui/textarea";
import { Button } from "../../components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "../../components/ui/card";
import { Label } from "../../components/ui/label";
import { FaPlusCircle } from "react-icons/fa";
import { useAuth } from "../../context/AuthContext";

interface CreateProjectModalProps {
    isOpen: boolean;
    onClose: () => void;
    onCreate: (newProject: ProjectRequestDto) => void;
}

const ModalCreateProject: React.FC<CreateProjectModalProps> = ({ isOpen, onClose, onCreate }) => {
    const { id } = useAuth()
    const [newProject, setNewProject] = useState<ProjectRequestDto>({
        name: "",
        description: "",
        dueDate: new Date(),
        projectStatus: ProjectStatus.NotStarted,
        projectPriority: ProjectPriority.Medium,
        creatorUserId: id ?? "",
    });

    const handleCreateProject = () => {
        onCreate(newProject);
        onClose();
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 px-2 bg-black/50 backdrop-blur-sm flex justify-center items-center z-50">
            <div className="absolute inset-0" aria-hidden="true" />
            <Card className="max-w-md w-full bg-white p-6 z-50">
                <CardHeader>
                    <CardTitle className="text-lg text-indigo-500/80 flex items-center gap-2">
                        <FaPlusCircle />
                        Create Project
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <form
                        onSubmit={(e) => {
                            e.preventDefault();
                            handleCreateProject();
                        }}
                        className="text-black/80"
                    >
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base'>Project Name</Label>
                            <Input
                                className='text-black/70 text-sm md:text-base border-indigo-500/70'
                                type="text"
                                value={newProject.name}
                                onChange={(e) =>
                                    setNewProject({ ...newProject, name: e.target.value })
                                }
                                autoFocus
                            />
                        </div>
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base'>Description</Label>
                            <Textarea
                                className='text-black/70 text-sm md:text-base border-indigo-500/70'
                                placeholder="Project Description"
                                value={newProject.description || ""}
                                onChange={(e) =>
                                    setNewProject({ ...newProject, description: e.target.value })
                                }
                            />
                        </div>
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base'>Due Date</Label>
                            <Input
                                className='text-black/70 text-sm md:text-base border-indigo-500/70'
                                type="date"
                                value={newProject.dueDate.toISOString().split("T")[0]}
                                onChange={(e) =>
                                    setNewProject({
                                        ...newProject,
                                        dueDate: new Date(e.target.value),
                                    })
                                }
                            />
                        </div>
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base'>Project Status</Label>
                            <select
                                value={newProject.projectStatus}
                                onChange={(e) =>
                                    setNewProject({
                                        ...newProject,
                                        projectStatus: e.target.value as ProjectStatus,
                                    })
                                }
                                className="w-full text-black/70 text-sm md:text-base border-indigo-500/70 py-2 px-3 cursor-pointer"
                            >
                                {Object.values(ProjectStatus).map((status) => (
                                    <option key={status} value={status}>
                                        {status}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base'>Project Priority</Label>
                            <select
                                value={newProject.projectPriority}
                                onChange={(e) =>
                                    setNewProject({
                                        ...newProject,
                                        projectPriority: e.target.value as ProjectPriority,
                                    })
                                }
                                className="w-full rounded-md text-black/70 text-sm md:text-base border-indigo-500/70 py-2 px-3 cursor-pointer"
                            >
                                {Object.values(ProjectPriority).map((priority) => (
                                    <option key={priority} value={priority}>
                                        {priority}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="md:flex md:justify-end gap-2 w-full mt-6">
                            <Button type="submit" className='mb-2 md:mb-0 w-full md:w-auto text-xs md:text-sm hover:bg-accent-hover hover:scale-105'>
                                Submit
                            </Button>
                            <Button type="button" onClick={onClose} variant="secondary" className='w-full md:w-auto text-xs md:text-sm bg-red-500 hover:bg-red-600 text-white hover:scale-105'>
                                Cancel
                            </Button>
                        </div>
                    </form>
                </CardContent>
            </Card>
        </div>
    );
};

export default ModalCreateProject;
