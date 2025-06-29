import React, { useContext, useEffect, useState } from "react";
import RawMatrialContext from "../../../context/RawMatrialContext";
import "./rowMatrialSupply.css";

export default function RowMatrialSupply({
  supplyMatraial,
  setSupplyMatraial,
  indx,
  material,
  deleteRow
}) {
  const { getAllRawMaterial, data } = useContext(RawMatrialContext);
  const [rowInputs, setRowInputs] = useState({
    materialId: material?.materialId || "",
    pricePerUnit: material?.pricePerUnit || ""
  });

  useEffect(() => {
    getAllRawMaterial();
  }, []);

  // Update local state when parent state changes
  useEffect(() => {
    setRowInputs({
      materialId: material?.materialId || "",
      pricePerUnit: material?.pricePerUnit || ""
    });
  }, [material]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    
    // Validate price - prevent negative or zero prices
    if (name === "pricePerUnit" && Number(value) <= 0) {
      return;
    }

    const newInputs = {
      ...rowInputs,
      [name]: name === "pricePerUnit" ? Number(value) : value
    };
    
    setRowInputs(newInputs);

    // Update parent state immutably
    setSupplyMatraial(prev => ({
      ...prev,
      supplyingMaterialDetails: prev.supplyingMaterialDetails.map((item, idx) =>
        idx === indx ? newInputs : item
      )
    }));
  };

  return (
    <div style={{ display: "flex", gap: "20px", marginBottom: "10px" }}>
      <select
        name="materialId"
        onChange={handleChange}
        value={rowInputs.materialId}
        style={{ padding: "8px", minWidth: "200px" }}
      >
        <option value="">Select material</option>
        {data?.map((material) => (
          <option key={material.materialId} value={material.materialId}>
            {material.materialId} - {material.materialName}
          </option>
        ))}
      </select>

      <input
        type="number"
        name="pricePerUnit"
        onChange={handleChange}
        value={rowInputs.pricePerUnit}
        placeholder="Price per unit"
        min="0.01"
        step="0.01"
        style={{ padding: "8px", width: "150px" }}
      />

      <button
        className="deleteButtonrow"
        onClick={() => deleteRow(indx)}
        type="button"
      >
        Remove
      </button>
    </div>
  );
}
