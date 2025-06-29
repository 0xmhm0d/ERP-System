import "./newemployeetrain.scss";
import Sidebar from './../../Components/sidebar/Sidebar';
import Navbar from './../../Components/navbar/Navbar';
import { useEffect, useContext, useState } from "react";
import axios from 'axios';
import { useNavigate } from "react-router-dom";
import Swal from 'sweetalert2';
import EmployeeContext from "../../context/EmployeeContext";
import HrMangerContext from "../../context/HrMangerContext";

function NewEmployeeTrain({ inputs, title, logOut }) {
  const [file, setFile] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const navigate = useNavigate();
  const { getAllemployee, dataemp } = useContext(EmployeeContext);
  const { getAllhrmanager, data } = useContext(HrMangerContext);

  const [emptrain, setEmptrain] = useState({
    trainningType: "",
    trainningDescription: "",
    employeeId: 0,
    hrid: 0
  });

  const validateForm = () => {
    const newErrors = {};
    if (!emptrain.trainningType.trim()) {
      newErrors.trainningType = "Training type is required";
    }
    if (!emptrain.trainningDescription.trim()) {
      newErrors.trainningDescription = "Training description is required";
    }
    if (emptrain.employeeId === 0) {
      newErrors.employeeId = "Please select an employee";
    }
    if (emptrain.hrid === 0) {
      newErrors.hrid = "Please select an HR manager";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const productInvOptions = dataemp?.map((product) => (
    <option value={product.employeeId} key={product.employeeId}>
      {product.employeeId} - {product.employeeFullName}
    </option>
  ));

  const hrOptions = data?.map((product) => (
    <option value={product.hrid} key={product.hrid}>
      {product.hrid} - {product.hrfullName}
    </option>
  ));

  useEffect(() => {
    getAllemployee();
    getAllhrmanager();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setEmptrain((prev) => ({
      ...prev,
      [name]: value,
    }));
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };

  async function sendData() {
    if (!validateForm()) {
      return;
    }

    setIsLoading(true);
    const emptrainData = { ...emptrain };
    const config = {
      headers: {
        "Content-Type": "application/json",
      },
    };

    try {
      const res = await axios.post(
        `https://localhost:5001/api/AddEmployeeTraining`,
        emptrainData,
        config
      );
      
      Swal.fire({
        icon: "success",
        title: "Success!",
        text: "Training record has been created successfully.",
        showConfirmButton: false,
        timer: 2000,
      }).then(() => {
        navigate("/employeetrain");
      });
    } catch (error) {
      console.error("Error adding training:", error);
      Swal.fire({
        icon: "error",
        title: "Error!",
        text: error.response?.data?.message || "Failed to create training record. Please try again.",
        showConfirmButton: true,
      });
    } finally {
      setIsLoading(false);
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
          <div className="left">
            <img
              src={
                file
                  ? URL.createObjectURL(file)
                  : "https://icon-library.com/images/no-image-icon/no-image-icon-0.jpg"
              }
              alt=""
            />
          </div>
          <div className="right">
            <form onSubmit={handleSubmit}>
              <div className="form-group" style={{ marginBottom: "20px" }}>
                <label htmlFor="hrid">Choose HR-Manager:</label>
                <select
                  name="hrid"
                  id="hrid"
                  onChange={handleInputChange}
                  className={errors.hrid ? "error-input" : ""}
                >
                  <option value={0}>Select HR Manager</option>
                  {hrOptions}
                </select>
                {errors.hrid && <span className="error-message">{errors.hrid}</span>}
              </div>

              <div className="form-group" style={{ marginBottom: "20px" }}>
                <label htmlFor="employeeId">Choose Employee:</label>
                <select
                  name="employeeId"
                  id="employeeId"
                  onChange={handleInputChange}
                  className={errors.employeeId ? "error-input" : ""}
                >
                  <option value={0}>Select Employee</option>
                  {productInvOptions}
                </select>
                {errors.employeeId && <span className="error-message">{errors.employeeId}</span>}
              </div>

              {inputs.map((input) => (
                <div className="form-group" key={input.id} style={{ marginBottom: "20px" }}>
                  <label>{input.label}</label>
                  <input
                    type={input.type}
                    name={input.name}
                    onChange={handleInputChange}
                    placeholder={input.placeholder}
                    className={errors[input.name] ? "error-input" : ""}
                  />
                  {errors[input.name] && <span className="error-message">{errors[input.name]}</span>}
                </div>
              ))}
              
              <button type="submit" disabled={isLoading}>
                {isLoading ? "Sending..." : "Send"}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default NewEmployeeTrain;