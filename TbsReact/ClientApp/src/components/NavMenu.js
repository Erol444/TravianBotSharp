import PropTypes from "prop-types";
import React from "react";
import {
  AppBar,
  Container,
  Toolbar,
  Typography,
  Tab,
  Tabs,
} from "@mui/material";
import SideBar from "./sidebar/SideBar";
import { Link } from "react-router-dom";
const NavMenu = ({ selected, setSelected }) => {
  const [value, setValue] = React.useState("/info");

  const handleChange = (event, newValue) => {
    setValue(newValue);
  };
  return (
    <>
      <AppBar position="static">
        <Container maxWidth="xl">
          <Toolbar>
            <SideBar selected={selected} setSelected={setSelected} />
            <Typography
              variant="h6"
              noWrap
              component="div"
              sx={{ mr: 2, display: { xs: "none", md: "flex" } }}
            >
              TbsReact
            </Typography>
            {/* Im not sure how to change style now, that is why these tab has black instead of white color */}
            <Tabs value={value} onChange={handleChange}>
              <Tab label="Debug" value="/debug" to="/debug" component={Link} />
              <Tab label="Info" value="/info" to="/info" component={Link} />
            </Tabs>
          </Toolbar>
        </Container>
      </AppBar>
    </>
  );
};

NavMenu.propTypes = {
  selected: PropTypes.number.isRequired,
  setSelected: PropTypes.func.isRequired,
};

export default NavMenu;
