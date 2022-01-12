import PropTypes from "prop-types";
import React from "react";
import { AppBar, Container, Toolbar, Typography } from "@mui/material";
import SideBar from "./sidebar/SideBar";
import RightHeader from "./Header/RightHeader";
import LeftHeader from "./Header/LeftHeader";

const NavMenu = ({ selected, setSelected }) => {
	return (
		<>
			<AppBar position="static">
				<Container maxWidth="xl">
					<Toolbar>
						<SideBar
							selected={selected}
							setSelected={setSelected}
						/>
						<Typography
							variant="h6"
							noWrap
							component="div"
							sx={{ mr: 2, display: { xs: "none", md: "flex" } }}
						>
							TbsReact
						</Typography>
						<LeftHeader
							selected={selected}
							setSelected={setSelected}
						/>
						<RightHeader />
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
