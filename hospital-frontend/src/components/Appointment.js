import React, { useEffect, useState } from "react";
import API from "../services/api";
import Navbar from "./Navbar";

function Appointments() {
  const [data, setData] = useState([]);

  useEffect(() => {
    API.get("/appointments")
      .then(res => setData(res.data))
      .catch(() => alert("Error"));
  }, []);

  return (
    <div>
      <Navbar />
      <div className="container mt-4">
        <h3>Appointments</h3>

        {data.map(a => (
          <div key={a.appointmentId} className="card p-3 mb-2">
            Patient: {a.patientId} | Doctor: {a.doctorId}
            <br />
            {a.dateTime} | {a.status}
          </div>
        ))}
      </div>
    </div>
  );
}

export default Appointments;