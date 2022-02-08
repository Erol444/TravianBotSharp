import PropTypes from "prop-types";
import React from "react";
import { Box } from "@mui/material";

import style from "../styles/box";

const ContentBox = ({ name }) => {
	return (
		<>
			<Box sx={style}>
				<h1>[WIP] this is placeholder</h1>

				{name}
			</Box>
		</>
	);
};

ContentBox.propTypes = {
	name: PropTypes.string.isRequired,
};

export default ContentBox;
