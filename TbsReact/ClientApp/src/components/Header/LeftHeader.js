import React, { useState } from "react";
import PropTypes from "prop-types";
import { Tabs, Tab, Box } from "@mui/material";
import { Link } from "react-router-dom";
const LeftHeader = ({ selected, setSelected }) => {
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

LeftHeader.propTypes = {
	selected: PropTypes.number.isRequired,
	setSelected: PropTypes.func.isRequired,
};

export default LeftHeader;
