import React, { useContext, useEffect, useState } from "react";
import RawMatrialContext from "../../../context/RawMatrialContext";
import SupplierContext from "../../../context/SupplierContext";
import "./rowmatrial.css";

export default function RowMaterial({
  supporder,
  setSupporder,
  indx,
  deleteRow
}) {
  const { getAllRawMatrial, data } = useContext(RawMatrialContext);
  const { getSuplliermatrialById } = useContext(SupplierContext);
  
  const [rowInputs, setRowInputs] = useState({ materialId: "", qty: "" });
  const [supplyMaterial, setSupplyMaterial] = useState([]);

  useEffect(() => {
    getAllRawMatrial();
  }, [getAllRawMatrial]);

  const handleChange = async (e) => {
    const { name, value } = e.target;
    const newValue = Number(value);

    setRowInputs(prev => ({
      ...prev,
      [name]: newValue
    }));

    // Update parent state
    const orderedMaterials = [...supporder.orderedMaterials];
    orderedMaterials[indx] = {
      ...orderedMaterials[indx],
      [name]: newValue
    };
    
    setSupporder(prev => ({
      ...prev,
      orderedMaterials
    }));

    // Fetch supplier material data if materialId changes
    if (name === 'materialId') {
      try {
        const response = await getSuplliermatrialById(newValue);
        setSupplyMaterial(response.data);
      } catch (error) {
        console.error('Error fetching supplier material:', error);
      }
    }
  };

  return (
    <div style={{ display: "flex", gap: "20px" }}>
      <select 
        name="materialId" 
        onChange={handleChange} 
        value={rowInputs.materialId}
      >
        <option value="">Select raw material</option>
        {data?.map((material) => (
          <option key={material.materialId} value={material.materialId}>
            {material.materialName}
          </option>
        ))}
      </select>

      <input
        type="number"
        name="qty"
        onChange={handleChange}
        value={rowInputs.qty}
        placeholder="Quantity"
        min="0"
      />

      <button 
        className="deleteButtonrow" 
        onClick={() => deleteRow(indx)}
        type="button"
      >
        Remove Row
      </button>
    </div>
  );
}
