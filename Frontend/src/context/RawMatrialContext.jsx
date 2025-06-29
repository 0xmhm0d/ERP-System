import { createContext, useState } from "react";
import axios from "axios";
import Swal from "sweetalert2";

const BASE_URL = "https://localhost:5001/api";
const RawMaterialContext = createContext();

export function RawMaterialContextProvider({ children }) {
  const [data, setData] = useState([]);
  const [data1, setData1] = useState([]);
  const [loading, setLoading] = useState(false);

  const handleApiError = (error, message) => {
    console.error(message, error);
    Swal.fire({
      icon: "error",
      title: "Error",
      text: message,
    });
  };

  async function getAllRawMaterial() {
    try {
      setLoading(true);
      const response = await axios.get(`${BASE_URL}/GetAllRawMaterials`);
      setData1(response.data);
      setData(response.data);
      return response.data;
    } catch (error) {
      handleApiError(error, "Failed to fetch raw materials");
    } finally {
      setLoading(false);
    }
  }

  async function getRawMaterialById(id) {
    try {
      setLoading(true);
      const response = await axios.get(`${BASE_URL}/GetRawMaterialById/${id}`);
      return response;
    } catch (error) {
      handleApiError(error, "Failed to fetch raw material details");
      return null;
    } finally {
      setLoading(false);
    }
  }

  const handleDelete = async (id) => {
    try {
      const result = await Swal.fire({
        icon: "warning",
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
      });

      if (result.isConfirmed) {
        setLoading(true);
        await axios.delete(`${BASE_URL}/DeleteRawMaterial?id=${id}`);
        setData((prevData) => prevData.filter(item => item.materialId !== id));
        
        Swal.fire({
          icon: "success",
          title: "Deleted!",
          showConfirmButton: false,
          timer: 2000,
        });
      } else if (result.dismiss === Swal.DismissReason.cancel) {
        Swal.fire("Cancelled", "Your item is safe", "info");
      }
    } catch (error) {
      handleApiError(error, "Failed to delete raw material");
    } finally {
      setLoading(false);
    }
  };

  const handleUpdate = async (id, updatedData) => {
    try {
      setLoading(true);
      await axios.put(`${BASE_URL}/UpdateRawMaterial/${id}`, updatedData);
      await getAllRawMaterial();
      
      Swal.fire({
        icon: "success",
        title: "Updated successfully",
        showConfirmButton: false,
        timer: 1500,
      });
    } catch (error) {
      handleApiError(error, "Failed to update raw material");
    } finally {
      setLoading(false);
    }
  };

  const valuetoshare = {
    data,
    data1,
    loading,
    getAllRawMaterial,
    handleDelete,
    handleUpdate,
    getRawMaterialById,
  };

  return (
    <RawMaterialContext.Provider value={valuetoshare}>
      {children}
    </RawMaterialContext.Provider>
  );
}

export default RawMaterialContext;
