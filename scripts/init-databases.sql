-- Initialize all Tasky databases
CREATE DATABASE "Tasky_SaaS";
CREATE DATABASE "Tasky_Administration";
CREATE DATABASE "Tasky_Identity";
CREATE DATABASE "Tasky_Projects";

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE "Tasky_SaaS" TO postgres;
GRANT ALL PRIVILEGES ON DATABASE "Tasky_Administration" TO postgres;
GRANT ALL PRIVILEGES ON DATABASE "Tasky_Identity" TO postgres;
GRANT ALL PRIVILEGES ON DATABASE "Tasky_Projects" TO postgres;
