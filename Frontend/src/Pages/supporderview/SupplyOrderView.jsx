import { Link, useNavigate, useParams } from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import SupplierorderContext from "../../context/SupplierorderContext";
import "./supporderview.scss";
import Navbar from "./../../Components/navbar/Navbar";
import Sidebar from "./../../Components/sidebar/Sidebar";
import axios from "axios";
import Swal from 'sweetalert2';

export default function SupplyOrderView({logOut}) {
  const { supplierorderId } = useParams();
  const navigate = useNavigate();
  const { 
    GetSupplierOrderById, 
    changeSupplierOrderStatusToShipped, 
    changeSupplierOrderStatusToFulfilled 
  } = useContext(SupplierorderContext);
  const [isLoading, setIsLoading] = useState(false);
  
  const initialOrderState = {
    id: 0,
    supplierId: 0,
    supplierName: "",
    totalQty: 0,
    subTotalPrice: 0,
    shippingCost: 0,
    totalPrice: 0,
    orderStatusId: 0,
    orderStatus: "",
    orderingDate: "",
    expectedArrivalDate: "",
    orderedMaterials: []
  };

  const [supporderdata, setSupporderdata] = useState(initialOrderState);

  const updateOrderStatus = async (newStatus) => {
    try {
      setIsLoading(true);
      console.log("newStatus", newStatus);
      
      // Use the context functions to update the status
      if (newStatus === 'Shipped') {
        await changeSupplierOrderStatusToShipped(supplierorderId);
      } else if (newStatus === 'Fulfilled') {
        await changeSupplierOrderStatusToFulfilled(supplierorderId);
      }

      setSupporderdata(prev => ({
        ...prev,
        orderStatus: newStatus
      }));

      Swal.fire({
        icon: 'success',
        title: 'Changed!',
        text: `Order status has been changed to ${newStatus}.`,
        showConfirmButton: false,
        timer: 1500
      });

      navigate("/supplierorders");
    } catch (error) {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: `Failed to update order status: ${error.message}`,
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleShipped = () => updateOrderStatus('Shipped');
  const handleFulfilled = () => updateOrderStatus('Fulfilled');

  async function getsupporder() {
    try {
      setIsLoading(true);
      const response = await GetSupplierOrderById(supplierorderId);
      setSupporderdata(response.data);
    } catch (error) {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: `Failed to fetch order details: ${error.message}`,
      });
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    getsupporder();
  }, [supplierorderId]);

  const StatusButton = ({ status, currentStatus, onClick, children }) => {
    const isDisabled = isLoading || !canChangeStatus(currentStatus, status.toLowerCase());
    const isCurrent = currentStatus.toLowerCase() === status.toLowerCase();
    
    return (
      <button
        className={`manufacturEdit${status === 'Shipped' ? '1' : '2'}`}
        onClick={onClick}
        style={{ 
          opacity: isDisabled ? 0.6 : 1,
          cursor: isDisabled ? 'not-allowed' : 'pointer',
          color: isCurrent ? '#666' : '#fff',
          fontWeight: isCurrent ? 'bold' : 'normal',
          position: 'relative'
        }}
        disabled={isDisabled}
        title={getButtonTooltip(currentStatus, status)}
      >
        {isLoading ? (
          <span className="loading-indicator">
            Processing...
          </span>
        ) : (
          <span>
            {isCurrent ? `✓ ${children}` : children}
          </span>
        )}
      </button>
    );
  };

  const getButtonTooltip = (currentStatus, targetStatus) => {
    if (currentStatus.toLowerCase() === 'fulfilled') 
      return 'Order is already fulfilled and cannot be changed';
    if (currentStatus.toLowerCase() === targetStatus.toLowerCase()) 
      return `Order is already ${targetStatus}`;
    if (currentStatus.toLowerCase() === 'pending' && targetStatus.toLowerCase() === 'fulfilled') 
      return 'Order must be shipped before it can be fulfilled';
    return `Change order status to ${targetStatus}`;
  };

  const canChangeStatus = (currentStatus, targetStatus) => {
    if (currentStatus.toLowerCase() === 'fulfilled') return false;
    if (currentStatus.toLowerCase() === targetStatus) return false;
    if (currentStatus.toLowerCase() === 'pending' && targetStatus === 'fulfilled') return false;
    return true;
  };

  return (
    <div className="list">
      <Sidebar />
      <div className="listContainer">
      <Navbar logOut={logOut}/>
      <div className="manufactur">
          <div className="manufacturTitleContainer">
            <h1 className="manufacturTitle">Supplier Order</h1>
            <Link to="/supplierorders/newordersupply">
              <button className="manufacturAddButton">Order</button>
            </Link>
          </div>
          <div className="manufacturTop">
            <div className="manufacturTopRight">
              <div className="manufacturInfoTop">
              <p className="categoryName">
                <span className="spanform">supplierName: </span>
                {supporderdata.supplierName}
              </p>
                <p className="paddorder">
                  <span className="spanform">id: </span>
                  {supporderdata.id}
                </p>
                <p className="paddorder">
                  <span className="spanform">supplierId: </span>
                  {supporderdata.supplierId}
                </p>

                <p className="paddorder">
                  <span className="spanform">totalQty: </span>
                  {supporderdata.totalQty}
                </p>
                <p className="paddorder">
                  <span className="spanform">subTotalPrice: </span>
                  {supporderdata.subTotalPrice}
                </p>
                <p className="paddorder">
                  <span className="spanform">shippingCost: </span>
                  {supporderdata.shippingCost}
                </p>
                <p className="paddorder">
                  <span className="spanform">totalPrice: </span>
                  {supporderdata.totalPrice}
                </p>
                <p className="paddorder">
                  <span className="spanform">orderStatusId: </span>
                  {supporderdata.orderStatusId}
                </p>
                <p className="paddorder">
                  <span className="spanform">orderStatus: </span>
                  {supporderdata.orderStatus}
                </p>
                <p className="paddorder">
                  <span className="spanform">orderingDate: </span>
                  {supporderdata.orderingDate}
                </p>
                <p className="paddorder">
                  <span className="spanform">expectedArrivalDate: </span>
                  {supporderdata.expectedArrivalDate}
                </p>
              </div>
              <h3>Order Status:</h3>
              <div style={{ 
                display: "flex", 
                gap: "20px", 
                marginBottom: "20px",
                flexDirection: window.innerWidth < 768 ? "column" : "row" 
              }}>
                <div className="status-container">
                  <div className="current-status">
                    Current Status: 
                    <span className={`status-badge status-${supporderdata.orderStatus.toLowerCase()}`}>
                      {supporderdata.orderStatus}
                    </span>
                  </div>
                  <div className="status-actions">
                    <StatusButton 
                      status="Shipped" 
                      currentStatus={supporderdata.orderStatus}
                      onClick={handleShipped}
                    >
                      Mark as Shipped
                    </StatusButton>
                    <StatusButton 
                      status="Fulfilled" 
                      currentStatus={supporderdata.orderStatus}
                      onClick={handleFulfilled}
                    >
                      Mark as Fulfilled
                    </StatusButton>
                  </div>
                </div>
              </div>
              <hr />
              <h3>Ordered Materials:  </h3>
              <table className="table-striped">
                <thead className="header">
                  <tr>
                    <th>Material-ID</th>
                    <th>Material-Name</th>
                    <th>price Per Unit (Sales Price)</th>
                    <th>quantity</th>
                    <th>price</th>
                  </tr>
                </thead>
                <tbody>
                  {supporderdata.orderedMaterials.map((item) => (
                    <tr key={item.id}>
                      <td>{item.materialId}</td>
                      <td>{item.materialName}</td>
                      <td>{item.salesPrice}</td>
                      <td>{item.quantity}</td>
                      <td>{item.price}</td>
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
