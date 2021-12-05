import React from 'react';
import { AppBar, Container, Toolbar, Typography } from '@mui/material'
import './NavMenu.css';
import SideBar from './sidebar/SideBar';


const NavMenu = ({ selected, setSelected }) => {
  return (
    <>
      <AppBar position="static">
        <Container maxWidth="xl">
          <Toolbar >
            <SideBar selected={selected} setSelected={setSelected} />
            <Typography
              variant="h6"
              noWrap
              component="div"
              sx={{ mr: 2, display: { xs: 'none', md: 'flex' } }}
            >
              TbsReact
            </Typography>
          </Toolbar>
        </Container>
      </AppBar>
    </>
  );
}

export default NavMenu;
