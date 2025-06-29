import "./newsupplierorder.scss";
import Sidebar from "./../../Components/sidebar/Sidebar";
import Navbar from "./../../Components/navbar/Navbar";
import { useContext, useEffect, useState } from "react";
import axios from "axios";
import SupplierContext from "../../context/SupplierContext";
import RawMatrialContext from "../../context/RawMatrialContext";
import { useNavigate } from "react-router-dom";
import Swal from 'sweetalert2';

function NewSupplierorder({ inputs, title, logOut }) {
  const { getAllsupplier, data } = useContext(SupplierContext);
  const { getAllRawMaterial, isLoading: rawMaterialLoading } = useContext(RawMatrialContext);
  const [getRawMaterial, setGetRawMaterial] = useState([]);
  const [error, setError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigate = useNavigate();

  const [supporder, setSupporder] = useState({
    supplierId: 0,
    shippingCost: 0,
    orderedMaterials: []
  });

  useEffect(() => {
    const loadRawMaterials = async () => {
      try {
        const materials = await getAllRawMaterial();
        if (!materials) {
          throw new Error('Failed to load raw materials');
        }
        setGetRawMaterial(materials);
      } catch (err) {
        setError('Failed to load raw materials. Please try again.');
        console.error('Error loading raw materials:', err);
      }
    };
    loadRawMaterials();
  }, []);

  useEffect(() => {
    getAllsupplier();
  }, []);

  const supporderOptions = data?.map((supp) => (
    <option value={supp.supplierId} key={supp.supplierId}>
      {supp.supplierId} - {supp.supplierName}
    </option>
  ));
  const rowMaterialOptions = getRawMaterial?.map((material) => (
    <option value={material.materialId} key={material.materialId}>
      {material.materialId} - {material.materialName}
    </option>
  ));

  const deleteRow = (indx) => {
    setSupporder(prev => ({
      ...prev,
      orderedMaterials: prev.orderedMaterials.filter((_, index) => index !== indx)
    }));
  };
  const addMaterial = () => {
    setSupporder(prev => ({
      ...prev,
      orderedMaterials: [
        ...prev.orderedMaterials,
        {
          materialId: 0,
          qty: 0
        }
      ]
    }));
    getAllRawMaterial().then((res)=>{
      setGetRawMaterial(res);
    })
  };

  async function handleInputChange (e)  {
    const { name, value } = e.target;
    setSupporder(prev => ({
      ...prev,
      [name]: name === "shippingCost" ? Number(value) : value
    }));
  };
  async function handleInputChanges (e)  {
    const { name, value } = e.target;
    const numericValue = Number(value);
    setSupporder(prev => ({
      ...prev,
      [name]: name === "supplierId" ? numericValue : value,
      orderedMaterials: prev.orderedMaterials.map(material => ({
        ...material
      }))
    }));
  };

  async function sendData(id, cost) {
    setIsSubmitting(true);
    setError(null);
    try {
      if (!id) {
        throw new Error('Please select a supplier first');
      }
      const formattedMaterials = supporder.orderedMaterials.map(material => ({
        materialId: Number(material.materialId),
        qty: Number(material.qty)
      })).filter(material => material.materialId && material.qty > 0);
      if (formattedMaterials.length === 0) {
        throw new Error('Please add at least one material with quantity');
      }
      console.log("formattedMaterials",formattedMaterials);
      const res = await axios.post(
        `https://localhost:5001/api/OrderRawMaterialFromSupplier?supplierId=${Number(id)}&shippingCost=${cost}`,
        formattedMaterials
      );
      if (res.status === 200) {
        Swal.fire({
          icon: 'success',
          title: 'Added!',
          text: 'Supplier order has been Added.',
          showConfirmButton: false,
          timer: 1500
        });
        navigate("/supplierorders");
      }
    } catch (err) {
      const errorMessage = err.message || 'Failed to create supplier order. Please ensure all materials are associated with the selected supplier.';
      setError(errorMessage);
      Swal.fire({
        icon: 'error',
        title: 'Error!',
        text: errorMessage,
        showConfirmButton: true
      });
    } finally {
      setIsSubmitting(false);
    }
  }

  const handleSubmit = (e) => {
    e.preventDefault();

    sendData(supporder.supplierId,supporder.shippingCost);
    
  };

  if (rawMaterialLoading) {
    return (
      <div className="newmanufactur">
        <Sidebar />
        <div className="newContainer">
          <Navbar logOut={logOut}/>
          <div className="loading">Loading raw materials...</div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="newmanufactur">
        <Sidebar />
        <div className="newContainer">
          <Navbar logOut={logOut}/>
          <div className="error-container">
            <div className="error-message">{error}</div>
            <button onClick={() => setError(null)} className="retry-button">
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="newmanufactur">
      <Sidebar />
      <div className="newContainer">
        <Navbar logOut={logOut}/>
        <div className="top">
          <h1>{title}</h1>
        </div>

        <div className="bottom">
          <div className="right">
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>choose Supplier</label>
                <select
                  name="supplierId"
                  onChange={handleInputChanges}
                  value={supporder.supplierId}
                >
                  <option value="">Select a supplier</option>
                  {supporderOptions}
                </select>
              </div>

              <div className="form-group">
                <label>shippingCost</label>
                <input
                  type="number"
                  name="shippingCost"
                  onChange={handleInputChange}
                  placeholder="Enter shipping cost"
                  value={supporder.shippingCost}
                />
              </div>

              {supporder.orderedMaterials.map((material, indx) => (
                <div key={indx} className="material-row">
                  <select
                    value={material.materialId || 0}
                    onChange={(e) => {
                      setSupporder(prev => ({
                        ...prev,
                        orderedMaterials: prev.orderedMaterials.map((item, index) => 
                          index === indx ? { ...item, materialId: Number(e.target.value) } : item
                        )
                      }));
                    }}
                  >
                    <option value={0}>Select raw material</option>
                    {rowMaterialOptions}
                  </select>
                  <input
                    type="number"
                    placeholder="Quantity"
                    value={material.qty || 0}
                    onChange={(e) => {
                      setSupporder(prev => ({
                        ...prev,
                        orderedMaterials: prev.orderedMaterials.map((item, index) => 
                          index === indx ? { ...item, qty: Number(e.target.value) } : item
                        )
                      }));
                    }}
                  />
                  <button 
                    type="button" 
                    className="remove-row"
                    onClick={() => deleteRow(indx)}
                  >
                    Remove Row
                  </button>
                </div>
              ))}

              <button 
                type="button" 
                className="add-material-btn" 
                onClick={addMaterial}
              >
                Add Row Material
              </button>

              <button 
                type="submit" 
                className="send-btn" 
                disabled={isSubmitting}
              >
                {isSubmitting ? 'Sending...' : 'Send'}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default NewSupplierorder;
