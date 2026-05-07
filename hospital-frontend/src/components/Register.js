import React, { useState } from "react";
import API from "../services/api";

function Register() {
  const [data, setData] = useState({
    username: "",
    password: "",
    role: ""
  });

  const register = async () => {
    try {
      await API.post("/auth/register", data);
      alert("Registered successfully");
      window.location.href = "/";
    } catch {
      alert("Error registering");
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="card p-4 shadow" style={{ width: "350px" }}>
        <h3 className="text-center">Register</h3>

        <input className="form-control mb-2" placeholder="Username"
          onChange={e => setData({...data, username: e.target.value})} />

        <input type="password" className="form-control mb-2" placeholder="Password"
          onChange={e => setData({...data, password: e.target.value})} />

        <select className="form-control mb-3"
          onChange={e => setData({...data, role: e.target.value})}>
          <option>Select Role</option>
          <option>Admin</option>
          <option>Patient</option>
          <option>Doctor</option>
        </select>

        <button className="btn btn-success w-100" onClick={register}>
          Register
        </button>
      </div>
    </div>
  );
}

export default Register;