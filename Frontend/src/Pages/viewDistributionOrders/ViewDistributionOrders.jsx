import { Link, useNavigate, useParams } from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import DistributionOrdersContext from "../../context/DistributionOrdersContext";
import "./ViewDistributionOrders.scss";
import Navbar from "../../Components/navbar/Navbar";
import Sidebar from "../../Components/sidebar/Sidebar";
import axios from "axios";
import Swal from "sweetalert2";

// Constants
const API_BASE_URL = 'https://localhost:5001/api';

// Initial state
const initialOrderState = {
  id: 0,
  distributorId: 0,
  distributorName: "",
  totalQty: 0,
  subTotal: 0,
  totalPrice: 0,
  orderStatusId: 0,
  orderStatus: "",
  orderingDate: "",
  expectedArrivalDate: "",
  distributionOrderDetails: [],
};

// Components
const OrderDetails = ({ data }) => (
  <div className="manufacturInfoTop">
    {Object.entries({
      "Distributor Name": data.distributorName,
      "ID": data.id,
      "Distributor Id": data.distributorId,
      "Total Qty": data.totalQty,
      "Sub Total Price": data.subTotal,
      "Total Price": data.totalPrice,
      "Order Status Id": data.orderStatusId,
      "Order Status": data.orderStatus,
      "Ordering Date": data.orderingDate,
      "Expected Arrival Date": data.expectedArrivalDate,
    }).map(([label, value]) => (
      <p key={label} className={label === "Distributor Name" ? "categoryName" : "paddorder"}>
        <span className="spanform">{label}: </span>
        {value}
      </p>
    ))}
  </div>
);

const OrderStatusButtons = ({ orderStatus, onShipped, onFulfilled, isLoading }) => (
  <>
    <h3 className="status-title">Order Status:</h3>
    <div className="status-buttons">
      <button
        className="manufacturEdit1"
        onClick={onShipped}
      >
        {isLoading ? "Processing..." : "Shipped"}
      </button>
      <button
        className="manufacturEdit2"
        onClick={onFulfilled}
       
      >
        {isLoading ? "Processing..." : "Fulfilled"}
      </button>
    </div>
  </>
);

const OrderedMaterialsTable = ({ materials }) => (
  <div className="materials-table-container">
    <h3 className="materials-title">Ordered Materials</h3>
    <div className="table-wrapper">
      <table className="table-striped">
        <thead className="header">
          <tr>
            <th>Product Id</th>
            <th>Product Name</th>
            <th>Qty</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {materials.map((item) => (
            <tr key={item.productId}>
              <td>{item.productId}</td>
              <td>{item.productName}</td>
              <td>{item.qty}</td>
              <td>{item.price}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  </div>
);

export default function ViewDistributionOrders() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { GetDistributionOrdersById } = useContext(DistributionOrdersContext);
  
  const [orderData, setOrderData] = useState(initialOrderState);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  // Fetch order data
  useEffect(() => {
    const fetchOrderData = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const response = await GetDistributionOrdersById(id);
        setOrderData(response.data);
      } catch (err) {
        setError('Failed to fetch order details');
        Swal.fire({
          icon: 'error',
          title: 'Error',
          text: 'Failed to load order details. Please try again.',
        });
      } finally {
        setIsLoading(false);
      }
    };

    fetchOrderData();
  }, [id, GetDistributionOrdersById]);

  // Status update handlers
  const updateOrderStatus = async (newStatus) => {
    try {
      setIsLoading(true);
      setError(null);
      console.log(newStatus);
      
      await axios.put(
        `${API_BASE_URL}/ChangeDistributionStatusTo${newStatus}?orderId=${id}`
      );

      setOrderData(prev => ({
        ...prev,
        orderStatus: newStatus.toLowerCase()
      }));

      Swal.fire({
        icon: "success",
        title: "Changed!",
        text: `Order status has been changed to ${newStatus}.`,
        showConfirmButton: false,
        timer: 1500,
      });

      navigate("/distributororders");
    } catch (err) {
      setError(`Failed to update status to ${newStatus}`);
      Swal.fire({
        icon: "error",
        title: "Error",
        text: `Failed to change status: ${err.response?.data || err.message}`,
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleShipped = () => updateOrderStatus('Shipped');
  const handleFulfilled = () => updateOrderStatus('Fulfilled');

  if (error) {
    return (
      <div className="list">
        <Sidebar />
        <div className="listContainer">
          <Navbar />
          <div className="error-container">
            <h2>Error</h2>
            <p>{error}</p>
            <button onClick={() => window.location.reload()}>Retry</button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="list">
      <Sidebar />
      <div className="listContainer">
        <Navbar />
        <div className="manufactur">
          <div className="manufacturTitleContainer">
            <h1 className="manufacturTitle">Distributor Order/ {id}</h1>
            <Link to="/distributororders/neworder">
              <button className="manufacturAddButton">Order</button>
            </Link>
          </div>
          
          <div className="manufacturTop">
            <div className="manufacturTopRight">
              {isLoading ? (
                <div className="loading">Loading...</div>
              ) : (
                <>
                  <OrderDetails data={orderData} />
                  <OrderStatusButtons 
                    orderStatus={orderData.orderStatus}
                    onShipped={handleShipped}
                    onFulfilled={handleFulfilled}
                    isLoading={isLoading}
                  />
                  <hr />
                  <OrderedMaterialsTable 
                    materials={orderData.distributionOrderDetails} 
                  />
                </>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
