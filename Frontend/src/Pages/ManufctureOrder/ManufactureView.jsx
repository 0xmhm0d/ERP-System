import { Link, useNavigate, useParams } from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import ManufactoringContext from "../../context/ManufactoringContext";
import "./manufacureview.scss";
import Navbar from "./../../Components/navbar/Navbar";
import Sidebar from "./../../Components/sidebar/Sidebar";
import axios from "axios";
import Swal from 'sweetalert2';

const INITIAL_MANUFACTURE_STATE = {
  productManufacturedId: 0,
  qtyToManufacture: 0,
  manufacturingCost: 0,
  startingDate: "2023-03-27T20:46:30.541Z",
  id: 0,
  productManufacturedName: "",
  finishingDate: "2023-03-27T20:46:30.541Z",
  manufacturingStatus: "",
  manufacturingOrderDetails: [
    {
      rawMaterialId: 0,
      rawMaterialName: "",
      rawMaterialQtyUsed: 0,
    },
  ],
};

const API_BASE_URL = 'https://localhost:5001/api';

export default function ManufactureView({logOut}) {
  const { manufacturId } = useParams();
  const navigate = useNavigate();
  const { GetManfacturingOrderById } = useContext(ManufactoringContext);
  
  const [manufacturdata, setManufacturedata] = useState(INITIAL_MANUFACTURE_STATE);

  const updateManufactureStatus = async (newStatus, endpoint) => {
    try {
      await axios.put(`${API_BASE_URL}/${endpoint}`, null, {
        params: { orderId: manufacturId }
      });
      
      setManufacturedata(prev => ({
        ...prev,
        manufacturingStatus: newStatus
      }));

      Swal.fire({
        icon: 'success',
        title: 'Changed!',
        text: `Manufacturing status has been changed to ${newStatus}.`,
        showConfirmButton: false,
        timer: 1500
      });

      navigate("/manufactur");
    } catch (error) {
      console.error('Error updating status:', error);
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: `Failed to update status: ${error.response?.data || error.message}`,
      });
    }
  };

  const handleShipped = () => updateManufactureStatus(
    'ShippedToInventory', 
    'ChangeManufacturingStatusToShippedToInventory'
  );

  const handleManufacture = () => updateManufactureStatus(
    'Manufacturing', 
    'ChangeManufacturingStatusToManufacturing'
  );

  useEffect(() => {
    const fetchManufactureData = async () => {
      try {
        const response = await GetManfacturingOrderById(manufacturId);
        setManufacturedata(response.data);
      } catch (error) {
        Swal.fire({
          icon: 'error',
          title: 'Error',
          text: `Failed to fetch manufacture data: ${error.message}`,
        });
      }
    };

    fetchManufactureData();
  }, [manufacturId, GetManfacturingOrderById]);

  const isStatusPending = manufacturdata.manufacturingStatus === "Pending";
  const isStatusShipped = manufacturdata.manufacturingStatus === "ShippedToInventory";

  return (
    <div className="list">
      <Sidebar />
      <div className="listContainer">
      <Navbar logOut={logOut}/>
      <div className="manufactur">
          <div className="manufacturTitleContainer">
            <h1 className="manufacturTitle">Manufacturing</h1>
            <Link to="/manufactur/newmanufactur">
              <button className="manufacturAddButton">Create</button>
            </Link>
          </div>
          <div className="manufacturTop">
            <div className="manufacturTopRight">
              <div className="manufacturInfoTop">
                <p className="categoryName">
                  <span className="spanform">productManufacturedId: </span>
                  {manufacturdata.productManufacturedId}
                </p>
                <p className="paddorder">
                  <span className="spanform">productManufacturedName: </span>
                  {manufacturdata.productManufacturedName}
                </p>

                <p className="paddorder">
                  <span className="spanform">qtyToManufacture: </span>
                  {manufacturdata.qtyToManufacture}
                </p>
                <p className="paddorder">
                  <span className="spanform">manufacturingCost: </span>
                  {manufacturdata.manufacturingCost}
                </p>
                <p className="paddorder">
                  <span className="spanform">manufacturId: </span>
                  {manufacturdata.id}
                </p>
                <p className="paddorder">
                  <span className="spanform">startingDate: </span>
                  {manufacturdata.startingDate}
                </p>
                <p className="paddorder">
                  <span className="spanform">finishingDate: </span>
                  {manufacturdata.finishingDate}
                </p>
                <p className="paddorder">
                  <span className="spanform">manufacturingStatus: </span>
                  {manufacturdata.manufacturingStatus}
                </p>
              </div>
              <h3>Manufacturing Status:</h3>
              <div
                style={{ display: "flex", gap: "50px", marginBottom: "20px" }}
              >
                <button 
                  className="manufacturEdit1" 
                  onClick={handleManufacture}
                  style={{color: isStatusPending ? "blue" : "lightgray"}}
                  disabled={!isStatusPending}
                >
                  Manufacturing
                </button>
                <button 
                  className="manufacturEdit2" 
                  onClick={handleShipped}
                  style={{color: !isStatusShipped ? "blue" : "lightgray"}}
                  disabled={isStatusShipped}
                >
                  ShippedToInventory
                </button>
              </div>
              <hr />
              <h3>Raw Matrial Used: </h3>
              <table className="table-striped">
                <thead className="header">
                  <tr>
                    <th>Material-ID</th>
                    <th>Material-Name</th>
                    <th>QTY</th>
                  </tr>
                </thead>
                <tbody>
                  {manufacturdata.manufacturingOrderDetails.map((item) => (
                    <tr key={item.id}>
                      <td>{item.rawMaterialId}</td>
                      <td>{item.rawMaterialName}</td>
                      <td>{item.rawMaterialQtyUsed}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
