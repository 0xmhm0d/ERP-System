import { Link, useParams } from "react-router-dom";
import { useContext, useEffect, useState } from 'react';
import EmployeeContext from "../../context/EmployeeContext";
import "./employeeview.scss";
import Navbar from './../../Components/navbar/Navbar';
import Sidebar from './../../Components/sidebar/Sidebar';

const EmployeeDetailItem = ({ label, value }) => (
  <div className="employee-detail-item">
    <span className="detail-label">{label}:</span>
    <span className="detail-value">{value}</span>
  </div>
);

const formatDate = (dateString) => {
  if (!dateString) return "N/A";
  try {
    // API returns dates in format "YYYY-MM-DDThh:mm:ss"
    return new Date(dateString).toLocaleDateString();
  } catch (error) {
    return dateString;
  }
};

const formatCurrency = (value) => {
  return typeof value === 'number' 
    ? `$${value.toLocaleString()}` 
    : value;
};

export default function EmployeeView({ logOut }) {
  const { employeeid } = useParams();
  const { getEmployeeById } = useContext(EmployeeContext);
  const [empdata, setEmpdata] = useState({
    employeeId: 0,
    hrName: "",
    employeeFullName: "",
    taxWithholding: 0,
    hoursWorked: 0,
    dateOfJoining: "",
    attendenceTime: "",
    holidays: "",
    employeeSalary: 0,
    hrid: 0
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEmployeeData = async () => {
      try {
        setLoading(true);
        setError(null);
        // Response is directly the employee object, not nested under 'data'
        const response = await getEmployeeById(employeeid);
        setEmpdata(response);
      } catch (err) {
        setError("Failed to fetch employee data. Please try again.");
        console.error("Error fetching employee:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchEmployeeData();
    // Use a stable reference to the getEmployeeById function
  }, [employeeid]);

  const renderEmployeeDetails = () => {
    const details = [
      { label: "Employee Name", value: empdata.employeeFullName },
      { label: "Tax Withholding", value: `${empdata.taxWithholding}%` },
      { label: "Hours Worked", value: empdata.hoursWorked },
      { label: "Date of Joining", value: formatDate(empdata.dateOfJoining) },
      { label: "Attendance Time", value: formatDate(empdata.attendenceTime) },
      { label: "Holidays", value: formatDate(empdata.holidays) },
      { label: "Salary", value: formatCurrency(empdata.employeeSalary) },
      { label: "Employee ID", value: empdata.employeeId },
      { label: "HR Name", value: empdata.hrName },
      { label: "HR Manager ID", value: empdata.hrid }
    ];

    return (
      <div className="employee-details-container">
        {details.map((detail, index) => (
          <EmployeeDetailItem 
            key={index}
            label={detail.label}
            value={detail.value}
          />
        ))}
      </div>
    );
  };

  return (
    <div className="list">
      <Sidebar />
      <div className="listContainer">
        <Navbar logOut={logOut} />
        <div className="product">
          <div className="productTitleContainer">
            <h1 className="productTitle">Employee Details</h1>
            <Link to="/employee/new">
              <button className="productAddButton">Create New</button>
            </Link>
          </div>
          <div className="productTop">
            <div className="productTopRight">
              {loading && <p className="loading-message">Loading employee data...</p>}
              {error && <p className="error-message">{error}</p>}
              {!loading && !error && renderEmployeeDetails()}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
