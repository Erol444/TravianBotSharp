import React, { useState, useEffect } from "react";
import { Tabs, Tab, Box } from "@mui/material";
import { Link, useLocation } from "react-router-dom";
const LeftHeader = () => {
	const location = useLocation();
	const [value, setValue] = useState("/info");

	const handleChange = (event, newValue) => {
		setValue(newValue);
	};

	useEffect(() => {
		const path = location.pathname.split("/")[1];
		setValue(`/${path || "info"}`);
	}, [location]);
	return (
		<>
			<Box sx={{ flex: 1 }}>
				<Tabs value={value} onChange={handleChange} textColor="inherit">
					<Tab
						label="Setting"
						value="/setting"
						to="/setting"
						component={Link}
					/>
					<Tab
						label="Hero"
						value="/hero"
						to="/hero"
						component={Link}
					/>
					<Tab
						label="Villages"
						value="/villages"
						to="/villages"
						component={Link}
					/>
					<Tab
						label="Overview"
						value="/overview"
						to="/overview"
						component={Link}
					/>
					<Tab
						label="Farming"
						value="/farming"
						to="/farming"
						component={Link}
					/>
					<Tab
						label="New villages"
						value="/newvills"
						to="/newvills"
						component={Link}
					/>
					<Tab
						label="Debug"
						value="/debug"
						to="/debug"
						component={Link}
					/>
					<Tab
						label="Info"
						value="/info"
						to="/info"
						component={Link}
					/>
				</Tabs>
			</Box>
		</>
	);
};

export default LeftHeader;
