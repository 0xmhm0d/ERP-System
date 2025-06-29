import "./NewDistributionOrders.scss";
import Sidebar from "../../Components/sidebar/Sidebar";
import Navbar from "../../Components/navbar/Navbar";
import { useContext, useEffect, useState, useCallback, useMemo } from "react";
import axios from "axios";
import DistributorContext from "../../context/DistributorContext";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";

function NewDistributionOrders() {
  const { getAlldistributor, getDistributorProduct, data, productData } =
    useContext(DistributorContext);
  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [isInitialized, setIsInitialized] = useState(false);

  const [newDistOrder, setnewDistOrder] = useState({
    distributorId: 0,
    productsOrdered: [
      {
        productId: 0,
        qty: "",
      },
    ],
  });

  const newDistOrderOptions = useMemo(() => 
    data?.map((dist) => (
      <option value={dist.distributorId} key={dist.distributorId}>
        Id: {dist.distributorId} -- Name: {dist.distributorName}
      </option>
    )) || [], [data]
  );

  const newProductOptions = useMemo(() => 
    productData?.map((dist) => (
      <option value={dist.productId} key={dist.productId}>
        Id: {dist.productId} -- Name: {dist.productName}
      </option>
    )) || [], [productData]
  );

  useEffect(() => {
    if (!isInitialized) {
      const fetchData = async () => {
        try {
          setError(null);
          setIsLoading(true);
          await Promise.all([getAlldistributor(), getDistributorProduct()]);
          setIsInitialized(true);
        } catch (err) {
          setError('Failed to fetch initial data');
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Failed to load required data. Please try again.',
          });
        } finally {
          setIsLoading(false);
        }
      };
      fetchData();
    }
  }, [isInitialized, getAlldistributor, getDistributorProduct]);

  const deleteRow = useCallback((indx, e) => {
    e.preventDefault(); // Prevent form submission
    if (newDistOrder.productsOrdered.length <= 1) {
      Swal.fire({
        icon: 'warning',
        title: 'Cannot Delete',
        text: 'At least one product is required',
      });
      return;
    }

    setnewDistOrder(prev => ({
      ...prev,
      productsOrdered: prev.productsOrdered.filter((_, index) => index !== indx)
    }));
  }, []);

  const addProduct = useCallback((e) => {
    e.preventDefault(); // Prevent form submission
    setnewDistOrder(prev => ({
      ...prev,
      productsOrdered: [
        ...prev.productsOrdered,
        {
          productId: 0,
          qty: "",
        }
      ]
    }));
  }, []);

  const validateForm = useCallback(() => {
    if (!newDistOrder.distributorId || newDistOrder.distributorId === 0) {
      throw new Error('Please select a distributor');
    }

    const invalidProducts = newDistOrder.productsOrdered.some(
      product => !product.productId || product.productId === 0 || !product.qty || product.qty <= 0
    );

    if (invalidProducts) {
      throw new Error('Please fill in all product details with valid quantities');
    }

    // Check for duplicate products
    const productIds = newDistOrder.productsOrdered.map(p => p.productId);
    if (new Set(productIds).size !== productIds.length) {
      throw new Error('Duplicate products are not allowed');
    }
  }, [newDistOrder]);

  const handleChange = useCallback((e) => {
    const { name, value } = e.target;
    const numValue = parseInt(value);
    
    if (isNaN(numValue) && value !== '') {
      return; // Don't update if not a number and not empty
    }
    
    setnewDistOrder(prev => ({
      ...prev,
      [name]: value === '' ? 0 : numValue
    }));
  }, []);

  const handleInputChange = useCallback((e, indx) => {
    const { name, value } = e.target;
    
    if (name === 'qty' && value !== '' && isNaN(parseInt(value))) {
      return; // Don't update if qty is not a number and not empty
    }
    
    setnewDistOrder(prev => {
      const updatedProducts = [...prev.productsOrdered];
      updatedProducts[indx] = {
        ...updatedProducts[indx],
        [name]: name === 'qty' ? (value === '' ? '' : parseInt(value) || 0) : value
      };
      return {
        ...prev,
        productsOrdered: updatedProducts
      };
    });
  }, []);

  const handleProductChange = useCallback((e, indx) => {
    const { value } = e.target;
    const productId = parseInt(value);
    
    if (isNaN(productId) && value !== '') {
      return; // Don't update if not a number and not empty
    }
    
    setnewDistOrder(prev => {
      const updatedProducts = [...prev.productsOrdered];
      updatedProducts[indx] = {
        ...updatedProducts[indx],
        productId: value === '' ? 0 : productId
      };
      return {
        ...prev,
        productsOrdered: updatedProducts
      };
    });
  }, []);

  const sendData = useCallback(async () => {
    if (isLoading) return; // Prevent multiple submissions

    try {
      setIsLoading(true);
      setError(null);
      
      validateForm();

      await axios.post(
        `https://localhost:5001/api/CreateDistributionOrder`,
        newDistOrder
      );

      Swal.fire({
        icon: "success",
        title: "Added!",
        text: `Distribution order has been added successfully.`,
        showConfirmButton: false,
        timer: 1500,
      });

      navigate("/distributororders");
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to create distribution order';
      setError(errorMessage);
      Swal.fire({
        icon: "error",
        title: "Error",
        text: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  }, [isLoading, navigate, newDistOrder, validateForm]);

  const handleSubmit = useCallback((e) => {
    e.preventDefault();
    sendData();
  }, [sendData]);

  if (isLoading && !isInitialized) {
    return (
      <div className="newmanufactur">
        <Sidebar />
        <div className="newContainer">
          <Navbar />
          <div className="loading-container" style={{ 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center', 
            height: '50vh' 
          }}>
            <div>Loading...</div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="newmanufactur">
      <Sidebar />
      <div className="newContainer">
        <Navbar />
        <div className="top">
          <h1>New Distribution Order</h1>
        </div>
        <div className="bottom">
          <div className="right">
            {error && (
              <div className="error-message" style={{ 
                color: 'red', 
                marginBottom: '1rem', 
                padding: '0.5rem', 
                border: '1px solid red', 
                borderRadius: '4px' 
              }}>
                {error}
              </div>
            )}
            <form onSubmit={handleSubmit}>
              <div style={{ display: "flex", gap: "20px", marginBottom: "20px" }}>
                <label htmlFor="distributorId">Distributor</label>
                <select
                  name="distributorId"
                  id="distributorId"
                  value={newDistOrder.distributorId}
                  onChange={handleChange}
                  required
                  disabled={isLoading}
                >
                  <option value="">Select Distributor</option>
                  {newDistOrderOptions}
                </select>
              </div>

              {newDistOrder.productsOrdered.map((product, indx) => (
                <div key={indx} style={{ marginBottom: "20px", padding: "15px", border: "1px solid #ddd", borderRadius: "4px" }}>
                  <div style={{ marginBottom: "10px" }}>
                    <label htmlFor={`product-${indx}`}>Product</label>
                    <select
                      name="productId"
                      id={`product-${indx}`}
                      value={product.productId}
                      onChange={(e) => handleProductChange(e, indx)}
                      required
                      disabled={isLoading}
                    >
                      <option value="">Select Product</option>
                      {newProductOptions}
                    </select>
                  </div>
                  <div style={{ marginBottom: "10px" }}>
                    <label>Quantity</label>
                    <input
                      type="number"
                      name="qty"
                      value={product.qty}
                      onChange={(e) => handleInputChange(e, indx)}
                      min="1"
                      required
                      disabled={isLoading}
                    />
                  </div>
                  <button 
                    type="button"
                    onClick={(e) => deleteRow(indx, e)}
                    disabled={isLoading}
                    style={{ 
                      backgroundColor: "#ff4444",
                      color: "white",
                      border: "none",
                      padding: "5px 10px",
                      borderRadius: "4px",
                      cursor: isLoading ? "not-allowed" : "pointer",
                      opacity: isLoading ? 0.7 : 1
                    }}
                  >
                    Delete Row
                  </button>
                </div>
              ))}

              <button 
                type="button" 
                onClick={addProduct} 
                className="btnadd"
                disabled={isLoading}
                style={{
                  backgroundColor: "#4CAF50",
                  color: "white",
                  border: "none",
                  padding: "10px 20px",
                  borderRadius: "4px",
                  marginRight: "10px",
                  cursor: isLoading ? "not-allowed" : "pointer",
                  opacity: isLoading ? 0.7 : 1
                }}
              >
                Add New Product
              </button>

              <button 
                type="submit" 
                className="btnsend" 
                disabled={isLoading}
                style={{
                  backgroundColor: "#2196F3",
                  color: "white",
                  border: "none",
                  padding: "10px 20px",
                  borderRadius: "4px",
                  cursor: isLoading ? "not-allowed" : "pointer",
                  opacity: isLoading ? 0.7 : 1
                }}
              >
                {isLoading ? 'Creating Order...' : 'Create Order'}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default NewDistributionOrders;
