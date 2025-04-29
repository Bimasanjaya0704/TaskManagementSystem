import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuGroup,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "../components/ui/dropdown-menu"
import { MdPeople } from "react-icons/md"
import { useNavigate } from "react-router-dom";
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "./ui/dialog";
import { useAuth } from "../context/AuthContext";
import { Button } from "./ui/button";

export function ProfileBtn() {
    const navigate = useNavigate();
    const { isAuthenticated, logout } = useAuth();

    const handleLogin = () => {
        navigate("/login");
    };

    const handleRegister = () => {
        navigate("/register");
    };

    const handleProfile = () => {
        navigate("/profile");
    };

    const handleLogout = () => {
        logout();
        navigate("/");
    };

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button className="rounded-full"><MdPeople className="text-xl" /></Button>
            </DropdownMenuTrigger>
            {isAuthenticated ? <DropdownMenuContent className="w-56">
                <DropdownMenuLabel>My Account</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                    <DropdownMenuItem className="cursor-pointer" onClick={handleProfile}>
                        Profile
                    </DropdownMenuItem>
                </DropdownMenuGroup>
                <Dialog>
                    <DialogTrigger>
                        <div className="flex items-center rounded-sm cursor-pointer gap-2 text-sm px-2 py-1 hover:bg-red-100 transition-all w-[228px]">
                            Log out
                        </div>
                    </DialogTrigger>
                    <DialogContent className="bg-white/10 backdrop-blur-md text-white">
                        <DialogHeader>
                            <DialogTitle className="text-white text-sm md:text-xl">Are you sure to Logout of your account?</DialogTitle>
                        </DialogHeader>
                        <DialogFooter className="sm:justify-start mt-4 gap-4">
                            <DialogClose asChild>
                                <Button
                                    type="button"
                                    variant="secondary"
                                    className="hover:scale-105 text-xs md:text-sm w-full lg:w-[220px]"
                                >
                                    Cancel
                                </Button>
                            </DialogClose>
                            <Button
                                onClick={handleLogout}
                                className="hover:scale-105 text-xs md:text-sm max-w-md lg:w-[220px]"
                            >
                                Yes
                            </Button>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
            </DropdownMenuContent> : <DropdownMenuContent className="w-56">
                <DropdownMenuLabel>Welcome Guest!</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                    <DropdownMenuItem className="cursor-pointer" onClick={handleLogin}>
                        Login
                    </DropdownMenuItem>
                </DropdownMenuGroup>
                <DropdownMenuGroup>
                    <DropdownMenuItem className="cursor-pointer" onClick={handleRegister}>
                        Register
                    </DropdownMenuItem>
                </DropdownMenuGroup>
            </DropdownMenuContent>}
        </DropdownMenu>
    )
}
