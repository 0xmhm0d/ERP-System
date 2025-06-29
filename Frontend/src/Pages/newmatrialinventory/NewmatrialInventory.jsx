import "./newmatrialinventory.scss";
import Sidebar from './../../Components/sidebar/Sidebar';
import Navbar from './../../Components/navbar/Navbar';
import {useEffect, useContext,useState } from "react";
import  axios  from 'axios';
import Swal from 'sweetalert2';
import { useNavigate } from "react-router-dom";
import RawMatrialContext from "../../context/RawMatrialContext";

function NewmatrialInventory({ inputs, title ,logOut}) {
  const [file, setFile] = useState("");
  const navigate = useNavigate();
  const { getAllRawMaterial, data } = useContext(RawMatrialContext);

  const [rawmatrialinventory,setRawmatrialinventory] = useState({
 rawmatrialinventoryName: "",
  rawmatrialinventoryDescription: ""})
  const productInvOptions = data?.map((product) => {
    return (
      <option value={product.materialId} key={product.materialId}>
        {product.materialId} - {product.materialName}
      </option>
    );
  });
  useEffect(() => {
    getAllRawMaterial();
  }, [rawmatrialinventory]);

  const handleInputChange = (e) => {
    const rawmatrialinventoryData = {...rawmatrialinventory}
    rawmatrialinventoryData[e.target.name]= e.target.value;
    setRawmatrialinventory(rawmatrialinventoryData);


  }
  async function sendData ()
  {
    const rawmatrialinventoryData = {...rawmatrialinventory ,
    
  
   }
   const res = await axios.post (`https://localhost:5001/api/AddNewRawMaterialToInventory`,rawmatrialinventoryData)
   navigate("/rawmatrialinventory");

// console.log('====================================');
// console.log(res);
// console.log('====================================');
  }
  const handleSubmit = (e) =>{
    e.preventDefault();
 
    sendData();
    Swal.fire({
      icon: 'success',
      title: 'Added!',
      text: `${rawmatrialinventory.rawmatrialinventoryName} has been Added.`,
      showConfirmButton: false,
      timer: 1500
  });

  }
  return (
    <div className="newrawmatrialinventory">
    <Sidebar />
    <div className="newContainer">
    <Navbar logOut={logOut}/>
    <div className="top">
      <h1>{title}</h1>
    </div>
    <div className="bottom">
      <div className="left">
        <img
          src={
            file
              ? URL.createObjectURL(file)
              : "https://icon-library.com/images/no-image-icon/no-image-icon-0.jpg"
          }
          alt=""
        />
      </div>
      <div className="right">
        <form onSubmit={handleSubmit} >
        <div className="" style={{ display: "flex", gap: "15px" }}>
        <label htmlFor="materialId">
         Choose Raw-Matrial:
        </label>
        <select
          name="materialId"
          id="materialId"
          onChange={handleInputChange}
        >
          {productInvOptions}
        </select>
      </div>

          {inputs.map((input) => (
            <div className="formInput" key={input.id}>
              <label>{input.label}</label>
              <input type={input.type} name={input.name}  onChange={handleInputChange} placeholder={input.placeholder} />
            </div>
          ))}
          <button>Send</button>
        </form>
      </div>
    </div>
  </div>
    </div>
  );
}
export default NewmatrialInventory

/////////////////////////////////////////////////////////after edit
// import "./newmatrialinventory.scss";
// import Sidebar from './../../Components/sidebar/Sidebar';
// import Navbar from './../../Components/navbar/Navbar';
// import { useEffect, useContext, useState } from "react";
// import axios from 'axios';
// import Swal from 'sweetalert2';
// import { useNavigate } from "react-router-dom";
// import RawMatrialContext from "../../context/RawMatrialContext";

// function NewmatrialInventory({ inputs, title, logOut }) {
//   const [file, setFile] = useState("");
//   const navigate = useNavigate();
//   const { getAllRawMaterial, data } = useContext(RawMatrialContext);

