import React from "react";
import { AppBar, Container, Toolbar, Typography } from "@mui/material";
import SideBar from "./sidebar/SideBar";
import RightHeader from "./Header/RightHeader";
import LeftHeader from "./Header/LeftHeader";

const NavMenu = () => {
	return (
		<>
			<AppBar position="static">
				<Container maxWidth="xl">
					<Toolbar>
						<SideBar />
						<Typography
							variant="h6"
							noWrap
							component="div"
							sx={{ mr: 2, display: { xs: "none", md: "flex" } }}
						>
							TravianBotSharp
						</Typography>
						<LeftHeader />
						<RightHeader />
					</Toolbar>
				</Container>
			</AppBar>
		</>
	);
};
export default NavMenu;
