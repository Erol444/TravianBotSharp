import PropTypes from "prop-types";
import React from "react";
import { Box } from "@mui/material";

import boxStyle from "../styles/box";

const ContentBox = ({ children, name }) => {
	return (
		<>
			<Box sx={boxStyle}>
				{children}

				{name}
			</Box>
		</>
	);
};

ContentBox.propTypes = {
	children: PropTypes.any,
	name: PropTypes.string,
};

export default ContentBox;
