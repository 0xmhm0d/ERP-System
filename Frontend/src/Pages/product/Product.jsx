import { Link ,useParams,useNavigate} from "react-router-dom";
import "./product.css";
import Sidebar from "../../Components/sidebar/Sidebar";
import Navbar from './../../Components/navbar/Navbar';
import { useContext ,useEffect,useState} from 'react';
import AllproductContext from "../../context/AllproductContext";
import Swal from 'sweetalert2';


export default function Product({logOut}) {
    const {productId} = useParams(); 
    const {handleupdate,getProductById} = useContext (AllproductContext);
    const [prodata, setProdata] = useState({
        productName: "",
        productDescription: "",
        purchasePrice: "",
        salesPrice: "",
        categoryId: ""
    });
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        // Validate form data
        if (!prodata.productName || !prodata.productDescription) {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Product name and description are required.',
                confirmButtonText: 'OK'
            });
            return;
        }

        // Convert numeric values
        const updatedData = {
            ...prodata,
            purchasePrice: Number(prodata.purchasePrice),
            salesPrice: Number(prodata.salesPrice),
            categoryId: Number(prodata.categoryId)
        };

        try {
            await handleupdate(productId, updatedData);
            Swal.fire({
                icon: 'success',
                title: 'Updated!',
                text: `${updatedData.productName} has been updated.`,
                showConfirmButton: false,
                timer: 1500
            });
            navigate("/products");
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Failed to update product. Please try again.',
                confirmButtonText: 'OK'
            });
            console.error('Update error:', error);
        }
    }

    const handleChange = (e) => {
        const { name, value } = e.target;
        setProdata(prev => ({
            ...prev,
            [name]: value
        }));
    }

    async function getproduct() {
        try {
            setIsLoading(true);
            setError(null);
            const product = await getProductById(productId);
            if (!product || !product.data) {
                throw new Error('Product data not found');
            }
            setProdata(product.data);
        } catch (error) {
            setError(error.message);
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Failed to load product data. Please try again.',
                confirmButtonText: 'OK'
            }).then(() => {
                navigate("/products");
            });
            console.error('Fetch error:', error);
        } finally {
            setIsLoading(false);
        }
    }

    useEffect(() => {
        getproduct();
    }, []);

    return (
        <div className="list">
            <Sidebar/>
            <div className="listContainer">
                <Navbar logOut={logOut}/>
                <div className="product">
                    <div className="productTitleContainer">
                        <h1 className="productTitle">Product</h1>
                        <Link to="/products/newproduct">
                            <button className="productAddButton">Create</button>
                        </Link>
                    </div>
                    {isLoading ? (
                        <div className="loadingContainer">
                            <div className="loadingSpinner"></div>
                            <p>Loading product data...</p>
                        </div>
                    ) : error ? (
                        <div className="errorContainer">
                            <p className="errorMessage">{error}</p>
                            <button 
                                className="retryButton"
                                onClick={() => {
                                    setError(null);
                                    getproduct();
                                }}
                            >
                                Retry
                            </button>
                        </div>
                    ) : (
                        <div className="productTop">
                            <div className="productTopLeft">
                                <form className="productForm" onSubmit={handleSubmit}>
                                    <div className="productFormLeft">
                                        <label>Product Name</label>
                                        <input 
                                            type="text" 
                                            onChange={handleChange} 
                                            placeholder="Enter product name" 
                                            name="productName" 
                                            value={prodata?.productName}
                                            required
                                        />
                                    </div>
                                    <div className="productFormLeft">
                                        <label>Product Description</label>
                                        <input 
                                            type="text" 
                                            onChange={handleChange} 
                                            placeholder="Enter product description" 
                                            name="productDescription" 
                                            value={prodata?.productDescription}
                                            required
                                        />
                                    </div>
                                    <div className="productFormLeft">
                                        <label>Purchase Price</label>
                                        <input 
                                            type="number" 
                                            onChange={handleChange} 
                                            placeholder="Enter purchase price" 
                                            name="purchasePrice" 
                                            value={prodata?.purchasePrice}
                                            min="0"
                                            step="0.01"
                                        />
                                    </div>
                                    <div className="productFormLeft">
                                        <label>Sales Price</label>
                                        <input 
                                            type="number" 
                                            onChange={handleChange} 
                                            placeholder="Enter sales price" 
                                            name="salesPrice" 
                                            value={prodata?.salesPrice}
                                            min="0"
                                            step="0.01"
                                        />
                                    </div>
                                    <div className="productFormLeft">
                                        <label>Category ID</label>
                                        <input 
                                            type="number" 
                                            onChange={handleChange} 
                                            placeholder="Enter category ID" 
                                            name="categoryId" 
                                            value={prodata?.categoryId}
                                            min="1"
                                        />
                                    </div>
                                    <button 
                                        className="productAddButton" 
                                        type="submit"
                                        disabled={isLoading}
                                    >
                                        {isLoading ? 'Updating...' : 'Update'}
                                    </button>
                                </form>
                            </div>
                            <div className="productTopRight">
                                <div className="productInfoTop">
                                    <p className="categoryName">
                                        <span className="spanform">Product Name: </span>
                                        {prodata?.productName}
                                    </p>
                                    <p className="paddorder">
                                        <span className="spanform">Description: </span>
                                        {prodata?.productDescription}
                                    </p>
                                    <p className="paddorder">
                                        <span className="spanform">Purchase Price: </span>
                                        ${prodata?.purchasePrice}
                                    </p>
                                    <p className="paddorder">
                                        <span className="spanform">Sales Price: </span>
                                        ${prodata?.salesPrice}
                                    </p>
                                    <p className="paddorder">
                                        <span className="spanform">Category ID: </span>
                                        {prodata?.categoryId}
                                    </p>
                                </div>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}
