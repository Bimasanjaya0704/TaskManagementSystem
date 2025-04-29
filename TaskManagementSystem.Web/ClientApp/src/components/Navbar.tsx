import React, { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { FiMenu, FiX } from "react-icons/fi";
import logo from "../../src/assets/logo.png";
import contact from "../../src/assets/contact.svg";
import { ProfileBtn } from "./ProfileBtn";
import { Button } from "./ui/button";

interface LinkType {
  name: string;
  path: string;
}

const links: LinkType[] = [
  { name: "Home", path: "/" },
  { name: "Projects", path: "/projects" },
  { name: "About us", path: "/about" },
  { name: "Contact Us", path: "/contact" },
];

interface MobileNavbarProps {
  onClose: () => void;
}

export const Navbar: React.FC = () => {
  const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);
  const location = useLocation();

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  const isActiveContact = location.pathname === "/contact";
  return (
    <nav className="sticky md:px-6 top-0 z-10 flex justify-between items-center space-x-6 py-5 bg-transparent mb-4">
      {/* Logo */}
      <Link to={"/"}>
        <div className="bg-accents shadow-xl px-2 md:px-4 py-0.5 rounded-full flex items-center ">
          <div>
            <img src={logo} className="w-[56px] md:w-[76px]" alt="logo" />
          </div>
        </div>
      </Link>

      {/* Action Buttons */}
      <div className="flex shadow-md items-center pr-3 justify-between md:px-8 space-x-2 md:space-x-12 md:bg-secondaryLight md:w-full py-2 2xl:py-[9px] rounded-full">
        {/* Desktop Nav */}
        <ul className="hidden md:flex space-x-3 lg:space-x-8 text-gray-700 dark:text-gray-300 transition-all">
          {links.map((link, index) => {
            const isActive = location.pathname === link.path;
            return (
              <li key={index}>
                <Link
                  to={link.path}
                  className={`flex justify-center text-sm md:text-base 2xl:text-lg items-center hover:text-accents dark:hover:text-darkAccent-hover ${isActive
                    ? "text-darkAccent font-semibold border-b-2 border-b-accents scale-110"
                    : ""
                    }`}
                >
                  {link.name}
                </Link>
              </li>
            );
          })}
        </ul>
        <div className="flex items-center space-x-2">
          <Link to={"/contact"} className="hidden md:block ">
            <div
              className={`px-3 py-1 flex items-center  space-x-2 bg-accent-hover hover:bg-[#2b4e69] dark:hover:bg-darkAccent-hover dark:bg-darkAccent rounded-full ${isActiveContact ? "bg-darkAccent" : ""
                }`}
            >
              <img
                src={contact}
                width={25}
                alt="contact icon"
                className="p-1 rounded-full bg-light dark:bg-dark"
              />
              <span className="text-light dark:text-dark text-xs 2xl:text-base font-light">
                Contact
              </span>
            </div>
          </Link>
          <div className="flex space-x-2">
            {/* Profile Toggle */}
            <ProfileBtn />

            {/* Mobile Menu Button */}
            <Button
              onClick={toggleMenu}
              className="md:hidden text-xs"
            >
              {isMenuOpen ? <FiX /> : <FiMenu />}
            </Button>
          </div>
        </div>
      </div>

      {/* Mobile Nav */}
      {isMenuOpen && <MobileNavbar onClose={toggleMenu} />}
    </nav>
  );
};

const MobileNavbar: React.FC<MobileNavbarProps> = ({ onClose }) => {
  return (
    <div className="md:hidden absolute top-16 left-0 w-full bg-white/70 backdrop-blur-md shadow-accents shadow-md">
      <ul className="flex flex-col text-xs transition-all space-y-1 py-2">
        {links.map((link, index) => (
          <li key={index}>
            <Link
              to={link.path}
              onClick={onClose}
              className="block py-2 transition-all px-4 hover:bg-accents hover:text-light text-black/90"
            >
              {link.name}
            </Link>
          </li>
        ))}
        <li>
          <Link
            to={"/contact"}
            onClick={onClose}
            className="block py-2 transition-all px-4 hover:bg-accents hover:text-light text-black/90"
          >
            Contact
          </Link>
        </li>
      </ul>
    </div>
  );
};

export default Navbar;
