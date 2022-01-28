import React, { useState } from "react";
import { Tabs, Tab, Box } from "@mui/material";
import { Link } from "react-router-dom";
const LeftHeader = () => {
	const [value, setValue] = useState("/info");

	const handleChange = (event, newValue) => {
		setValue(newValue);
	};
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
						label="Villages"
						value="/villages"
						to="/villages"
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
