import {
  FaFacebookSquare,
  FaInstagramSquare,
  FaGithubSquare,
  FaLinkedin,
} from "react-icons/fa";

interface SocialProps {
  containerStyles?: string;
  iconStyles?: string;
}

const socials = [
  {
    Icon: <FaFacebookSquare />,
    path: "https://facebook.com/bima.c.cetia.1",
  },
  {
    Icon: <FaInstagramSquare />,
    path: "https://www.instagram.com/bimsanss",
  },
  {
    Icon: <FaGithubSquare />,
    path: "https://github.com/Bimasanjaya0704",
  },
  {
    Icon: <FaLinkedin />,
    path: "https://www.linkedin.com/in/bimasanjaya/",
  },
];

const SocialMedia: React.FC<SocialProps> = ({
  containerStyles,
  iconStyles,
}) => {
  const handleSocialLinkClick = (url: string) => {
    window.open(url, "_blank", "noopener noreferrer");
  };

  return (
    <div className={containerStyles}>
      {socials.map((item, index) => {
        return (
          <button
            onClick={() => handleSocialLinkClick(item.path)}
            key={index}
            className={iconStyles}
          >
            {item.Icon}
          </button>
        );
      })}
    </div>
  );
};

export default SocialMedia;
