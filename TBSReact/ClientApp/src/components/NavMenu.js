import React from 'react';
import {  Container, Navbar, NavbarBrand} from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import SideBar from './sidebar/SideBar';

const NavMenu = ({ selected, setSelected }) => {

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
        <Container>
          <SideBar selected={selected} setSelected={setSelected}/>
          <NavbarBrand tag={Link} to="/">TBSReact</NavbarBrand>          
        </Container>
      </Navbar>
    </header>
  );
}

export default NavMenu;
