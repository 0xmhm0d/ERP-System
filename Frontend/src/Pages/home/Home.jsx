import React from 'react';
import Navbar from '../../Components/navbar/Navbar';
import Sidebar from '../../Components/sidebar/Sidebar';
import Widget from '../../Components/widget/Widget';
import "./home.scss";
import Feature from './../../Components/feature/Feature';
import Charts from './../../Components/Chart/Charts';
import Table  from './../../Components/table/Table';
const Home = ({logOut}) => {
  return (
    <div className='home'>
    
    <Sidebar/>
    <div className="homeContainer">
    
    <Navbar logOut={logOut}/>

    
 

    <div className='chart'>
    <iframe title="GP_ERP_BI" width="1140" height="541.25" src="https://app.powerbi.com/reportEmbed?reportId=50a04272-3b42-4f1a-87bf-1646d4c6463e&autoAuth=true&ctid=a7d1894d-7354-4c76-9ca8-fe9a3d434e6e" frameborder="0" allowFullScreen="true"></iframe>    </div>
    </div>
    </div>
  )
}

export default Home