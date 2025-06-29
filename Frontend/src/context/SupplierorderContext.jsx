import { createContext, useState  } from 'react';
import axios  from 'axios';


const SupplierorderContext = createContext ();

export function SupplierorderContextProvider({children})
{
    const [data, setData] = useState([]);
    async function getAllSupplierorder()
    {
        // const token = localStorage.getItem('token');
        const allSupplierorder = await axios.get(`https://localhost:5001/api/GetAllSupplierOrders`);
        console.log(allSupplierorder)

    //  const allSupplierorder = await axios.get(`https://localhost:5001/api/GetAllSupplierOrders`);
     setData(allSupplierorder.data);
    }
    async function GetSupplierOrderById(id)
    {
           const manuObject = await axios.get(`https://localhost:5001/api/GetSupplierOrderById/${id}`);
           return manuObject ;
          
    }

    // Functions to change supplier order status
    async function changeSupplierOrderStatusToShipped(orderId) {
        try {
            const response = await axios.put(
                `https://localhost:5001/api/ChangeSupplierOrderStatusToShipped`,
                null,
                { params: { orderId } }
            );
            return response;
        } catch (error) {
            console.error("Error changing order status to Shipped:", error);
            throw error;
        }
    }

    async function changeSupplierOrderStatusToFulfilled(orderId) {
        try {
            const response = await axios.put(
                `https://localhost:5001/api/ChangeSupplierOrderStatusToFulfilled`,
                null,
                { params: { orderId } }
            );
            return response;
        } catch (error) {
            console.error("Error changing order status to Fulfilled:", error);
            throw error;
        }
    }

    const valuetoshare = {
        data,
        getAllSupplierorder,
        GetSupplierOrderById,
        changeSupplierOrderStatusToShipped,
        changeSupplierOrderStatusToFulfilled
    }  
    return <SupplierorderContext.Provider value={valuetoshare}>{children}</SupplierorderContext.Provider>
}



export default SupplierorderContext;
