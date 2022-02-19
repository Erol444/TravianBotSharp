import PropTypes from "prop-types";
import React from "react";
import { Box } from "@mui/material";

import boxStyle from "../styles/box";
import modalStyle from "../styles/modal";

const ContentBox = ({ children, name, modal }) => {
	return (
		<>
			<Box sx={modal === true ? modalStyle : boxStyle}>
				{children}

				{name}
			</Box>
		</>
	);
};

ContentBox.propTypes = {
	children: PropTypes.any,
	name: PropTypes.string,
	modal: PropTypes.bool,
};

export default ContentBox;
