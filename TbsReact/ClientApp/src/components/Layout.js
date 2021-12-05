import React from 'react';
import NavMenu from './NavMenu';
import Info from './Views/Info';

const Layout = ({ selected, setSelected }) => {
  return (
    <>
      <NavMenu selected={selected} setSelected={setSelected} />
      <Info/>
    </>
  );

}

export default Layout;