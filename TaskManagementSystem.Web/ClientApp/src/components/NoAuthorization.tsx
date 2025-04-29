import { Link } from "react-router-dom";
import { FaKey } from "react-icons/fa6";
import { Button } from "./ui/button";

export default function UnauthorizedPage() {
  return (
    <>
      <div className="min-h-screen flex flex-col rounded-xl justify-center items-center text-center bg-gradient-to-br from-accents to-accent-hover p-6">
        <div className="max-w-lg bg-white/10 backdrop-blur-xl shadow-lg rounded-lg p-8">
          <h1 className="text-2xl md:text-4xl font-black text-white uppercase mb-4">
            You seem to be lost!
          </h1>
          <p className="text-white text-sm md:text-base">
            The page you're looking for isn't available. Please log in to access
            the content.
          </p>
          <Link to="/login">
            <Button
              className="text-white mt-6 px-6 py-3 text-lg flex items-center justify-center gap-4 w-full"
            >
              <FaKey /> Login
            </Button>
          </Link>
        </div>
      </div>
    </>
  );
}
