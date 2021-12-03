import React from 'react';
import NavMenu from './NavMenu';

const Layout = ({ selected, setSelected }) => {
  return (
    <>
      <NavMenu selected={selected} setSelected={setSelected} />
    </>
  );

}

export default Layout;