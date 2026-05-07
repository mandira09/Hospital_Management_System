import React, { useState } from "react";
import API from "../services/api";
import Navbar from "./Navbar";

function AddDoctor() {
  const [data, setData] = useState({
    name: "",
    specialization: "",
    availability: ""
  });

  const addDoctor = async () => {
    try {
      await API.post("/doctors", data);
      alert("Doctor added");
    } catch {
      alert("Error");
    }
  };

  return (
    <div>
      <Navbar />
      <div className="container mt-4">
        <div className="card p-4 shadow">
          <h3>Add Doctor</h3>

          <input className="form-control mb-2" placeholder="Name"
            onChange={e => setData({...data, name: e.target.value})} />

          <input className="form-control mb-2" placeholder="Specialization"
            onChange={e => setData({...data, specialization: e.target.value})} />

          <input className="form-control mb-3" placeholder="Availability"
            onChange={e => setData({...data, availability: e.target.value})} />

          <button className="btn btn-success" onClick={addDoctor}>
            Add Doctor
          </button>
        </div>
      </div>
    </div>
  );
}

export default AddDoctor;