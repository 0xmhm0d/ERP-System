import { createContext, useState } from 'react';
import axios from 'axios';
import Swal from 'sweetalert2';

const BASE_URL = 'https://localhost:5001/api';

const SupplierContext = createContext();

export function SupplierContextProvider({ children }) {
    const [data, setData] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [rowInputs, setrowInputs] = useState({ materialId: "", pricePerUnit: "" });

    const handleApiError = (error, customMessage) => {
        console.error(customMessage, error);
        throw error;
    };

    const showSuccessAlert = (message) => {
        Swal.fire({
            icon: 'success',
            title: message,
            showConfirmButton: false,
            timer: 2000,
        });
    };

    const showErrorAlert = (message) => {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: message,
        });
    };

    async function getAllsupplier() {
        setIsLoading(true);
        setError(null);
        try {
            const response = await axios.get(`${BASE_URL}/GetAllSuppliers`);
            setData(response.data);
        } catch (err) {
            const errorMessage = 'Failed to load suppliers. Please try again later.';
            console.error('Error fetching suppliers:', err);
            setError(errorMessage);
            setData([]);
        } finally {
            setIsLoading(false);
        }
    }

    async function getAllsupply() {
        try {
            const response = await axios.get(`${BASE_URL}/GetAllSuppliers`);
            return response.data;
        } catch (err) {
            return handleApiError(err, 'Error fetching suppliers:');
        }
    }

    async function getSupllierById(id) {
        try {
            const response = await axios.get(`${BASE_URL}/GetSupplierById/${id}`);
            return response;
        } catch (err) {
            return handleApiError(err, `Error fetching supplier with ID ${id}:`);
        }
    }

    async function getSuplliermatrialById(id) {
        try {
            const response = await axios.get(`${BASE_URL}/GetSuppliersMaterials/${id}`);
            return response;
        } catch (err) {
            return handleApiError(err, `Error fetching supplier materials for ID ${id}:`);
        }
    }

    async function deletesupplier(id) {
        try {
            const response = await axios.delete(`${BASE_URL}/DeleteSupplier?id=${id}`);
            return response;
        } catch (err) {
            return handleApiError(err, `Error deleting supplier with ID ${id}:`);
        }
    }

    const handleDelete = (id) => {
        Swal.fire({
            icon: 'warning',
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
        }).then(async (result) => {
            if (result.value) {
                try {
                    await deletesupplier(id);
                    showSuccessAlert('Deleted!');
                    setData(data.filter(employee => employee.supplierId !== id));
                } catch (err) {
                    showErrorAlert('Failed to delete supplier. Please try again.');
                }
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                Swal.fire(
                    'Cancelled',
                    'Your supplier is safe :)',
                    'error'
                );
            }
        });
    };

    async function updateSupplier(id, updatedData) {
        try {
            const response = await axios.put(`${BASE_URL}/UpdateSupplier/${id}`, updatedData);
            return response;
        } catch (err) {
            return handleApiError(err, `Error updating supplier with ID ${id}:`);
        }
    }

    const handleupdate = async (id, updatedData) => {
        try {
            await updateSupplier(id, updatedData);
            await getAllsupplier();
        } catch (err) {
            console.error('Error updating supplier:', err);
            showErrorAlert('Failed to update supplier. Please try again.');
        }
    };

    const valuetoshare = {
        data,
        isLoading,
        error,
        getAllsupplier,
        handleDelete,
        handleupdate,
        getSupllierById,
        getSuplliermatrialById
    };

    return <SupplierContext.Provider value={valuetoshare}>{children}</SupplierContext.Provider>
}

export default SupplierContext;
