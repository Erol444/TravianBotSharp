import React from "react";
import ChromeSettings from "./SettingChild/ChromeSettings";
import { Grid } from "@mui/material";

const General = () => {
	return (
		<>
			<Grid container spacing={1}>
				<Grid item xs={8}>
					<ChromeSettings />
				</Grid>
			</Grid>
		</>
	);
};

export default General;
