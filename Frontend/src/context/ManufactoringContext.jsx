import { createContext, useState  } from 'react';
import axios  from 'axios';


const ManufactoringContext = createContext ();

export function ManufactoringContextProvider({children})
{
    const [data, setData] = useState([]);
    async function getAllmanufactur()
    {
        try {
            const allmanufactur = await axios.get(`https://localhost:5001/api/GetAllManufacturingOrders`);
            setData(allmanufactur.data);
            
        } catch (error) {
            console.error('Error fetching manufacturing orders:', error);
            setData([]); 
            throw error;
        }
    }
    async function GetManfacturingOrderById(id)
    {
        try {
            const manuObject = await axios.get(`https://localhost:5001/api/GetManufacturingOrderById/${id}`);
            return manuObject;
        } catch (error) {
            console.error('Error fetching manufacturing order:', error);
            throw error;
        }
    }

    async function createManufacturingOrder(orderData) {
        try {
            const response = await axios.post('https://localhost:5001/api/CreateManufacturingOrder', orderData);
            return response;
        } catch (error) {
            console.error('Error creating manufacturing order:', error);
            throw error;
        }
    }

    async function changeStatusToManufacturing(orderId) {
        try {
            const response = await axios.put(`https://localhost:5001/api/ChangeManufacturingStatusToManufacturing?orderId=${orderId}`);
            return response;
        } catch (error) {
            console.error('Error changing status to manufacturing:', error);
            throw error;
        }
    }

    async function changeStatusToShipped(orderId) {
        try {
            const response = await axios.put(`https://localhost:5001/api/ChangeManufacturingStatusToShippedToInventory?orderId=${orderId}`);
            return response;
        } catch (error) {
            console.error('Error changing status to shipped:', error);
            throw error;
        }
    }

    const valuetoshare = {
        data,
        getAllmanufactur,
        GetManfacturingOrderById,
        createManufacturingOrder,
        changeStatusToManufacturing,
        changeStatusToShipped
    }

    return <ManufactoringContext.Provider value={valuetoshare}>{children}</ManufactoringContext.Provider>
}



export default ManufactoringContext;
