import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../../ContentBox";

const Attack = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={8}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Button add wave" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="wave planned list" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={4}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Oasis farming" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Send scout manual to player id" />
						</Grid>
					</Grid>
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default Attack;
