import React, { useState } from "react";
import API from "../services/api";

function Login() {
  const [data, setData] = useState({
    username: "",
    password: ""
  });

  const handleLogin = async () => {
    try {
      const res = await API.post("/auth/login", data);

console.log("LOGIN RESPONSE:", res.data);

// 🔥 ALWAYS FORCE STRING TOKEN
const token = typeof res.data === "string"
  ? res.data
  : res.data.token;

localStorage.setItem("token", token);

alert("Login success");
window.location.href = "/doctors";
    } catch (err) {
      console.log(err);
      alert("Invalid credentials");
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="card p-4 shadow" style={{ width: "350px" }}>
        <h3 className="text-center mb-3">Login</h3>

        <input
          className="form-control mb-2"
          placeholder="Username"
          onChange={(e) =>
            setData({ ...data, username: e.target.value })
          }
        />

        <input
          type="password"
          className="form-control mb-3"
          placeholder="Password"
          onChange={(e) =>
            setData({ ...data, password: e.target.value })
          }
        />

        <button className="btn btn-primary w-100" onClick={handleLogin}>
          Login
        </button>
      </div>
    </div>
  );
}

export default Login;