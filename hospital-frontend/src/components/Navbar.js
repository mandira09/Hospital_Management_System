import { Link } from "react-router-dom";

function Navbar() {
  const role = localStorage.getItem("role");

  const logout = () => {
    localStorage.clear();
    window.location.href = "/";
  };

  return (
    <nav className="navbar navbar-dark bg-dark p-3">
      <span className="navbar-brand">Hospital System</span>

      <div>
        {/* COMMON */}
        <Link to="/doctors" className="btn btn-light me-2">Doctors</Link>

        {/* ADMIN */}
        {role === "Admin" && (
          <>
            <Link to="/add-doctor" className="btn btn-light me-2">Add Doctor</Link>
            <Link to="/add-patient" className="btn btn-light me-2">Add Patient</Link>
            <Link to="/billing" className="btn btn-light me-2">Billing</Link>
            <Link to="/appointments" className="btn btn-light me-2">Appointments</Link>
          </>
        )}

        {/* PATIENT */}
        {role === "Patient" && (
          <>
            <Link to="/appointment" className="btn btn-light me-2">Book</Link>
            <Link to="/appointments" className="btn btn-light me-2">My Appointments</Link>
          </>
        )}

        {/* DOCTOR */}
        {role === "Doctor" && (
          <>
            <Link to="/appointments" className="btn btn-light me-2">Appointments</Link>
          </>
        )}

        <button className="btn btn-danger" onClick={logout}>Logout</button>
      </div>
    </nav>
  );
}

export default Navbar;