import React from "react";
import PropTypes from "prop-types";
import ChromeSettings from "./GeneralChild/ChromeSettings";
import { Grid } from "@mui/material";

const General = ({ selected }) => {
	return (
		<>
			<Grid container spacing={1}>
				<Grid item xs={8}>
					<ChromeSettings selected={selected} />
				</Grid>
			</Grid>
		</>
	);
};

General.propTypes = {
	selected: PropTypes.number.isRequired,
};

export default General;
