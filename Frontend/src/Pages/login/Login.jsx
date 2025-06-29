import React, { Fragment , useState} from 'react'
import axios from 'axios';
import {useNavigate} from 'react-router-dom';
import Joi from 'joi';
import "./login.css"
import pic from "./../../Components/sidebar/logo.jpg";
import { columnGroupsStateInitializer } from '@mui/x-data-grid/internals';
export default function Login({saveUserData}) {
  

  let navigate = useNavigate();
  
const [errorList, seterrorList] = useState([]);
const [error, setError] = useState('');
const [isLoading, setisLoading] = useState(false);
 const [user, setUser] = useState({

    
        email: "Admin@gmail.com",
        password: "P@$$w0rd1234"
      
 });
 function getUserData(e)
 {
  let myUser ={...user};
    // console.log("hello");
    // myUser.first_name = e.target.value;
    myUser[e.target.name] = e.target.value;
    setUser (myUser);
    // console.log(myUser);
 }
async function sendLoginDataToApi(e)
 {
    e.preventDefault();
    console.log(user)

  const config = {
    headers: {
      "Content-Type": "application/json",
    },
  };
  let {data} =  await axios.post(`https://localhost:5001/api/Login`,user,config);
  console.log(data)
  if(data.token)
  {

      setisLoading(false);
      localStorage.setItem('token', data.token);
      localStorage.setItem('email', data.email);
      localStorage.setItem('name', data.userName);
      localStorage.setItem('role', data.role);
      //
      saveUserData();
      navigate('/home');


  }
  else{
    setisLoading(false);
    setError(data.message);
    console.log("error");
    

  }
 
 }

//  function submitLoginForm(e)
//  {
//    e.preventDefault();
//    setisLoading(true);
//       // sendLoginDataToApi();
//      let validation =  validateLoginForm();
//      if(validation.error)
//      {
//           setisLoading(false);
//           seterrorList(validation.error.details);
//           console.log("error is",validation.error.details)
//         ///
          


//      }
//      else{
//       sendLoginDataToApi();
//      }
//  }

 function validateLoginForm()
 {
   let scheme =   Joi.object({
        
        email:Joi.string().email({ minDomainSegments: 2, tlds: { allow: ['com', 'net'] } }).required(),
        password:Joi.string()

      
      });
    return scheme.validate(user,{abortEarly:false});
 }
  return<Fragment>
  
  {
  //   errorList.map((err,index)=> {
  //   if(err.context.label ==='password')
  //   {
  //      return <div key={index} className="alert alert-danger my-2">password invalied 😒..</div>
  //   }
  //   else{
  //     return <div key={index} className="alert alert-danger my-2">{err.message}</div>
  //   }
  // }

  //    )
}

  {
//     error.length >0 ? <div className="alert alert-danger my-2">{error}</div>
// :''
}

{
//   <form onSubmit={submitLoginForm}>
    
//     <label htmlFor="email">email :</label>
//     <input  onChange={getUserData} type="email" className='form-control my-input-log my-2' name='email' />
//     {errorList.filter((err)=> err.context.label === 'email')[0]?<div className="alert alert-danger my-2">
//     <p>{errorList.filter((err)=> err.context.label === 'email')[0]?.message}</p>
//  </div>:''}
//     <label htmlFor="password">password :</label>
//     <input  onChange={getUserData} type="password" className='form-control my-input-log my-2' name='password' />
//     {errorList.filter((err)=> err.context.label === 'password')[0]?<div className="alert alert-danger my-2">
//     <p>password invalied 😒.. must the initial letter be capital</p>
//  </div>:''}
//     <button  type='submit' className='btn btn-info'>
//      {isLoading === true ?<i className='fas fa-spinner fa-spin'></i>:'Login'}
//     </button>
//   </form>
}
<div className='body'>
<img src={pic} alt="" />
<form onSubmit={sendLoginDataToApi}>

<div className="container">
        <div className="card">
            <p className="login">Log in</p>
            <div className="inputBox">
            <input  onChange={getUserData} value={user.email} type="email" name='email'  required="required"/>
            <span className="user">Email</span>
            {errorList.filter((err)=> err.context.label === 'email')[0]?<div className="alert alert-danger my-2">
    <p>{errorList.filter((err)=> err.context.label === 'email')[0]?.message}</p>
  </div>:''}
            </div>

            <div className="inputBox">
             <input  onChange={getUserData} value={user.password} type="password"  name='password'  required="required" />
            <span>Password</span>
            {errorList.filter((err)=> err.context.label === 'password')[0]?<div className="alert alert-danger my-2">
              <p>password invalied 😒..</p>
            </div>:''}
            </div>

            <button  type='submit' className='enter'>
            {isLoading === true ?<i className='fas fa-spinner fa-spin'></i>:'Login'}
             </button>
        </div>
    </div>
</form>

</div>
  </Fragment>
}


