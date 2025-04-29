import React, { ReactNode } from "react";
import { Navbar } from "./components/Navbar";
import Footer from "./components/Footer";
import { Toaster } from "sonner";

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div>
      <Navbar />
      <main>{children}</main>
      <Toaster />
      <Footer />
    </div>
  );
};

export default Layout;
