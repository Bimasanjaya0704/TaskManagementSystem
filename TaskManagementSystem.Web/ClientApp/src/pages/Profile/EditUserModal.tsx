import React, { useState } from 'react';
import { UserResponseDto } from '../../types/interfaces';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { FaEdit } from 'react-icons/fa';

interface EditUserModalProps {
    user: UserResponseDto;
    onUpdateUser: (updatedUser: UserResponseDto) => void;
    onCancel: () => void;
}

const EditUserModal: React.FC<EditUserModalProps> = ({ user, onUpdateUser, onCancel }) => {
    const [updateUSer, setUpdateUser] = useState<UserResponseDto>(user);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onUpdateUser(updateUSer);
    };

    return (
        <div className="fixed inset-0 px-2 bg-black/50 backdrop-blur-sm flex justify-center items-center z-50">
            <div className="absolute inset-0" aria-hidden="true" />
            <Card className="max-w-md w-full bg-white md:p-6 z-50">
                <CardHeader>
                    <CardTitle className='text-lg text-indigo-500/80 lg:text-[24px]'>
                        <div className='flex items-center gap-2'>
                            <FaEdit />Edit User</div>
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <form onSubmit={handleSubmit} className='text-black/78'>
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base'>First Name</Label>
                            <Input
                                className='text-black/70 text-sm md:text-base border-indigo-500/70'
                                type="text"
                                value={updateUSer.firstName}
                                onChange={(e) =>
                                    setUpdateUser({ ...updateUSer, firstName: e.target.value })
                                }
                                autoFocus
                            />
                        </div>
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base '>Last Name</Label>
                            <Input
                                className='text-black/70 text-sm md:text-base border-indigo-500/70'
                                type="text"
                                value={updateUSer.lastName}
                                onChange={(e) =>
                                    setUpdateUser({ ...updateUSer, lastName: e.target.value })
                                }
                            />
                        </div>
                        <div className="mb-4">
                            <Label className='mb-1.5 ml-0.5 text-black/70 text-sm md:text-base'>Email</Label>
                            <Input
                                className='text-black/70 text-sm md:text-base border-indigo-500/70'
                                type="email"
                                value={updateUSer.email}
                                onChange={(e) =>
                                    setUpdateUser({ ...updateUSer, email: e.target.value })
                                }
                            />
                        </div>
                        <div className="md:flex md:justify-end gap-2 w-full mt-6">
                            <Button type="submit" className='mb-2 md:mb-0 w-full md:w-auto text-xs md:text-sm hover:bg-accent-hover hover:scale-105'>
                                Update User
                            </Button>
                            <Button type="button" onClick={onCancel} variant="secondary" className='w-full md:w-auto text-xs md:text-sm bg-red-500 hover:bg-red-600 text-white hover:scale-105'>
                                Cancel
                            </Button>
                        </div>
                    </form>
                </CardContent>
            </Card>
        </div>
    );
};

export default EditUserModal;
