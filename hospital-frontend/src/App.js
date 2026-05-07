import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "./components/Login";
import Doctors from "./components/Doctors";
import AddDoctor from "./components/AddDoctor";


function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/doctors" element={<Doctors />} />
        <Route path="/add-doctor" element={<AddDoctor />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;