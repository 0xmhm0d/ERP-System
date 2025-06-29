import React, { useContext, useEffect, useState } from "react";
import "./rowmatrial.css";
import RawMaterialContext from "../../../context/RawMatrialContext";

// Rename to RawMaterial for consistency
export default function RawMaterial({
  manufactur,
  setManufactur,
  indx,
  material,
  deleteRow
}) {
  const { getAllRawMaterial } = useContext(RawMaterialContext);
  
  const [rawMaterials, setRawMaterials] = useState([]);
  const [rowInputs, setRowInputs] = useState({
    materialId: material.materialId || "",
    qty: material.qty || ""
  });

  useEffect(() => {
    const loadRawMaterials = async () => {
      try {
        const materials = await getAllRawMaterial();
        if (materials) {
          setRawMaterials(materials);
        }
      } catch (error) {
        console.error("Error loading raw materials:", error);
      }
    };
    loadRawMaterials();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Remove getAllRawMaterial from dependencies

  // Update rowInputs when material prop changes
  useEffect(() => {
    setRowInputs({
      materialId: material.materialId || "",
      qty: material.qty || ""
    });
  }, [material]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    
    // Validate quantity - prevent 0 or negative numbers
    if (name === "qty" && Number(value) <= 0) {
      return; // Early return if invalid quantity
    }

    // Create new state object instead of mutating
    const newInputs = {
      ...rowInputs,
      [name]: name === "qty" ? Number(value) : value
    };
    
    setRowInputs(newInputs);

    // Update parent state immutably
    setManufactur(prev => ({
      ...prev,
      rawMaterialsUsed: prev.rawMaterialsUsed.map((item, idx) =>
        idx === indx ? newInputs : item
      )
    }));
  };

  return (
    <div className="raw-material-row">
      <select 
        name="materialId"
        onChange={handleChange}
        value={rowInputs.materialId}
      >
        <option value="">Select raw material</option>
        {rawMaterials.map((material) => (
          <option value={material.materialId} key={material.materialId}>
            {material.materialId} - {material.materialName}
          </option>
        ))}
      </select>

      <input
        type="number"
        name="qty"
        onChange={handleChange}
        value={rowInputs.qty}
        placeholder="Quantity"
        min="1"
      />

      <button 
        className="delete-button-row"
        onClick={() => deleteRow(indx)}
        type="button"
      >
        Remove Row
      </button>
    </div>
  );
}
