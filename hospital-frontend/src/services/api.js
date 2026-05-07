import axios from "axios";

const API = axios.create({
  baseURL: "https://localhost:7042/api"
});

API.interceptors.request.use((req) => {
  const token = localStorage.getItem("token");

  console.log("TOKEN SENT:", token); // 👈 add this

  if (token) {
    req.headers.Authorization = `Bearer ${token}`;
  }

  return req;
});

export default API;