import React from "react";
import ChromeSettings from "./ChromeSettings";
import { Grid } from "@mui/material";

const General = () => {
	return (
		<>
			<Grid container spacing={3}>
				<Grid item xs={8}>
					<ChromeSettings />
				</Grid>
			</Grid>
		</>
	);
};

export default General;
