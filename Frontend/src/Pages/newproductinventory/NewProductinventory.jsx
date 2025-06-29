import "./newproductinventory.scss";
import Sidebar from './../../Components/sidebar/Sidebar';
import Navbar from './../../Components/navbar/Navbar';
import { useEffect, useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from 'sweetalert2';
import AllproductContext from "../../context/AllproductContext";
import ProductInventoryContext from "../../context/ProductInventoryContext";

function NewProductinventory({ inputs, title, logOut }) {
    const navigate = useNavigate();
    const { getAllproduct, data: products } = useContext(AllproductContext);
    const { addProductToInventory, loading } = useContext(ProductInventoryContext);

    const [formData, setFormData] = useState({
        productId: '',
        quantity: '',
        shippingDate: new Date().toISOString().split('T')[0],
        monthlyCosts: '',
        area: '',
        reorderingPoint: ''
    });

    useEffect(() => {
        getAllproduct();
    }, []);

    const productInvOptions = products?.map((product) => (
        <option value={product.productId} key={product.productId}>
            {product.productId} - {product.productName}
        </option>
    ));

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: ['productIdhttps://localhost:5001/api/reorderingPoint'].includes(name)
                ? Number(value) || 0
                : value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Validation
        if (!formData.productId) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Please select a product'
            });
            return;
        }

        try {
            await addProductToInventory(formData);
            Swal.fire({
                icon: 'success',
                title: 'Added!',
                text: 'Product has been added to inventory',
                showConfirmButton: false,
                timer: 1500
            });
            navigate("/productsinventory");
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: error.message || 'Failed to add product to inventory'
            });
        }
    };

    return (
        <div className="NewProductinventory">
            <Sidebar />
            <div className="newproductContainer">
                <Navbar logOut={logOut} />
                <div className="top">
                    <h1>{title}</h1>
                </div>
                <div className="bottom">
                    <div className="right">
                        <form onSubmit={handleSubmit}>
                            <div className="form-group">
                                <label htmlFor="productId">Choose Product:</label>
                                <select
                                    name="productId"
                                    id="productId"
                                    onChange={handleInputChange}
                                    value={formData.productId}
                                    required
                                >
                                    <option value="">Select a product</option>
                                    {productInvOptions}
                                </select>
                            </div>

                            {inputs.map((input) => (
                                <div className="formInput" key={input.id}>
                                    <label>{input.label}</label>
                                    <input
                                        type={input.type}
                                        name={input.name}
                                        value={formData[input.name]}
                                        onChange={handleInputChange}
                                        placeholder={input.placeholder}
                                        required
                                    />
                                </div>
                            ))}
                            <button type="submit" disabled={loading}>
                                {loading ? 'Adding...' : 'Send'}
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default NewProductinventory;