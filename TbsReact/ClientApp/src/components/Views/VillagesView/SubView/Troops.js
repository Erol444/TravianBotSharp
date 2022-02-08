import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../../ContentBox";

const Troops = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={6}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Train troop manual" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Research troop manual" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Improve troop manual" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Send scout to other village" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={6}>
					<ContentBox name="village troop info" />
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default Troops;
