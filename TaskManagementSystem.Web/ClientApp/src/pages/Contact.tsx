import React from "react";
import { FaWhatsapp } from "react-icons/fa";
import { Button } from "../components/ui/button";
import Lottie from "lottie-react";
import contactAnimation from "../assets/contact.json";

const Contact: React.FC = () => {
  const phoneNumber = "6281234567890";
  const message = encodeURIComponent(
    "Halo, saya tertarik untuk berdiskusi lebih lanjut mengenai project Anda."
  );
  const waLink = `https://wa.me/${phoneNumber}?text=${message}`;

  return (
    <div className="flex items-center justify-center bg-gradient-to-r from-accents to-accent-hover px-4 md:px-10 py-10 text-white rounded-xl">
      <div className="grid md:grid-cols-2 gap-10 items-center max-w-6xl w-full bg-white/10 backdrop-blur-md p-8 md:p-12 rounded-2xl shadow-2xl border border-white/20 transition-all duration-300 transform hover:scale-[1.01]">
        
        <div className="w-full flex justify-center">
          <Lottie animationData={contactAnimation} loop={true} className="max-w-[350px] w-full" />
        </div>

        <div className="text-center md:text-left">
          <h2 className="text-4xl font-bold text-white mb-4">Get in Touch</h2>
          <p className="text-base text-gray-200 mb-6 leading-relaxed">
            Have a question, a project idea, or just want to say hello? 
            Let’s connect and make something great together!
            <br />
            Click the button below to reach me via WhatsApp — no need to type anything.
          </p>

          <div className="flex justify-center md:justify-start">
            <a href={waLink} target="_blank" rel="noopener noreferrer">
              <Button className="flex items-center gap-2 px-6 py-3 hover:scale-105 bg-green-500 border border-white text-white font-semibold rounded-lg shadow-md hover:bg-green-600 transition">
                <FaWhatsapp size={20} />
                Chat via WhatsApp
              </Button>
            </a>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Contact;
