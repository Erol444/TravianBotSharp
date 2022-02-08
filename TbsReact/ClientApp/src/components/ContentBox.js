import PropTypes from "prop-types";
import React from "react";
import { Box } from "@mui/material";

import style from "../styles/box";

const ContentBox = ({ children, name }) => {
	return (
		<>
			<Box sx={style}>
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
