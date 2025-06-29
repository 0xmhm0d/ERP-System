import "./newemptask.scss";
import Sidebar from './../../Components/sidebar/Sidebar';
import Navbar from './../../Components/navbar/Navbar';
import { useEffect, useContext, useState } from "react";
import axios from 'axios';
import { useNavigate } from "react-router-dom";
import Swal from 'sweetalert2';
import EmployeeContext from "../../context/EmployeeContext";

function NewEmployeetask({ inputs, title, logOut }) {
  const navigate = useNavigate();
  const { getAllemployee, dataemp } = useContext(EmployeeContext);

  const [emptask, setEmptask] = useState({
    taskDescription: "",
    taskAssignedTime: new Date().toISOString(),
    taskDeadlineTime: new Date().toISOString(),
    emplyeeId: 0
  });

  useEffect(() => {
    getAllemployee();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setEmptask((prev) => ({
      ...prev,
      [name]: name === "emplyeeId" ? parseInt(value) : value,
    }));
  };

  const validateForm = () => {
    if (!emptask.emplyeeId || emptask.emplyeeId === 0) {
      Swal.fire({
        icon: "error",
        title: "Error!",
        text: "Please select a valid employee",
        showConfirmButton: true,
      });
      return false;
    }
    if (!emptask.taskDescription.trim()) {
      Swal.fire({
        icon: "error",
        title: "Error!",
        text: "Please enter a task description",
        showConfirmButton: true,
      });
      return false;
    }
    return true;
  };

  async function sendData() {
    if (!validateForm()) return;

    const emptaskData = {
      taskDescription: emptask.taskDescription.trim(),
      taskAssignedTime: new Date().toISOString(),
      taskDeadlineTime: new Date(emptask.taskDeadlineTime).toISOString(),
      emplyeeId: parseInt(emptask.emplyeeId)
    };

    try {
      console.log("emptaskData", emptaskData);
      const res = await axios.post(
        `https://localhost:5001/api/AddNewTaskForEmployee`,
        emptaskData,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      console.log("res", res);
      navigate("/employeetask");
      Swal.fire({
        icon: "success",
        title: "Added!",
        text: `${emptask.taskDescription} has been Added.`,
        showConfirmButton: false,
        timer: 2000,
      });
    } catch (error) {
      console.error("Error adding task:", error);
      Swal.fire({
        icon: "error",
        title: "Error!",
        text: "Failed to add task. Please check the data and try again.",
        showConfirmButton: true,
      });
    }
  }

  const handleSubmit = (e) => {
    e.preventDefault();
    sendData();
  };

  return (
    <div className="newrawmatrial">
      <Sidebar />
      <div className="newContainer">
        <Navbar logOut={logOut} />
        <div className="top">
          <h1>{title}</h1>
        </div>
        <div className="bottom">
          <div className="right">
            <form onSubmit={handleSubmit}>
              <div className="formInput">
                <label htmlFor="emplyeeId">Choose Employee:</label>
                <select
                  name="emplyeeId"
                  id="emplyeeId"
                  onChange={handleInputChange}
                  value={emptask.emplyeeId}
                  required
                >
                  <option value="">Select an employee</option>
                  {dataemp?.map((employee) => (
                    <option value={employee.employeeId} key={employee.employeeId}>
                      {employee.employeeId} - {employee.employeeFullName}
                    </option>
                  ))}
                </select>
              </div>

              {inputs.map((input) => (
                <div className="formInput" key={input.id}>
                  <label>{input.label}</label>
                  <input
                    type={input.type}
                    name={input.name}
                    onChange={handleInputChange}
                    placeholder={input.placeholder}
                    value={emptask[input.name]}
                    required
                  />
                </div>
              ))}
              <button type="submit">Send</button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default NewEmployeetask;