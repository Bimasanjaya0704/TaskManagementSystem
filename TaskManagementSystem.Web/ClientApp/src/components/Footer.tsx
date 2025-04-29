import { FaFileWaveform, FaInstagram, FaPhone } from "react-icons/fa6";
import { MdEmail } from "react-icons/md";
import { Button } from "./ui/button";

const Footer = () => {
  return (
    <footer className="mt-6 rounded-t-2xl bg-darkAccent-hover text-white p-6 px-10 md:p-10">
      <div className="w-full mx-auto float-none md:flex items-center md:px-10">
        {/* Center Text */}
        <div className="text-center md:text-left mt-4 md:mt-0 md:px-12 w-full 2xl:w-[800px]">
          <h2 className="text-[20px] md:text-[30px] 2xl:text-[50px] font-bold leading-tight">
            Task <br /> Management System
          </h2>
        </div>

        <div className="border-l border-white mr-6 h-[110px] 2xl:h-[200px] 2xl:border-l-4 hidden md:block"></div>

        {/* Contact Section */}
        <div className="mt-6 md:mt-0 md:px-12 2xl:w-[800px] text-center md:text-left">
          <h3 className="text-xl font-bold">Contact</h3>
          <div className="mt-2 text-[10px] 2xl:text-[16px] flex-none md:flex items-center gap-12">
            <div className="space-y-2 text-secondaryLight font-light">
              <div className="flex items-center space-x-2 justify-center md:justify-start">
                <MdEmail className="text-gray-400" />
                <div>bimasanjayawork@gmail.com</div>
              </div>
              <div className="flex items-center space-x-2 justify-center md:justify-start">
                <FaPhone className="text-gray-400" />
                <div>089340868777</div>
              </div>
            </div>
            <div className="space-y-2 text-secondaryLight font-light">
              <div className="flex items-center space-x-2 justify-center md:justify-start">
                <FaInstagram />
                <div>@bimsanss</div>
              </div>
              <div className="flex items-center space-x-2 justify-center md:justify-start">
                <FaFileWaveform /> <div>https://bimasanjaya.vercel.app/</div>
              </div>
            </div>
          </div>
          <Button
            variant="default"
            className="bg-accents/50 hover:scale-105 backrdrop-blur-md text-[12px] 2xl:text-[16px] border border-white mt-4 w-full"
          >
            Manage Task, <span className="font-bold">It's Free!</span>
          </Button>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
