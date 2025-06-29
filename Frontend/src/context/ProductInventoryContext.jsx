import { createContext, useState, useCallback } from 'react';
import axios from 'axios';
import Swal from 'sweetalert2';

const BASE_URL = 'https://localhost:5001/api';

const ProductInventoryContext = createContext();

export function ProductInventoryContextProvider({ children }) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);

    // Fetch all products
    const getProductInventory = useCallback(async () => {
        try {
            setLoading(true);
            const response = await axios.get(`${BASE_URL}/GetAllProductsInInventory`);
            setData(response.data);
        } catch (error) {
            console.error('Error fetching inventory:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to fetch inventory data'
            });
        } finally {
            setLoading(false);
        }
    }, []);

    // Get product by ID
    const getProductInventoryById = async (id) => {
        try {
            const response = await axios.get(`${BASE_URL}/GetProductInInventoryById/${id}`);
            return response;
        } catch (error) {
            console.error('Error fetching product:', error);
            throw error;
        }
    };

    // Delete product
    const deleteProductInventory = async (id) => {
        try {
            await axios.delete(`${BASE_URL}/DeleteProductFromInventory/${id}`);
            setData(prevData => prevData.filter(item => item.productId !== id));
        } catch (error) {
            console.error('Error deleting product:', error);
            throw error;
        }
    };

    // Handle delete with confirmation
    const handleDelete = async (id) => {
        try {
            const result = await Swal.fire({
                icon: 'warning',
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                showCancelButton: true,
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'No, cancel!'
            });

            if (result.isConfirmed) {
                await deleteProductInventory(id);
                await Swal.fire({
                    icon: 'success',
                    title: 'Deleted!',
                    showConfirmButton: false,
                    timer: 2000
                });
            }
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to delete product'
            });
        }
    };

    // Update product
    const handleUpdate = async (updatedData) => {
        try {
            setLoading(true);
            await axios.put(`${BASE_URL}/UpdateProductInInventory`, updatedData);
            await getProductInventory();
        } catch (error) {
            console.error('Error updating product:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to update product'
            });
        } finally {
            setLoading(false);
        }
    };

    // Add new product to inventory
    const addProductToInventory = async (productData) => {
        try {
            setLoading(true);
            await axios.post(`${BASE_URL}/AddAProductToInventory`, productData);
            await getProductInventory(); // Refresh the list
        } catch (error) {
            console.error('Error adding product to inventory:', error);
            throw new Error(error.response?.data?.message || 'Failed to add product to inventory');
        } finally {
            setLoading(false);
        }
    };

    const value = {
        data,
        loading,
        getProductInventory,
        handleDelete,
        handleUpdate,
        getProductInventoryById,
        addProductToInventory
    };

    return (
        <ProductInventoryContext.Provider value={value}>
            {children}
        </ProductInventoryContext.Provider>
    );
}

export default ProductInventoryContext;
