import React from 'react';
import NavMenu from './NavMenu';

//debug view
import Debug from './Views/Debug';
import LogBoard from './Views/Debug/LogBoard'

const Layout = ({ selected, setSelected, isConnect }) => {
  return (
    <>
      <NavMenu selected={selected} setSelected={setSelected} />
      <Debug 
        logBoard={<LogBoard selected={selected} isConnect={isConnect}/>}          
        />
    </>
  );

}

export default Layout;