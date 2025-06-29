import "./newSupplierMatrial.scss";
import Sidebar from "./../../Components/sidebar/Sidebar";
import Navbar from "./../../Components/navbar/Navbar";
import { useContext, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import Swal from 'sweetalert2';
import axios from "axios";
import ProductInventoryContext from "../../context/ProductInventoryContext";
import RowMatrialSupply from "./RowMatrialSupply/RowMatrialSupply";
import SupplierContext from "../../context/SupplierContext";

function NewSupplierMatrial({ title, inputs, logOut }) {
  const navigate = useNavigate();
  const { getProductInventory } = useContext(ProductInventoryContext);
  const { getSuplliermatrialById } = useContext(SupplierContext);
  const { suppliermatrialId } = useParams();

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [viewSupplierMaterials, setViewSupplierMaterials] = useState([]);
  const [supplyMaterial, setSupplyMaterial] = useState({
    supplyingMaterialDetails: [
      {
        materialId: "",
        pricePerUnit: ""
      },
    ],
  });

  // Error handling utility functions
  const handleApiError = (error, customMessage) => {
    console.error('API Error:', error);
    const errorMessage = error.response?.data?.message 
      || error.response?.data 
      || error.message 
      || customMessage 
      || 'An unexpected error occurred';
    
    setError(errorMessage);
    Swal.fire({
      icon: 'error',
      title: 'Error',
      text: errorMessage,
      confirmButtonText: 'OK'
    });
  };

  const showWarning = (message) => {
    Swal.fire({
      icon: 'warning',
      title: 'Warning',
      text: message,
      confirmButtonText: 'OK'
    });
  };

  useEffect(() => {
    let isMounted = true;

    const fetchData = async () => {
      try {
        setError(null);
        setIsLoading(true);

        if (!suppliermatrialId) {
          throw new Error('Supplier ID is missing');
        }

        const inventoryPromise = getProductInventory().catch(error => {
          throw new Error('Failed to load product inventory: ' + error.message);
        });

        const materialsPromise = getSuplliermatrialById(suppliermatrialId).catch(error => {
          throw new Error('Failed to load supplier materials: ' + error.message);
        });

        const [_, materialsResponse] = await Promise.all([
          inventoryPromise,
          materialsPromise
        ]);

        if (!isMounted) return;

        if (materialsResponse?.data) {
          setViewSupplierMaterials(materialsResponse.data);
        } else {
          throw new Error('No data received from the server');
        }
      } catch (error) {
        if (!isMounted) return;
        handleApiError(error, 'Failed to load supplier materials');
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };
    
    fetchData();

    return () => {
      isMounted = false;
    };
  }, [suppliermatrialId, getProductInventory, getSuplliermatrialById]);

  const validateForm = () => {
    const errors = [];

    if (!suppliermatrialId) {
      errors.push('Supplier ID is required');
    }

    if (supplyMaterial.supplyingMaterialDetails.length === 0) {
      errors.push('At least one material must be added');
    }

    // Check for duplicate material IDs
    const materialIds = new Set();
    supplyMaterial.supplyingMaterialDetails.forEach((detail, index) => {
      if (!detail.materialId) {
        errors.push(`Material ID is required for row ${index + 1}`);
      } else if (materialIds.has(detail.materialId)) {
        errors.push(`Duplicate Material ID found in row ${index + 1}`);
      } else {
        materialIds.add(detail.materialId);
      }

      if (!detail.pricePerUnit) {
        errors.push(`Price per unit is required for row ${index + 1}`);
      } else {
        const price = Number(detail.pricePerUnit);
        if (isNaN(price)) {
          errors.push(`Invalid price format in row ${index + 1}`);
        } else if (price <= 0) {
          errors.push(`Price per unit must be greater than 0 in row ${index + 1}`);
        }
      }
    });

    // Check for existing materials
    supplyMaterial.supplyingMaterialDetails.forEach((detail) => {
      const existingMaterial = viewSupplierMaterials.find(m => m.materialId === detail.materialId);
      if (existingMaterial) {
        errors.push(`Material ID ${detail.materialId} is already assigned to this supplier`);
      }
    });

    return errors;
  };

  const deleteRow = (index) => {
    if (supplyMaterial.supplyingMaterialDetails.length === 1) {
      showWarning('At least one material is required');
      return;
    }

    setSupplyMaterial(prev => ({
      ...prev,
      supplyingMaterialDetails: prev.supplyingMaterialDetails.filter((_, idx) => idx !== index)
    }));
  };

  const addMaterial = () => {
    const maxMaterials = 10; // Set a reasonable limit
    if (supplyMaterial.supplyingMaterialDetails.length >= maxMaterials) {
      showWarning(`Maximum ${maxMaterials} materials allowed per submission`);
      return;
    }

    setSupplyMaterial(prev => ({
      ...prev,
      supplyingMaterialDetails: [
        ...prev.supplyingMaterialDetails,
        { materialId: "", pricePerUnit: "" }
      ]
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const validationErrors = validateForm();
    if (validationErrors.length > 0) {
      Swal.fire({
        icon: 'error',
        title: 'Validation Error',
        html: validationErrors.join('<br>'),
        confirmButtonText: 'OK'
      });
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const payload = {
        supplierId: Number(suppliermatrialId),
        supplyingMaterialDetails: supplyMaterial.supplyingMaterialDetails.map(detail => ({
          materialId: Number(detail.materialId),
          pricePerUnit: Number(detail.pricePerUnit)
        }))
      };

      const response = await axios.post(
        'https://localhost:5001/api/AddNewSupplyingMaterialToSupplier',
        payload,
        { headers: { 'Content-Type': 'application/json' } }
      );
      console.log("response",response);
      if (response.status === 204) {
        await Swal.fire({
          icon: 'success',
          title: 'Success!',
          text: 'Supply materials have been added successfully',
          timer: 1500,
          showConfirmButton: false
        });
        navigate("/supplier");
      } else {
        throw new Error('Server responded with an error');
      }
    } catch (error) {
      handleApiError(error, 'Failed to add supply materials');
    } finally {
      setIsLoading(false);
    }
  };

  const renderedRowMaterials = supplyMaterial.supplyingMaterialDetails.map(
    (material, indx) => (
      <RowMatrialSupply
        key={indx}
        indx={indx}
        supplyMatraial={supplyMaterial}
        setSupplyMatraial={setSupplyMaterial}
        material={material}
        deleteRow={deleteRow}
      />
    )
  );

  if (error) {
    return (
      <div className="newmanufactur">
        <Sidebar />
        <div className="newContainer">
          <Navbar logOut={logOut}/>
          <div className="error-message">
            <h3>Error</h3>
            <p>{error}</p>
            <button 
              onClick={() => setError(null)} 
              className="retry-button"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (isLoading && !viewSupplierMaterials.length) {
    return (
      <div className="newmanufactur">
        <Sidebar />
        <div className="newContainer">
          <Navbar logOut={logOut}/>
          <div className="loading-message">
            <p>Loading supplier materials...</p>
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
            {viewSupplierMaterials.length > 0 && (
              <div className="existing-materials">
                <h3>Existing Materials</h3>
                <table className="materials-table">
                  <thead>
                    <tr>
                      <th>Material ID</th>
                      <th>Material Name</th>
                      <th>Price Per Unit</th>
                    </tr>
                  </thead>
                  <tbody>
                    {viewSupplierMaterials.map(item => (
                      <tr key={item.id}>
                        <td>{item.materialId}</td>
                        <td>{item.materialName}</td>
                        <td>{item.pricePerUnit}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            <form onSubmit={handleSubmit}>
              <div className="form-materials">
                <h3>Add New Materials</h3>
                {renderedRowMaterials}
              </div>
              <div className="form-actions">
                <button 
                  type="button" 
                  onClick={addMaterial} 
                  className="btnadd" 
                  disabled={isLoading}
                >
                  Add Material
                </button>
                <button 
                  type="submit" 
                  className="btnsend" 
                  disabled={isLoading}
                >
                  {isLoading ? 'Saving...' : 'Save Materials'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default NewSupplierMatrial;
