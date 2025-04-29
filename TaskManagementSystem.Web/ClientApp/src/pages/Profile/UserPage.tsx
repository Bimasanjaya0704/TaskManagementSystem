import React, { useEffect, useState } from 'react';
import { updateUser, getUserById } from '../../utils/api';
import { UserResponseDto } from '../../types/interfaces';
import { useAuth } from '../../context/AuthContext';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Alert, AlertTitle, AlertDescription } from '../../components/ui/alert';
import { AlertCircle } from 'lucide-react';
import { LoadingIcon } from '../../components/LoadingIcon';
import EditUserModal from './EditUserModal';

const UserPage = () => {
    const [user, setUser] = useState<UserResponseDto | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [editMode, setEditMode] = useState<boolean>(false);
    const { token, id } = useAuth();

    useEffect(() => {
        const fetchUser = async () => {
            setLoading(true);
            setError(null);

            try {
                if (!token) throw new Error('Token is required to fetch user.');
                if (!id) throw new Error('Id is required to fetch user.');
                const ConvertIdToNumber = Number(id);
                const response = await getUserById(ConvertIdToNumber, token);
                setUser(response.data);
            } catch (err) {
                setError('Failed to fetch user. Please try again later.');
            } finally {
                setLoading(false);
            }
        };

        fetchUser();
    }, [token, id]);

    const handleEdit = () => setEditMode(true);

    const handleUpdateUser = async (updatedUser: UserResponseDto) => {
        setLoading(true);
        setError(null);

        try {
            if (!token) throw new Error('Token is required to update user.');
            const ConvertIdToNumber = Number(updatedUser.id);
            const response = await updateUser(ConvertIdToNumber, token, updatedUser);
            setUser(response.data);
            setEditMode(false);
        } catch (err) {
            setError('Failed to update user. Please try again later.');
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <LoadingIcon />;

    return (
        <div className={`md:p-6 ${editMode ? 'overflow-hidden' : ''}`}>
            {error && (
                <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>{error}</AlertDescription>
                </Alert>
            )}

            {user ? (
                <Card className="w-full max-w-[1000px] mx-auto">
                    <CardHeader>
                        <div className='flex items-center justify-between'><CardTitle className="text-lg text-black/80 lg:text-[30px]">Profile User</CardTitle>
                            <Button variant="default" onClick={handleEdit} className='hover:scale-105'>
                                Edit
                            </Button></div>
                    </CardHeader>
                    <CardContent className='flex-none md:flex justify-between'>
                        <div className="text-black/80 text-lg mb-6 md:mb-0 w-full md:max-w-[240px]">
                            {/* First Name */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">First Name :</div>
                                <div className="text-sm md:text-base">{user.firstName}</div>
                            </div>

                            {/* Last Name */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">Last Name :</div>
                                <div className="text-sm md:text-base">{user.lastName}</div>
                            </div>

                            {/* Full Name */}
                            <div className="md:flex items-center md:gap-4 mb-2 md:mb-0">
                                <div className="font-semibold md:font-bold text-sm md:text-base">Full Name :</div>
                                <div className="text-sm md:text-base">
                                    {user.firstName} {user.lastName}
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
                        <div className='flex-none md:flex justify-between items-center gap-4'>
                            <div className='px-10 py-4 bg-gradient-to-r from-accents to-accent-hover rounded-md mb-6 md:mb-0'>
                                <div className='text-white font-semibold text-xl'>Assigned Task</div>
                                <div className='text-white text-[50px] font-bold text-center'>{user.assignedTasks.length > 0 ? user.assignedTasks.length : 0}</div>
                            </div>
                            <div className='px-10 py-4 bg-gradient-to-r from-accent-hover to-accents rounded-md'>

                                <div className='text-white font-semibold text-xl'>Reviewed Task</div>
                                <div className='text-white text-[50px] font-bold text-center'>{user.reviewedTasks.length > 0 ? user.reviewedTasks.length : 0}</div>
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
