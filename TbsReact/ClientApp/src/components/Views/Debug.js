import PropTypes from "prop-types";
import React from "react";
import Grid from "@mui/material/Grid";

const Debug = ({ taskTable, logBoard }) => {
	return (
		<>
			<Grid container spacing={2}>
				<Grid item xs={6}>
					{taskTable}
				</Grid>
				<Grid item xs={6}>
					{logBoard}
				</Grid>
			</Grid>
		</>
	);
};

Debug.propTypes = {
	logBoard: PropTypes.object.isRequired,
	taskTable: PropTypes.object.isRequired,
};

export default Debug;
