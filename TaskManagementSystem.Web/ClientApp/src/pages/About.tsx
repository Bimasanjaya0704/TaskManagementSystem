import authorImage from "../assets/photo.png";
import SocialMedia from "../components/SocialMedia";

const featurelist: { name: string }[] = [
  { name: "Real-time task updates and notifications" },
  {
    name: "AI-powered task suggestions for improved efficiency",
  },
  { name: "Collaboration tools with user roles and permissions" },
  { name: "Advanced analytics and reporting" },
];

const TechStack = [
  {
    Role: "Backend",
    Desc: "ASP.NET 8",
  },
  {
    Role: "FrontEnd",
    Desc: "React Vite, TypeScript, Tailwind CSS",
  },
  {
    Role: "Database",
    Desc: "PostgreSQL",
  },
  {
    Role: "Design Software",
    Desc: "Onion Architecture, Result Pattern Design",
  },
];

const About = () => {
  return (
    <div className="flex flex-col rounded-xl items-center justify-center min-h-screen p-8 bg-gradient-to-r from-indigo-600 to-accent-hover text-white">
      <div className="text-center w-full bg-white/10 backdrop-blur-md p-8 rounded-2xl shadow-lg">
        {/* Title Section */}
        <header className="w-full flex justify-center items-center mb-8">
          <h1 className="text-2xl md:text-5xl lg:text-6xl font-extrabold text-white">
            About Us
          </h1>
        </header>

        {/* About Section */}
        <section className="lg:flex lg:space-x-12">
          {/* Left Text (About the system) */}
          <div className="lg:w-1/2 mb-8 lg:mb-0">
            <h2 className="text-xl md:text-3xl 2xl:text-[50px] font-bold text-center mb-4">
              Task Management System?
            </h2>
            <div className="text-sm md:text-xl 2xl:text-[24px] text-justify leading-relaxed dark:text-primary">
              This Task Management System is designed to help teams efficiently
              manage their projects and tasks. Built with a structured
              architecture, it ensures scalability, maintainability, and
              performance optimization. Users can create and manage multiple
              projects, assign tasks, and track progress seamlessly.
            </div>
          </div>

          {/* Right Text (Tech Stack) */}
          <div className="lg:w-1/2">
            <div className="text-xl md:text-3xl 2xl:text-[50px] font-bold text-center mb-4">
              Techstack
            </div>
            <div className="space-y-4 dark:text-primary">
              {TechStack.map((tech, index) => (
                <div
                  key={index}
                  className="flex items-center w-full space-x-2 md:space-x-4 py-2 md:py-3 px-4 shadow-lg text-indigo-500 bg-white rounded-xl"
                >
                  <div className="w-[110px] 2xl:w-[150px] text-left font-semibold text-sm md:text-md 2xl:text-xl">
                    {tech.Role}
                  </div>
                  <div className="border-l-2 border-indigo-500 h-5"></div>

                  <div className="text-sm md:text-md 2xl:text-xl">
                    {tech.Desc}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* Author Section */}
        <section className="mt-12 2xl:mt-24">
          <h2 className="text-2xl md:text-5xl lg:text-6xl font-extrabold mb-6">
            Author
          </h2>
          <div className="flex flex-col md:flex-row items-center p-6 rounded-lg shadow-2xl shadow-accents">
            <div className="md:w-1/2 pr-4">
              <div className="text-sm md:text-xl 2xl:text-[24px] mb-4 text-justify">
                Haii, My name Bima Sanjaya. I am a software engineer with one
                year of experience. As a frontend engineer, I specialize in
                building responsive and interactive user interfaces, as well as
                integrating with backend services. My backend experience
                includes developing APIs, develop robotic system, and applying
                Object-Oriented Programming (OOP) principles usingcC# and
                Python.
              </div>
              <div className="text-lg font-semibold italic mb-8">
                "Building the future of task management, one step at a time."
              </div>
              <div className="flex justify-center">
                <SocialMedia
                  containerStyles="flex gap-4"
                  iconStyles="w-12 h-12 border border-accent 
                  rounded-full flex justify-center items-center 
                  text-accent text-base hover:bg-white 
                  hover:text-indigo-500 cursor-pointer hover:scale-125 hover:transition-all duration-500 text-[24px] 2xl:text-3xl 2xl:h-14 2xl:w-14"
                />
              </div>
            </div>
            <div className="md:w-1/2 ml-8 flex justify-center bg-accents/10 backdrop-blur-md rounded-full">
              <img
                src={authorImage}
                alt="Author Bima Sanjaya"
                className="w-[230px] h-[230px] lg:h-[498px] lg:w-[498px] 2xl:w-[498px] 2xl:h-[498px] object-contain"
              />
            </div>
          </div>
        </section>

        {/* Future Vision Section */}
        <section className="mt-12">
          <h2 className="text-2xl md:text-5xl lg:text-6xl font-extrabold mb-6">
            Future Vision
          </h2>
          <div className="text-sm md:text-xl 2xl:text-[24px] px-24 mb-16">
            The goal for this project is to continuously evolve by adding new
            features, optimizing performance, and ensuring seamless user
            experience. Future improvements include:
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-10 px-8">
            {featurelist.map((feature, index) => (
              <div
                key={index}
                className="flex px-[14px] py-[4px] md:px-6 md:py-3 bg-white/5 backdrop-blur-md justify-center rounded-full shadow-md hover:scale-105 hover:transition-all duration-500 cursor-zoom-in"
              >
                <div className="text-[12px] text-center font-semibold md:text-base 2xl:text-[16px]">
                  {feature.name}
                </div>
              </div>
            ))}
          </div>
        </section>
      </div>
    </div>
  );
};

export default About;
