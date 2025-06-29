import "./newmanufactur.scss";
import Sidebar from "./../../Components/sidebar/Sidebar";
import Navbar from "./../../Components/navbar/Navbar";
import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from 'sweetalert2';
import axios from "axios";
import ProductInventoryContext from "../../context/ProductInventoryContext";
import RowMaterial from "./RowMaterial/RowMaterial";

function NewManufactur({ inputs, title, logOut }) {
  const [file, setFile] = useState(null);
  const { getProductInventory, data } = useContext(ProductInventoryContext);
  const navigate = useNavigate();

  const [manufactur, setManufactur] = useState({
    productManufacturedId: "",
    qtyToManufacture: '',
    manufacturingCost: '',
    startingDate: new Date().toISOString().slice(0, 16), // Format for datetime-local input
    rawMaterialsUsed: [
      {
        materialId: "",
        qty: 0,
      },
    ],
  });

  const [errors, setErrors] = useState({});

  const productInvOptions = data?.map((product) => (
    <option value={product.productId} key={product.productId}>
      {product.productId} - {product.productName}
    </option>
  ));

  useEffect(() => {
    getProductInventory();
  }, [getProductInventory]);

  const deleteRow = (indx) => {
    setManufactur(prev => ({
      ...prev,
      rawMaterialsUsed: prev.rawMaterialsUsed.filter((_, index) => index !== indx)
    }));
  };

  const addMaterial = () => {
    setManufactur(prev => ({
      ...prev,
      rawMaterialsUsed: [
        ...prev.rawMaterialsUsed,
        { materialId: "", qty: 0 }
      ]
    }));
  };

  const validateField = (name, value) => {
    switch (name) {
      case 'qtyToManufacture':
      case 'manufacturingCost':
        return value <= 0 ? `${name} must be greater than 0` : '';
      case 'productManufacturedId':
        return !value ? 'Please select a product' : '';
      case 'startingDate':
        return !value ? 'Please select a starting date' : '';
      default:
        return '';
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    const error = validateField(name, value);
    
    setErrors(prev => ({
      ...prev,
      [name]: error
    }));

    setManufactur(prev => ({
      ...prev,
      [name]: ['qtyToManufacture', 'manufacturingCost'].includes(name) 
        ? parseFloat(value) || 0 
        : value
    }));
  };

  const handleMaterialChange = (index, field, value) => {
    setManufactur(prev => ({
      ...prev,
      rawMaterialsUsed: prev.rawMaterialsUsed.map((item, i) => 
        i === index 
          ? { 
              ...item, 
              [field]: field === 'qty' ? parseFloat(value) || 0 : value 
            }
          : item
      )
    }));
  };

  async function sendData() {
    try {
      // Validate all required fields
      const newErrors = {};
      Object.keys(manufactur).forEach(key => {
        if (key !== 'rawMaterialsUsed') {
          const error = validateField(key, manufactur[key]);
          if (error) newErrors[key] = error;
        }
      });

      // Validate raw materials
      const invalidMaterials = manufactur.rawMaterialsUsed.filter(material => 
        !material.materialId || material.qty <= 0
      );

      if (invalidMaterials.length > 0) {
        newErrors.rawMaterials = 'Please fill in all raw material IDs and quantities with valid values';
      }

      if (Object.keys(newErrors).length > 0) {
        setErrors(newErrors);
        throw new Error('Please fix the validation errors');
      }

      // Format the data according to API requirements
      const manufacturData = {
        productManufacturedId: parseInt(manufactur.productManufacturedId),
        qtyToManufacture: parseInt(manufactur.qtyToManufacture),
        manufacturingCost: parseFloat(manufactur.manufacturingCost),
        startingDate: new Date(manufactur.startingDate).toISOString(),
        rawMaterialsUsed: manufactur.rawMaterialsUsed.map(material => ({
          materialId: parseInt(material.materialId),
          qty: parseFloat(material.qty)
        }))
      };

      const res = await axios.post(
        'https://localhost:5001/api/CreateManufacturingOrder',
        manufacturData
      );

      if (res.data) {
        Swal.fire({
          icon: 'success',
          title: 'Added!',
          text: 'Manufacturing order has been created successfully.',
          showConfirmButton: false,
          timer: 1500
        });
        navigate("/manufactur");
        return true;
      }
      return false;
    } catch (err) {
      console.error("Error:", err.response?.data || err.message);
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: err.response?.data || err.message || 'An error occurred while creating the manufacturing order',
      });
      return false;
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    const success = await sendData();
    if (success) {
      navigate("/manufactur");
    }
  };

  const renderedRowMaterials = manufactur.rawMaterialsUsed.map(
    (material, indx) => (
      <RowMaterial
        key={indx}
        indx={indx}
        manufactur={manufactur}
        setManufactur={setManufactur}
        material={material}
        deleteRow={deleteRow}
        handleMaterialChange={handleMaterialChange}
      />
    )
  );

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
              <div className="formInput">
                <label htmlFor="productManufacturedId">
                  Product to Manufacture
                </label>
                <select
                  name="productManufacturedId"
                  id="productManufacturedId"
                  onChange={handleInputChange}
                  value={manufactur.productManufacturedId}
                  className={errors.productManufacturedId ? 'error' : ''}
                >
                  <option value="">Select a product</option>
                  {productInvOptions}
                </select>
                {errors.productManufacturedId && (
                  <span className="error-message">{errors.productManufacturedId}</span>
                )}
              </div>

              {inputs.map((input) => (
                <div className="formInput" key={input.id}>
                  <label>{input.label}</label>
                  <input
                    type={input.type}
                    name={input.name}
                    onChange={handleInputChange}
                    placeholder={input.placeholder}
                    value={manufactur[input.name]}
                    className={errors[input.name] ? 'error' : ''}
                  />
                  {errors[input.name] && (
                    <span className="error-message">{errors[input.name]}</span>
                  )}
                </div>
              ))}

              <div className="raw-materials-section">
                <h3>Raw Materials</h3>
                {renderedRowMaterials}
                <button type="button" onClick={addMaterial} className="btnadd">
                  Add Raw Material
                </button>
                {errors.rawMaterials && (
                  <span className="error-message">{errors.rawMaterials}</span>
                )}
              </div>

              <button type="submit" className="btnsend">Create Manufacturing Order</button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default NewManufactur;
