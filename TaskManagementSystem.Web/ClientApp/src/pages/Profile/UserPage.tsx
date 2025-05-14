import { useEffect, useState } from 'react';
import { updateUser, getUserById, getAssignedTasks, getReviewedTasks } from '../../utils/api';
import { UserResponseDto } from '../../types/interfaces';
import { useAuth } from '../../context/AuthContext';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { LoadingIcon } from '../../components/LoadingIcon';
import EditUserModal from './EditUserModal';
import { toast } from "sonner";

const UserPage = () => {
    const [user, setUser] = useState<UserResponseDto | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const [editMode, setEditMode] = useState<boolean>(false);
    const { token, id } = useAuth();

    useEffect(() => {
        const fetchUser = async () => {
            setLoading(true);

            try {
                if (!token) throw new Error('Token is required to fetch user.');
                if (!id) throw new Error('Id is required to fetch user.');
                const response = await getUserById(id);
                const assignedTask = await getAssignedTasks(id);
                const reviewedTask = await getReviewedTasks(id);
                response.data.assignedTasks = assignedTask.data;
                response.data.reviewedTasks = reviewedTask.data;
                console.log(response);
                setUser(response.data);
            } catch (err) {
                let errorMessage = "An unexpected error occurred";
                if (err instanceof Error) {
                    errorMessage = err.message;
                }
                toast.error("Error Fetching User", {
                    description: errorMessage,
                    duration: 3000,
                });
            } finally {
                setLoading(false);
            }
        };

        fetchUser();
    }, [token, id]);

    const handleEdit = () => setEditMode(true);

    const handleUpdateUser = async (updatedUser: UserResponseDto) => {
        setLoading(true);
        try {
            if (!token) throw new Error('Token is required to update user.');

            const payload = {
                firstName: updatedUser.firstName,
                lastName: updatedUser.lastName,
                username: updatedUser.username,
            };

            const response = await updateUser(updatedUser.userId, payload);
            setUser(response.data);
            setEditMode(false);
            toast.success("Update Successful", {
                description: "Your account has been updated.",
                duration: 3000,
            });
        } catch (err) {
            let errorMessage = "An unexpected error occurred";
            if (err instanceof Error) {
                errorMessage = err.message;
            }
            toast.error("Error Update User", {
                description: errorMessage,
                duration: 3000,
            });
        } finally {
            setLoading(false);
        }
    };


    if (loading) return <LoadingIcon />;

    return (
        <div className={`md:p-6 ${editMode ? 'overflow-hidden' : ''}`}>
            {user ? (
                <Card className="w-full max-w-[1000px] mx-auto">
                    <CardHeader>
                        <div className='flex items-center justify-between'><CardTitle className="text-lg text-black/80 lg:text-[30px]">Profile User</CardTitle>
                            <Button variant="default" onClick={handleEdit} className='hover:scale-105'>
                                Edit
                            </Button></div>
                    </CardHeader>
                    <CardContent className='flex-none md:flex justify-between'>
                        <div className="text-black/80 text-lg px-4 mb-6 md:mb-0 w-full md:max-w-[240px]">
                            {/* Username */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">Username :</div>
                                <div className="text-sm md:text-base">{user.username}</div>
                            </div>

                            {/* First Name */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">First Name :</div>
                                <div className="text-sm md:text-base">{user.firstName}</div>
                            </div>

                            {/* Last Name */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">Last Name :</div>
                                <div className="text-sm md:text-base">
                                    {user.lastName}
                                </div>
                            </div>

                            {/* Email */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">Email :</div>
                                <div className="text-sm md:text-base">{user.email}</div>
                            </div>

                            {/* Role */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">Role :</div>
                                <div className="text-sm md:text-base">{user.role}</div>
                            </div>
                        </div>
                        <div className='px-10 py-4 bg-gradient-to-r from-accents to-accent-hover rounded-md mb-6 md:mb-0'>
                            <div className='text-white font-semibold text-xl'>Assigned Task</div>
                            <div className='text-white text-[50px] font-bold text-center'>
                                {user.assignedTasks?.length || 0}
                            </div>
                        </div>
                        <div className='px-10 py-4 bg-gradient-to-r from-accent-hover to-accents rounded-md'>
                            <div className='text-white font-semibold text-xl'>Reviewed Task</div>
                            <div className='text-white text-[50px] font-bold text-center'>
                                {user.reviewedTasks?.length || 0}
                            </div>
                        </div>

                    </CardContent>
                </Card>
            ) : null
            }

            {
                editMode && user && (
                    <EditUserModal
                        user={user}
                        onUpdateUser={handleUpdateUser}
                        onCancel={() => setEditMode(false)}
                    />
                )
            }

            {!user && !editMode && <p>No user data available.</p>}
        </div >
    );
};

export default UserPage;
