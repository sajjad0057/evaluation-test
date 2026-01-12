import React from "react";
import HomePage from "./components/HomePage";

const App: React.FC = () => {
  return (
    <div>
      {/* Navbar */}
      <nav className="navbar navbar-expand-lg navbar-dark bg-primary">
        <div className="container">
          <a className="navbar-brand" href="#">
            User Manager
          </a>
        </div>
      </nav>

      {/* Main Content */}
      <main className="py-4 bg-light min-vh-100">
        <HomePage />
      </main>

      {/* Footer */}
      <footer className="bg-primary text-white text-center py-3 mt-auto">
        &copy; {new Date().getFullYear()} User Manager App
      </footer>
    </div>
  );
};

export default App;