//   const [rawmatrialinventory, setRawmatrialinventory] = useState({
//     materialId: 0,
//     rawmatrialinventoryName: "",
//     rawmatrialinventoryDescription: ""
//   });

//   const productInvOptions = data?.map((product) => {
//     return (
//       <option value={product.materialId} key={product.materialId}>
//         {product.materialId} - {product.materialName}
//       </option>
//     );
//   });

//   useEffect(() => {
//     getAllRawMaterial();
//   }, []);

//   const handleInputChange = (e) => {
//     const rawmatrialinventoryData = { ...rawmatrialinventory };
//     rawmatrialinventoryData[e.target.name] = e.target.value;
//     setRawmatrialinventory(rawmatrialinventoryData);
//   };

//   async function sendData() {
//     const rawmatrialinventoryData = { ...rawmatrialinventory };

//     // تحقق من أن الحقول المطلوبة ليست فارغة
//     if (rawmatrialinventoryData.materialId === 0) {
//       Swal.fire({
//         icon: "error",
//         title: "Error!",
//         text: "Please select a raw material.",
//         showConfirmButton: true,
//       });
//       return;
//     }

//     if (!rawmatrialinventoryData.rawmatrialinventoryName) {
//       Swal.fire({
//         icon: "error",
//         title: "Error!",
//         text: "Please enter a name for the raw material.",
//         showConfirmButton: true,
//       });
//       return;
//     }

//     if (!rawmatrialinventoryData.rawmatrialinventoryDescription) {
//       Swal.fire({
//         icon: "error",
//         title: "Error!",
//         text: "Please enter a description for the raw material.",
//         showConfirmButton: true,
//       });
//       return;
//     }

//     const config = {
//       headers: {
//         "Content-Type": "application/json",
//       },
//     };

//     try {
//       const res = await axios.post(
//         `https://localhost:5001/api/AddNewRawMaterialToInventory`,
//         rawmatrialinventoryData,
//         config
//       );
//       navigate("/rawmatrialinventory");
//       Swal.fire({
//         icon: "success",
//         title: "Added!",
//         text: `${rawmatrialinventory.rawmatrialinventoryName} has been Added.`,
//         showConfirmButton: false,
//         timer: 1500,
//       });
//     } catch (error) {
//       console.error("Error adding raw material to inventory:", error);
//       Swal.fire({
//         icon: "error",
//         title: "Error!",
//         text: "Failed to add raw material to inventory. Please check the data and try again.",
//         showConfirmButton: true,
//       });
//     }
//   }

//   const handleSubmit = (e) => {
//     e.preventDefault();
//     sendData();
//   };

//   return (
//     <div className="newrawmatrialinventory">
//       <Sidebar />
//       <div className="newContainer">
//         <Navbar logOut={logOut} />
//         <div className="top">
//           <h1>{title}</h1>
//         </div>
//         <div className="bottom">
//           <div className="left">
//             <img
//               src={
//                 file
//                   ? URL.createObjectURL(file)
//                   : "https://icon-library.com/images/no-image-icon/no-image-icon-0.jpg"
//               }
//               alt=""
//             />
//           </div>
//           <div className="right">
//             <form onSubmit={handleSubmit}>
//               <div className="" style={{ display: "flex", gap: "15px" }}>
//                 <label htmlFor="materialId">Choose Raw-Material:</label>
//                 <select
//                   name="materialId"
//                   id="materialId"
//                   onChange={handleInputChange}
//                 >
//                   <option value={0}>Select a raw material</option>
//                   {productInvOptions}
//                 </select>
//               </div>

//               {inputs.map((input) => (
//                 <div className="formInput" key={input.id}>
//                   <label>{input.label}</label>
//                   <input
//                     type={input.type}
//                     name={input.name}
//                     onChange={handleInputChange}
//                     placeholder={input.placeholder}
//                   />
//                 </div>
//               ))}
//               <button>Send</button>
//             </form>
//           </div>
//         </div>
//       </div>
//     </div>
//   );
// }

// export default NewmatrialInventory;