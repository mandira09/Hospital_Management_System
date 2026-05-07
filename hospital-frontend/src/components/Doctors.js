import React, { useEffect, useState } from "react";
import API from "../services/api";
import Navbar from "./Navbar";

function Doctors() {
  const [doctors, setDoctors] = useState([]);

  useEffect(() => {
    API.get("/doctors")
      .then((res) => setDoctors(res.data))
      .catch((err) => {
        console.log(err);
        alert("Unauthorized - token missing or invalid");
      });
  }, []);

  return (
    <div>
      <Navbar />

      <div className="container mt-4">
        <h2>Doctors</h2>

        <div className="row">
          {doctors.map((d) => (
            <div className="col-md-4" key={d.id}>
              <div className="card p-3 shadow mb-3">
                <h4>{d.name}</h4>
                <p>{d.specialization}</p>
                <p><b>{d.availability}</b></p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default Doctors;