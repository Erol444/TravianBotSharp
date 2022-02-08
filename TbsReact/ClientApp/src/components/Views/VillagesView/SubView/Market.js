import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../../ContentBox";

const Market = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={5}>
					<ContentBox name="NPC Manual" />
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default Market;
