import { createContext, useState } from 'react';
import axios from 'axios';
import Swal from 'sweetalert2';

const EmployeeContext = createContext();

const API_BASE_URL = 'https://localhost:5001/api';

export function EmployeeContextProvider({ children }) {
    const [dataemp, setDataemp] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleApiError = (error) => {
        const errorMessage = error.response?.data?.message || 'An error occurred';
        setError(errorMessage);
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage,
        });
    };

    async function getAllemployee() {
        try {
            setLoading(true);
            setError(null);
            const response = await axios.get(`${API_BASE_URL}/GetAllEmployee`);
            setDataemp(response.data);
        } catch (error) {
            handleApiError(error);
        } finally {
            setLoading(false);
        }
    }

    async function getAllemp() {
        try {
            setLoading(true);
            setError(null);
            const response = await axios.get(`${API_BASE_URL}/GetAllEmployee`);
            return response.data;
        } catch (error) {
            handleApiError(error);
            return [];
        } finally {
            setLoading(false);
        }
    }

    async function getEmployeeById(id) {
        try {
            setLoading(true);
            setError(null);
            const response = await axios.get(`${API_BASE_URL}/GetEmployeeById/${id}`);
            return response.data;
        } catch (error) {
            handleApiError(error);
            return null;
        } finally {
            setLoading(false);
        }
    }

    async function deleteemployee(id) {
        try {
            setLoading(true);
            setError(null);
            console.log("id",id);
            await axios.delete(`${API_BASE_URL}/DeleteEmployeeById?id=${id}`);
            return true;
        } catch (error) {
            handleApiError(error);
            return false;
        } finally {
            setLoading(false);
        }
    }

    const handleDelete = async (id) => {
        const result = await Swal.fire({
            icon: 'warning',
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
        });

        if (result.isConfirmed) {
            const success = await deleteemployee(id);
            if (success) {
                await Swal.fire({
                    icon: 'success',
                    title: 'Deleted!',
                    showConfirmButton: false,
                    timer: 2000,
                });
                setDataemp(prevData => prevData.filter(employee => employee.employeeId !== id));
            }
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire(
                'Cancelled',
                'Your record is safe :)',
                'info'
            );
        }
    };

    async function updateEmployee(id, updatedData) {
        try {
            setLoading(true);
            setError(null);
            console.log("updatedData",updatedData);
            const response = await axios.put(`${API_BASE_URL}/UpdateEmployee/${id}`, updatedData);
            return response.data;
        } catch (error) {
            handleApiError(error);
            return null;
        } finally {
            setLoading(false);
        }
    }

    const handleupdate = async (id, updatedData) => {
        const updatedEmployee = await updateEmployee(id, updatedData);
        if (updatedEmployee) {
            await getAllemployee();
            Swal.fire({
                icon: 'success',
                title: 'Updated!',
                showConfirmButton: false,
                timer: 2000,
            });
        }
    };

    const valuetoshare = {
        dataemp,
        loading,
        error,
        getAllemployee,
        handleDelete,
        handleupdate,
        getEmployeeById
    };

    return <EmployeeContext.Provider value={valuetoshare}>{children}</EmployeeContext.Provider>;
}

export default EmployeeContext;
